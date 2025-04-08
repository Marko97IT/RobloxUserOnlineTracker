// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

namespace RobloxUserOnlineTracker.Enums
{
    /// <summary>
    /// Represents the online presence type of a user.
    /// </summary>
    public enum UserPresenceType : byte
    {
        Offline,
        Online,
        InGame,
        Studio,
        Unknown
    }
}