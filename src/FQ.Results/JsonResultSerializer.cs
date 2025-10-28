using System.Text.Json;

namespace FQ.Results;
 
/// <summary>
/// Used for JSON Serialization operations to avoid direct deserialisations
/// of Results (due to immutability)
/// </summary>
public static class JsonResultSerializer
{
    public static Result Deserialize(byte[] json, JsonSerializerOptions? options = null)
    {
        var jsonResult = JsonSerializer.Deserialize<JsonResult>(json, options);
        var result = Result.FromJsonResult(jsonResult);

        return result;
    }
    
    public static Result Deserialize(string json, JsonSerializerOptions? options = null)
    {
        var jsonResult = JsonSerializer.Deserialize<JsonResult>(json, options);
        var result = Result.FromJsonResult(jsonResult);

        return result;
    }
    
    public static Result<TResponse> Deserialize<TResponse>(byte[] json, JsonSerializerOptions? options = null)
    {
        var jsonResult = JsonSerializer.Deserialize<JsonResult<TResponse>>(json, options); 
        var result = Result<TResponse>.FromJsonResult(jsonResult);

        return result;
    }
    
    public static Result<TResponse> Deserialize<TResponse>(string json, JsonSerializerOptions? options = null)
    {
        var jsonResult = JsonSerializer.Deserialize<JsonResult<TResponse>>(json, options); 
        var result = Result<TResponse>.FromJsonResult(jsonResult); 

        return result;
    }
}