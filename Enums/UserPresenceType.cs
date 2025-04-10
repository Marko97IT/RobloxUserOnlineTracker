// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

namespace RobloxUserOnlineTracker.Enums
{
    /// <summary>
    /// Represents the online presence type of a user.
    /// </summary>
    public enum UserPresenceType : byte
    {
        /// <summary>
        /// Indicates that the user is offline.
        /// </summary>
        Offline,
        /// <summary>
        /// Indicates that the user is online and is navigating the website.
        /// </summary>
        Online,
        /// <summary>
        /// Indicates that the user is in a game.
        /// </summary>
        InGame,
        /// <summary>
        /// Indicates that the user is in Roblox Studio.
        /// </summary>
        Studio,
        /// <summary>
        /// The user presence is unknown.
        /// </summary>
        Unknown
    }
}