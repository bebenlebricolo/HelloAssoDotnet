using System.Text.Json;
using System.Text.Json.Serialization;

namespace HelloAssoDotnet.Utils;

/// <summary>
/// Basic tool which helps to get standard JsonOptions for serialization/deserialization
/// </summary>
public static class JsonOptionsProvider
{
    /// <summary>
    /// Builds basic json options
    /// </summary>
    /// <returns></returns>
    public static JsonSerializerOptions GetJsonOptions()
    {
        return new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };
    }
}
