using FQ.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace FQ.AspNetCore;

public static class ResultActionResults
{
    public static IActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.NoContent();
        }
        
        var p = result.ToProblemShape(controller.Request?.Path.Value);
        
        return controller.Problem(
            detail: p.Detail,
            instance: p.Instance,
            statusCode: p.Status,
            title: p.Title,
            type: p.Type,
            extensions: p.WithCorrelationExtensions(controller.HttpContext)
        );
    }

    public static ActionResult<T> ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return result.Value!;
        }
        
        var p = result.ToProblemShape(controller.Request?.Path.Value);
        
        return controller.Problem(
            detail: p.Detail,
            instance: p.Instance,
            statusCode: p.Status,
            title: p.Title,
            type: p.Type,
            extensions: p.WithCorrelationExtensions(controller.HttpContext)
        );
    }

    public static IResult ToHttpResult(this Result result, HttpContext http)
    {
        if (result.IsSuccess)
        {
            return HttpResults.NoContent();
        }
        var p = result.ToProblemShape(http.Request?.Path.Value);
        
        return HttpResults.Problem(p.Detail, p.Instance, p.Status, p.Title, p.Type, extensions: p.WithCorrelationExtensions(http));
    }

    public static IResult ToHttpResult<T>(this Result<T> result, HttpContext http)
    {
        if (result.IsSuccess)
        {
            return HttpResults.Ok(result.Value);
        }
        
        var p = result.ToProblemShape(http.Request?.Path.Value);
        
        return HttpResults.Problem(p.Detail, p.Instance, p.Status, p.Title, p.Type, extensions: p.WithCorrelationExtensions(http));
    }
}