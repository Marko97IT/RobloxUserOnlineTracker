// Copyright (c) 2025 - Marco Concas. All rights reserved.
// Licensed under the MPL-2.0 License. License informations are available here: https://mozilla.org/MPL/2.0/

namespace RobloxUserOnlineTracker.Extensions
{
    internal static class HttpClientExtension
    {
        public static string ReadAsString(this HttpContent content)
        {
            return content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
    }
}