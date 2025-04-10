// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

#if NET6_0_OR_GREATER
using RobloxUserOnlineTracker.AOT;
#endif
using RobloxUserOnlineTracker.DTOs.Requests;
using RobloxUserOnlineTracker.DTOs.Responses;
using RobloxUserOnlineTracker.Enums;
using RobloxUserOnlineTracker.Events;
using RobloxUserOnlineTracker.Exceptions;
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
        /// Event triggered when an error occurs during tracking.
        /// </summary>
        public event EventHandler<Exception>? TrackingErrorOccurred;

        /// <summary>
        /// Initializes a new instance of the <see cref="RobloxUserOnlineTrackerClient"/> class with the specified Roblox authentication cookie value.
        /// </summary>
        /// <param name="cookieValue">The <c>.ROBLOSECURITY</c> cookie value.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="RobloxUserOnlineTrackerException"></exception>
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
                    { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36" }
                }
            };

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, "https://trades.roblox.com/v1/users/1/can-trade-with");
                using var response = _httpClient.Send(request);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new RobloxUserOnlineTrackerException("Unable to determine if the .ROBLOSECURITY cookie value is valid.", ex);
            }
        }

        /// <summary>
        /// Releases the resources used by the <see cref="RobloxUserOnlineTrackerClient"/> class.
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
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="RobloxUserOnlineTrackerException"></exception>
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

                            if (previousState != userOnlinePresence.Presence)
                            {
                                _userPreviousStates[userOnlinePresence.User.Id] = userOnlinePresence.Presence;

                                if (!userFirstTrack)
                                {
                                    UserOnlinePresenceChanged?.Invoke(this, new UserPresenceChangedEventArgs(userOnlinePresence));
                                }
                            }
                        });

                        await Task.Delay(trackInterval, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        StopTracking();

                        if (ex is RobloxUserOnlineTrackerException)
                        {
                            TrackingErrorOccurred?.Invoke(this, ex);
                            return;
                        }

                        if (ex is not TaskCanceledException)
                        {
                            TrackingErrorOccurred?.Invoke(this, new RobloxUserOnlineTrackerException("Unable to get user online presences. See the inner exception for more details", ex));
                            return;
                        }
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
        /// <exception cref="RobloxUserOnlineTrackerException"></exception>
        public async Task<RobloxUserOnlinePresence[]> GetUserOnlinePresenceAsync(long[] userIds, CancellationToken cancellationToken = default)
        {
            try
            {
                var payload = new UserPresenceRequest { UserIds = userIds };
#if NET6_0_OR_GREATER
                using var response = await _httpClient.PostAsJsonAsync("https://presence.roblox.com/v1/presence/users", payload, UserPresenceRequestJsonContext.Default.UserPresenceRequest, cancellationToken);
#else
                using var response = await _httpClient.PostAsJsonAsync("https://presence.roblox.com/v1/presence/users", payload, cancellationToken);
#endif
                response.EnsureSuccessStatusCode();

#if NET6_0_OR_GREATER
                var userPresenceResponse = await response.Content.ReadFromJsonAsync(UserPresenceResponseJsonContext.Default.UserPresenceResponse, cancellationToken);
#else
                var userPresenceResponse = await response.Content.ReadFromJsonAsync<UserPresenceResponse>(cancellationToken: cancellationToken);
#endif
                if (userPresenceResponse != null)
                {
                    return userPresenceResponse.UserPresences.Select(up => new RobloxUserOnlinePresence(up.UserId)
                    {
                        Presence = (UserPresenceType)up.UserPresenceType,
                        CurrentLocation = up.UserPresenceType == 0 ? string.Empty : up.LastLocation,
                        GameId = up.PlaceId,
                        GameInstanceId = up.GameId
                    }).ToArray();
                }
                else
                {
                    throw new RobloxUserOnlineTrackerException("Unable to deserialize the response");
                }
            }
            catch (Exception ex)
            {
                if (ex is not RobloxUserOnlineTrackerException)
                {
                    throw new RobloxUserOnlineTrackerException("Unable to get user online presences. See the inner exception for more details", ex); ;
                }

                throw;
            }
        }
    }
}