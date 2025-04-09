// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

#if NET6_0_OR_GREATER
using RobloxUserOnlineTracker.AOT;
#endif
using RobloxUserOnlineTracker.Enums;
using RobloxUserOnlineTracker.Exceptions;
using RobloxUserOnlineTracker.Extensions;
using System.Text.Json;

namespace RobloxUserOnlineTracker.Models
{
    /// <summary>
    /// Represents the online presence of a Roblox user.
    /// </summary>
    public sealed class RobloxUserOnlinePresence
    {
        private static HttpClient _httpClient = new();
        private static bool _isDisposed = false;

        /// <summary>
        /// Get the user informations as a <see cref="RobloxUser"/> object.
        /// </summary>
        public RobloxUser User { get; internal set; } = default!;
        /// <summary>
        /// Gets the actual presence of the user.
        /// </summary>
        public UserPresenceType UserPresence { get; internal set; }
        /// <summary>
        /// Get the location name where the user is currently located. This information is only available if the user shares this information with you or publicly.
        /// </summary>
        public string CurrentLocation { get; internal set; } = string.Empty;
        /// <summary>
        /// Get the location ID where the user is currently located. This information is only available if the user shares this information with you or publicly.
        /// </summary>
        public long? GameId { get; internal set; }
        /// <summary>
        /// Get the instance ID of the location where the user is currently located. It can be used to join the user experience. This information is only available if the user shares this information with you or publicly.
        /// </summary>
        public Guid? GameInstanceId { get; internal set; }

        internal RobloxUserOnlinePresence(long userId)
        {
            if (_isDisposed)
            {
                _httpClient = new HttpClient();
                _isDisposed = false;
            }

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"https://users.roblox.com/v1/users/{userId}");
                using var response = _httpClient.Send(request);
                response.EnsureSuccessStatusCode();

                var responseContent = response.Content.ReadAsString();
#if NET6_0_OR_GREATER
                User = JsonSerializer.Deserialize(responseContent, RobloxUserJsonContext.Default.RobloxUser) ?? throw new RobloxUserOnlineTrackerException("Unable to deserialize the response");
#else
                User = JsonSerializer.Deserialize<RobloxUser>(responseContent) ?? throw new RobloxUserOnlineTrackerException("Unable to deserialize the response");
#endif
            }
            catch (Exception ex)
            {
                if (ex is not RobloxUserOnlineTrackerException)
                {
                    throw new RobloxUserOnlineTrackerException("Unable to fetch user details", ex);
                }
                else
                {
                    throw;
                }
            }
        }

        internal static void Dispose()
        {
            if (!_isDisposed)
            {
                _httpClient.Dispose();
                _isDisposed = true;
            }
        }
    }
}