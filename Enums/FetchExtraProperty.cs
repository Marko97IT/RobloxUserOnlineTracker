// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

namespace RobloxUserOnlineTracker.Enums
{
    /// <summary>
    /// Enum representing the extra properties that can be fetched while tracking.
    /// </summary>
    [Flags]
    public enum FetchExtraProperty : byte
    {
        /// <summary>
        /// Fetches the user's profile details.
        /// </summary>
        UserData,
        /// <summary>
        /// Fetches the user's avatar thumbnail.
        /// </summary>
        UserAvatarThumbnail
    }
}