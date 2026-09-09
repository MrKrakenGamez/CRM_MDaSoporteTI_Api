using System.Security.Claims;
using CRM_MDaSoporteTI.Shared.Result;
using Microsoft.AspNetCore.Mvc;

namespace CRM_MDaSoporteTI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class CrmBaseController : ControllerBase
{
    /// <summary>Id del usuario autenticado, extraído del claim "userId" del JWT.</summary>
    protected int? CurrentUserId =>
        int.TryParse(User.FindFirstValue("userId"), out var id) ? id : null;

    /// <summary>IP del cliente que originó la petición.</summary>
    protected string? ClientIp =>
        HttpContext.Connection.RemoteIpAddress?.ToString();

    /// <summary>Identificador único de la petición HTTP actual, usado como SessionId de auditoría.</summary>
    protected string SessionId => HttpContext.TraceIdentifier;

    protected IActionResult ToAction<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(ApiResponse<T>.Ok(result.Value!));

        return StatusCode(result.Error!.HttpStatus, ApiResponse<T>.Fail(result.Error));
    }

    protected IActionResult ToAction(Result result)
    {
        if (result.IsSuccess)
            return Ok(ApiResponse.Ok());

        return StatusCode(result.Error!.HttpStatus, ApiResponse.Fail(result.Error));
    }

    protected IActionResult ToPagedAction<T>(
        Result<(IEnumerable<T> Items, int Total)> result, int page, int pageSize)
    {
        if (result.IsSuccess)
            return Ok(PagedApiResponse<T>.Ok(result.Value.Items, result.Value.Total, page, pageSize));

        return StatusCode(result.Error!.HttpStatus, PagedApiResponse<T>.Fail(result.Error));
    }

    protected IActionResult CreatedFromResult<T>(Result<T> result, string actionName, object routeValues)
    {
        if (result.IsSuccess)
            return CreatedAtAction(actionName, routeValues, ApiResponse<T>.Ok(result.Value!));

        return StatusCode(result.Error!.HttpStatus, ApiResponse<T>.Fail(result.Error));
    }
}