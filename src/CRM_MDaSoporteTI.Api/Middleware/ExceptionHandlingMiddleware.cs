// ============================================================
// Red de seguridad: captura CUALQUIER excepción no controlada que
// se escape de un UseCase (los errores "esperados" del SP ya viajan
// como Result<T> y nunca deberían llegar aquí).
// ============================================================
using System.Net;
using System.Text.Json;
using CRM_MDaSoporteTI.Shared.Result;

namespace CRM_MDaSoporteTI.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> log)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Excepción no controlada en {Path}", context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var body = ApiResponse.Fail(new ResultError(
                "UNEXPECTED_ERROR",
                "Ocurrió un error inesperado. Intenta nuevamente o contacta a soporte.",
                500));

            await context.Response.WriteAsync(JsonSerializer.Serialize(body,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
    }
}