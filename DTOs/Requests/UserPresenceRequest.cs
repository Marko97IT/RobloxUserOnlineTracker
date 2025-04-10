// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

#nullable disable warnings
using System.Text.Json.Serialization;

namespace RobloxUserOnlineTracker.DTOs.Requests
{
    internal class UserPresenceRequest
    {
        [JsonPropertyName("userIds")]
        public long[] UserIds { get; set; }
    }
}