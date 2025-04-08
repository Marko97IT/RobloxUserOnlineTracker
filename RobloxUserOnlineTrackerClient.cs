// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

using RobloxUserOnlineTracker.DTOs;
using RobloxUserOnlineTracker.Enums;
using RobloxUserOnlineTracker.Events;
using RobloxUserOnlineTracker.Models;
using System.Net;
using System.Net.Http.Json;

namespace RobloxUserOnlineTracker
{
    /// <summary>
    /// Represents the client for tracking the online presence of Roblox users.
    /// </summary>
    public class RobloxUserOnlineTrackerClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private Dictionary<long, UserPresenceType>? _userPreviousStates;
        private bool _isTracking;

        /// <summary>
        /// Event triggered when a user's online presence changes.
        /// </summary>
        public event EventHandler<UserPresenceChangedEventArgs>? UserOnlinePresenceChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="RobloxUserOnlineTrackerClient"/> class with the specified Roblox authentication cookie value.
        /// </summary>
        /// <param name="cookieValue">The <c>.ROBLOSECURITY</c> cookie value.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public RobloxUserOnlineTrackerClient(string cookieValue)
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
        }

        /// <summary>
        /// Releases the resources used by the <see cref="TrackerOnlineStatusClient"/> class.
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
            RobloxUserOnlinePresence.Dispose();
            UserOnlinePresenceChanged = null;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts tracking the online presence of Roblox users.
        /// </summary>
        /// <param name="userIds">The user IDs of the online presence of the user you want to track.</param>
        /// <param name="trackInterval">The interval in milliseconds for each update.</param>
        /// <exception cref="ArgumentException"></exception>
        public void StartTracking(long[] userIds, int trackInterval = 5000, CancellationToken cancellationToken = default)
        {
            if (_isTracking)
            {
                return;
            }

            if (userIds.Length == 0)
            {
                throw new ArgumentException("User IDs cannot be empty", nameof(userIds));
            }

            if (trackInterval < 5000)
            {
                throw new ArgumentException("Track interval must be at least 5000 milliseconds", nameof(trackInterval));
            }

            _ = Task.Run(async () =>
            {
                _isTracking = true;
                _userPreviousStates = new Dictionary<long, UserPresenceType>(userIds.Length);

                while (_isTracking)
                {
                    try
                    {
                        var userOnlinePresences = await GetUserOnlinePresenceAsync(userIds, cancellationToken);
                        Parallel.ForEach(userOnlinePresences, (userOnlinePresence, state) =>
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                state.Stop();
                                return;
                            }

                            var userFirstTrack = !_userPreviousStates.TryGetValue(userOnlinePresence.User.Id, out var previousState);

                            if (previousState != userOnlinePresence.UserPresence)
                            {
                                _userPreviousStates[userOnlinePresence.User.Id] = userOnlinePresence.UserPresence;

                                if (!userFirstTrack)
                                {
                                    UserOnlinePresenceChanged?.Invoke(this, new UserPresenceChangedEventArgs(userOnlinePresence));
                                }
                            }
                        });

                        await Task.Delay(trackInterval, cancellationToken);
                    }
                    catch (TaskCanceledException)
                    {
                        StopTracking();
                    }
                    catch
                    {
                        StopTracking();
                        throw new Exception("Unable to get user online presences. The tracking was stopped");
                    }
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Stops tracking the online presence of Roblox users.
        /// </summary>
        public void StopTracking()
        {
            _isTracking = false;
            _userPreviousStates = null;
        }

        /// <summary>
        /// Gets the online presence of the specified Roblox users.
        /// </summary>
        /// <param name="userIds">The user IDs of the users whose online presence you want to get.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>An array with the user online presences.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<RobloxUserOnlinePresence[]> GetUserOnlinePresenceAsync(long[] userIds, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PostAsJsonAsync("https://presence.roblox.com/v1/presence/users", new { userIds }, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var userPresenceResponse = await response.Content.ReadFromJsonAsync<UserPresenceResponse>(cancellationToken: cancellationToken);

                if (userPresenceResponse != null)
                {
                    return userPresenceResponse.UserPresences.Select(up => new RobloxUserOnlinePresence(up.UserId)).ToArray();
                }
            }

            throw new Exception("Unable to get user online presences");
        }
    }
}