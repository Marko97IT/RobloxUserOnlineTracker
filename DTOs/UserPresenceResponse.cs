// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

#nullable disable warnings
using System.Text.Json.Serialization;

namespace RobloxUserOnlineTracker.DTOs
{
    internal class UserPresenceResponse
    {
        [JsonPropertyName("userPresences")]
        public UserPresence[] UserPresences { get; set; }
    }

    internal class UserPresence
    {
        [JsonPropertyName("userPresenceType")]
        public int UserPresenceType { get; set; }
        [JsonPropertyName("lastLocation")]
        public string LastLocation { get; set; }
        [JsonPropertyName("placeId")]
        public long? PlaceId { get; set; }
        [JsonPropertyName("rootPlaceId")]
        public long? RootPlaceId { get; set; }
        [JsonPropertyName("gameId")]
        public Guid? GameId { get; set; }
        [JsonPropertyName("universeId")]
        public long? UniverseId { get; set; }
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
    }
}