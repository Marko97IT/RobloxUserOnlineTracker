// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

namespace RobloxUserOnlineTracker.Exceptions
{
    /// <summary>
    /// The exception that is thrown for errors with <see cref="RobloxUserOnlineTrackerClient"/> instance.
    /// </summary>
    public class RobloxUserOnlineTrackerException : Exception
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="RobloxUserOnlineTrackerException"/> class.
        /// </summary>
        public RobloxUserOnlineTrackerException()
        {
            
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="RobloxUserOnlineTrackerException"/> class with a specified error message.
        /// </summary>
        public RobloxUserOnlineTrackerException(string message) : base(message)
        {
            
        }


        /// <summary>
        /// Initialize a new instance of the <see cref="RobloxUserOnlineTrackerException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        public RobloxUserOnlineTrackerException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }
}