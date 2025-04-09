#nullable disable warnings
using System.Text.Json.Serialization;

namespace RobloxUserOnlineTracker.DTOs.Requests
{
    internal class UserPresenceRequest
    {
        [JsonPropertyName("userIds")]
        public long[] UserIds { get; set; }
    }
}