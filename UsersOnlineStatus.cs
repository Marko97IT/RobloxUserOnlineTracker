// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

namespace RobloxUserOnlineTracker
{
    /// <summary>
    /// Represents the online status of users on Roblox.
    /// </summary>
    public class UsersOnlineStatus
    {
        /// <summary>
        /// The list of user IDs of the Roblox users and their current online status.
        /// </summary>
        public Dictionary<long, UserPresenceType> Content { get; set; } = [];
    }
}