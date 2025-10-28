using System.Text.Json;
using FQ.Results;

namespace FQ.Cqrs;

internal static class JsonSerializerExtensions
{
    public static bool TryDeserializeResultFromJson(this string json, out Result result, JsonSerializerOptions? options = null)
    {
        return HandleDeserialization(() => JsonResultSerializer.Deserialize(json, options), out result);
    }
    
    public static bool TryDeserializeResultFromJson(this byte[] jsonBytes, out Result result, JsonSerializerOptions? options = null)
    {
        return HandleDeserialization(() => JsonResultSerializer.Deserialize(jsonBytes, options), out result);
    }
    
    public static bool TryDeserializeResultFromJson<TResponse>(this string json, out Result<TResponse> result, JsonSerializerOptions? options = null)
    {
        return HandleDeserialization(() => JsonResultSerializer.Deserialize<TResponse>(json, options), out result);
    }
    
    public static bool TryDeserializeResultFromJson<TResponse>(this byte[] jsonBytes, out Result<TResponse> result, JsonSerializerOptions? options = null)
    {
        return HandleDeserialization(() => JsonResultSerializer.Deserialize<TResponse>(jsonBytes, options), out result);
    }

    private static bool HandleDeserialization<TResponse>(Func<TResponse> deserializationAction, out TResponse? result)
    {
        try
        {
            result = deserializationAction();

            return true;
        }
        catch (JsonException)
        {
            result = default;

            return false;
        }
        catch (NotSupportedException)
        {
            result = default;

            return false;
        }
    }
}