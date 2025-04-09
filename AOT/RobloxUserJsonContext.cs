#if NET6_0_OR_GREATER
using RobloxUserOnlineTracker.Models;
using System.Text.Json.Serialization;

namespace RobloxUserOnlineTracker.AOT
{
    [JsonSerializable(typeof(RobloxUser))]
    internal partial class RobloxUserJsonContext : JsonSerializerContext
    {

    }
}
#endif