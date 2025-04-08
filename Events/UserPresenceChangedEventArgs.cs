// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

using RobloxUserOnlineTracker.Models;

namespace RobloxUserOnlineTracker.Events
{
    /// <summary>
    /// Represents the event arguments for the <see cref="RobloxUserOnlineTrackerClient.UserStatusChanged"/> event.
    /// </summary>
    public sealed class UserPresenceChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the online presence of the Roblox user.
        /// </summary>
        public RobloxUserOnlinePresence UserPresence { get; }

        internal UserPresenceChangedEventArgs(RobloxUserOnlinePresence userPresence)
        {
            UserPresence = userPresence;
        }
    }
}