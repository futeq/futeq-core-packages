using System.Text.Json;

namespace FQ.Cqrs;

internal static class JsonSerializerExtensions
{
    public static bool TryDeserializeFromJson<TResponse>(this string json, out TResponse? result, JsonSerializerOptions? options = null)
    {
        try
        {
            result = JsonSerializer.Deserialize<TResponse>(json, options);

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
    
    public static bool TryDeserializeFromJson<TResponse>(this byte[] jsonBytes, out TResponse? result, JsonSerializerOptions? options = null)
    {
        try
        {
            result = JsonSerializer.Deserialize<TResponse>(jsonBytes, options);

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