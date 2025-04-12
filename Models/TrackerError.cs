// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

namespace RobloxUserOnlineTracker.Models
{
    /// <summary>
    /// Represents an error that occurred during the tracking process.
    /// </summary>
    public sealed class TrackerError
    {
        /// <summary>
        /// Get the exception that caused the error.
        /// </summary>
        public Exception Exception { get; internal set; } = default!;
        /// <summary>
        /// Get the current retry attempt.
        /// </summary>
        public int RetryAttempt { get; internal set; }
        /// <summary>
        /// Get the maximum number of retry attempts.
        /// </summary>
        public int MaxRetryAttempts { get; internal set; }
        /// <summary>
        /// Get the date and time of the last attempt.
        /// </summary>
        public DateTime LastAttemptTime { get; internal set; }
        /// <summary>
        /// Get if the tracker has been stopped.
        /// </summary>
        public bool TrackerStopped { get; internal set; }

        internal TrackerError()
        {

        }
    }
}