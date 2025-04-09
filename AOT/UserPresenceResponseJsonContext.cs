#if NET6_0_OR_GREATER
using RobloxUserOnlineTracker.DTOs.Responses;
using System.Text.Json.Serialization;

namespace RobloxUserOnlineTracker.AOT
{
    [JsonSerializable(typeof(UserPresenceResponse))]
    [JsonSerializable(typeof(UserPresence))]
    internal partial class UserPresenceResponseJsonContext : JsonSerializerContext
    {

    }
}
#endif