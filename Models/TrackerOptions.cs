// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

using RobloxUserOnlineTracker.Enums;

namespace RobloxUserOnlineTracker.Models
{
    /// <summary>
    /// Rappresent options for tracking user online presence.
    /// </summary>
    public sealed class TrackerOptions
    {
        internal HashSet<byte>? ExtraProperties { get; private set; }
        internal long[] UserIds { get; private set; } = default!;
        internal int Interval { get; private set; } = 5000;
        internal bool RetryOnErrorValue { get; private set; }
        internal int RetryTimesValue { get; private set; }
        internal int RetryAfterValue { get; private set; }

        internal TrackerOptions()
        {
            
        }

        /// <summary>
        /// Set extra properties to fetch while tracking.
        /// </summary>
        /// <param name="properties">An array of extra properties.</param>
        /// <returns></returns>
        public TrackerOptions WithExtraProperties(ExtraProperty properties)
        {
            ExtraProperties = new HashSet<byte>(Enum.GetValues<ExtraProperty>()
                .Where(prop => properties.HasFlag(prop))
                .Select(prop => (byte)prop)
            );

            return this;
        }

        /// <summary>
        /// Set the user IDs to track.
        /// </summary>
        /// <param name="userIds">An array of user IDs.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public TrackerOptions WithUserIds(params long[] userIds)
        {
            if (userIds.Length == 0)
            {
                throw new ArgumentException("User IDs cannot be empty", nameof(userIds));
            }

            UserIds = userIds;
            return this;
        }

        /// <summary>
        /// Set the interval for tracking in milliseconds.
        /// </summary>
        /// <param name="milliseconds">The interval as milliseconds. Interval must be at least <c>5000</c> milliseconds.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public TrackerOptions WithInterval(int milliseconds)
        {
            if (milliseconds < 5000)
            {
                throw new ArgumentOutOfRangeException(nameof(milliseconds), "Interval must be at least 5000 milliseconds");
            }

            Interval = milliseconds;
            return this;
        }

        /// <summary>
        /// Set whether to retry on error.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public TrackerOptions RetryOnError(bool value)
        {
            RetryOnErrorValue = value;
            return this;
        }

        /// <summary>
        /// Set the number of retry attempts.
        /// </summary>
        /// <param name="times">The retry attempts.</param>
        /// <returns></returns>
        public TrackerOptions RetryTimes(int times)
        {
            RetryTimesValue = times;
            return this;
        }

        /// <summary>
        /// Set the retry delay in milliseconds.
        /// </summary>
        /// <param name="milliseconds">The retry delay in milliseconds.</param>
        /// <returns></returns>
        public TrackerOptions RetryAfter(int milliseconds)
        {
            RetryAfterValue = milliseconds;
            return this;
        }
    }
}