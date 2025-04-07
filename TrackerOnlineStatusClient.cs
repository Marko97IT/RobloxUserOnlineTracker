// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

using RobloxUserOnlineTracker.DTOs;
using System.Net;
using System.Net.Http.Json;
using Timer = System.Timers.Timer;

namespace RobloxUserOnlineTracker
{
    /// <summary>
    /// Represents a client for tracking the online status of Roblox users.
    /// </summary>
    public class TrackerOnlineStatusClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly Timer _trackerTimer;
        private Dictionary<long, UserPresenceType> _trackerPreviouslyUsersStatus = [];
        private bool _firstTrackTick = true;
        private long[] _trackUserIds = default!;

        /// <summary>
        /// Event triggered when a user's online status changes.
        /// </summary>
        public event EventHandler<UserOnlineStatusChangedEventArgs>? UserStatusChanged;
        /// <summary>
        /// Event triggered when the Roblox cookie expires. When the event is triggered, tracking stops automatically.
        /// </summary>
        public event EventHandler? RobloxCookieExpired;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackerOnlineStatusClient"/> class with the specified Roblox authentication cookie value.
        /// </summary>
        /// <param name="cookieValue">The <c>.ROBLOSECURITY</c> cookie value.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public TrackerOnlineStatusClient(string cookieValue)
        {
            if (string.IsNullOrEmpty(cookieValue))
            {
                throw new ArgumentNullException(nameof(cookieValue), "Cookie value cannot be null or empty");
            }

            var httpClientHandler = new HttpClientHandler
            {
                CookieContainer = new CookieContainer()
            };

            httpClientHandler.CookieContainer.Add(new Cookie
            {
                Name = ".ROBLOSECURITY",
                Value = cookieValue,
                Domain = ".roblox.com",
                Path = "/",
                Secure = true,
                HttpOnly = true,
            });

            _httpClient = new(httpClientHandler)
            {
                DefaultRequestHeaders =
                {
                    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36" }
                }
            };

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, "https://trades.roblox.com/v1/users/1/can-trade-with");
                using var response = _httpClient.Send(request);
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                throw new InvalidOperationException("Invalid cookie value");
            }

            _trackerTimer = new();
            _trackerTimer.Elapsed += async (sender, e) =>
            {
                try
                {
                    var usersStatus = await GetUsersOnlineStatusAsync(_trackUserIds);
                    Parallel.ForEach(usersStatus.Content, (userStatus, cancellationToken) =>
                    {
                        if (_trackerPreviouslyUsersStatus.TryGetValue(userStatus.Key, out var previousStatus))
                        {
                            if (previousStatus != userStatus.Value)
                            {
                                UserStatusChanged?.Invoke(this, new UserOnlineStatusChangedEventArgs(userStatus.Key, userStatus.Value));
                            }
                        }
                        else
                        {
                            if (!_firstTrackTick)
                            {
                                UserStatusChanged?.Invoke(this, new UserOnlineStatusChangedEventArgs(userStatus.Key, userStatus.Value));
                            }
                        }

                        _trackerPreviouslyUsersStatus[userStatus.Key] = userStatus.Value;
                    });

                    _firstTrackTick = false;
                }
                catch { }
            };
        }

        /// <summary>
        /// Releases the resources used by the <see cref="TrackerOnlineStatusClient"/> class.
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
            _trackerTimer.Dispose();
            UserStatusChanged = null;
            RobloxCookieExpired = null;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts tracking the online status of the specified Roblox users.
        /// </summary>
        /// <param name="userIds">The user IDs of the online status of the user you want to track.</param>
        /// <param name="trackInterval">The interval in milliseconds for each update.</param>
        /// <exception cref="ArgumentException"></exception>
        public void StartTracking(long[] userIds, int trackInterval = 5000)
        {
            if (userIds.Length == 0)
            {
                throw new ArgumentException("User IDs cannot be empty", nameof(userIds));
            }

            _trackerTimer.Interval = trackInterval;
            _trackUserIds = userIds;
            _firstTrackTick = true;
            _trackerTimer.Start();
        }

        /// <summary>
        /// Stops tracking the online status of Roblox users.
        /// </summary>
        public void StopTracking()
        {
            _trackerTimer.Stop();
            _trackUserIds = [];
            _trackerPreviouslyUsersStatus = [];
        }

        /// <summary>
        /// Asynchronously retrieves the online status of the specified Roblox users.
        /// </summary>
        /// <param name="userIds">The user IDs of the users whose online status you want to get.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A list of user IDs of the Roblox users and their current online status.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<UsersOnlineStatus> GetUsersOnlineStatusAsync(long[] userIds, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PostAsJsonAsync("https://presence.roblox.com/v1/presence/users", new { userIds }, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var userPresenceResponse = await response.Content.ReadFromJsonAsync<UserPresenceResponse>(cancellationToken: cancellationToken);

                if (userPresenceResponse != null)
                {
                    return new UsersOnlineStatus
                    {
                        Content = userPresenceResponse.UserPresences.ToDictionary(up => up.UserId, up => (UserPresenceType)up.UserPresenceType)
                    };
                }
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    RobloxCookieExpired?.Invoke(this, EventArgs.Empty);
                    StopTracking();
                }
            }

            throw new Exception("Unable to get users status");
        }
    }
}