namespace RobloxUserOnlineTracker
{
    /// <summary>
    /// Represents the event arguments for the <see cref="TrackerOnlineStatusClient.UserStatusChanged"/> event.
    /// </summary>
    public sealed class UserOnlineStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the user ID of the Roblox user whose online status has changed.
        /// </summary>
        public long UserId { get; }
        /// <summary>
        /// Gets the new online status of the Roblox user.
        /// </summary>
        public UserPresenceType UserStatus { get; }

        internal UserOnlineStatusChangedEventArgs(long userId, UserPresenceType userStatus)
        {
            UserId = userId;
            UserStatus = userStatus;
        }
    }
}