#if NET6_0_OR_GREATER
using RobloxUserOnlineTracker.DTOs.Requests;
using System.Text.Json.Serialization;

namespace RobloxUserOnlineTracker.AOT
{
    [JsonSerializable(typeof(UserPresenceRequest))]
    internal partial class UserPresenceRequestJsonContext : JsonSerializerContext
    {

    }
}
#endif