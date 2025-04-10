﻿// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

#if NET6_0_OR_GREATER
using RobloxUserOnlineTracker.DTOs.Responses;
using System.Text.Json.Serialization;

namespace RobloxUserOnlineTracker.AOT
{
    [JsonSerializable(typeof(UserPresenceResponse))]
    [JsonSerializable(typeof(UserPresence))]
    internal partial class UserPresenceResponseJsonContext : JsonSerializerContext
    {

    }
}
#endif