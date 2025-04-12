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
        private bool _isTracking;

        /// <summary>
        /// Event triggered when a user's online presence changes.
        /// </summary>
        public event EventHandler<UserPresenceChangedEventArgs>? UserOnlinePresenceChanged;
        /// <summary>
        /// Event triggered when an error occurs during tracking.
        /// </summary>
        public event EventHandler<(Exception Exception, int? RetryAttempt, bool? TrackerStopped)>? TrackingErrorOccurred;

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
            TrackingErrorOccurred = null;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts tracking the online presence of Roblox users in a new thread. Subscribe <see cref="TrackingErrorOccurred"/> event to handle errors.
        /// </summary>
        /// <param name="configure">The configuration for the tracker.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <exception cref="RobloxUserOnlineTrackerException"></exception>
        public void StartTracking(Action<TrackerOptions> configure, CancellationToken cancellationToken = default)
        {
            if (_isTracking)
            {
                return;
            }

            var options = new TrackerOptions();
            configure(options);

            if (options.RetryOnErrorValue)
            {
                if (options.RetryTimesValue < 1)
                {
                    throw new RobloxUserOnlineTrackerException("Retry times must be at least 1");
                }

                if (options.RetryAfterValue < 1)
                {
                    throw new RobloxUserOnlineTrackerException("Retry after must be at least 1 millisecond");
                }
            }

            _ = Task.Run(async () =>
            {
                _isTracking = true;

                var userPreviousStates = new Dictionary<long, byte>(options.UserIds.Length);
                var firstTrack = true;
                var retryAttempt = 0;

                while (_isTracking)
                {
                    try
                    {
                        var userOnlinePresences = await GetUserOnlinePresenceAsync(options.UserIds, cancellationToken);

                        Parallel.ForEach(userOnlinePresences, (userOnlinePresence, state) =>
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                state.Stop();
                                return;
                            }

                            if (userPreviousStates.GetValueOrDefault(userOnlinePresence.User.Id) != (byte)userOnlinePresence.Presence)
                            {
                                userPreviousStates[userOnlinePresence.User.Id] = (byte)userOnlinePresence.Presence;
                                
                                if (!firstTrack)
                                {
                                    UserOnlinePresenceChanged?.Invoke(this, new UserPresenceChangedEventArgs(userOnlinePresence));
                                }
                            }
                        });

                        firstTrack = false;
                        retryAttempt = 0;

                        await Task.Delay(options.Interval);
                    }
                    catch (Exception ex)
                    {
                        if (ex is RobloxUserOnlineTrackerException)
                        {
                            TrackingErrorOccurred?.Invoke(this, (ex, retryAttempt, retryAttempt == options.RetryTimesValue));
                        }
                        else if (ex is not TaskCanceledException)
                        {
                            TrackingErrorOccurred?.Invoke(this, (new RobloxUserOnlineTrackerException("Unable to get user online presences. See the inner exception for more details", ex), retryAttempt, retryAttempt == options.RetryTimesValue));
                        }

                        if (options.RetryOnErrorValue && retryAttempt < options.RetryTimesValue)
                        {
                            await Task.Delay(options.RetryAfterValue);

                            retryAttempt++;
                            continue;
                        }

                        StopTracking();
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