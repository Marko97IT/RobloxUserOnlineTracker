// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

using System.Text.Json.Serialization;

namespace RobloxUserOnlineTracker.Models
{
    /// <summary>
    /// Represents a Roblox user.
    /// </summary>
    public sealed class RobloxUser
    {
        /// <summary>
        /// Get the user about description.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("description")]
        public string About { get; internal set; } = string.Empty;
        /// <summary>
        /// Get the date and time when the user was created.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created")]
        public DateTime Created { get; internal set; }
        /// <summary>
        /// Get if the user is banned.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("isBanned")]
        public bool IsBanned { get; internal set; }
        /// <summary>
        /// Get if the user has the verified badge.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("hasVerifiedBadge")]
        public bool HasVerifiedBadge { get; internal set; }
        /// <summary>
        /// Get the user ID.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public long Id { get; internal set; }
        /// <summary>
        /// Get the user name.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Username { get; internal set; } = default!;
        /// <summary>
        /// Get the user display name.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; internal set; }
    }
}