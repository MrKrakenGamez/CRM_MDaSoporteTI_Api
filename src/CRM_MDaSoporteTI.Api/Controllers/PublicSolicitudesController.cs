using CRM_MDaSoporteTI.Application.DTOs.Requests;
using CRM_MDaSoporteTI.Application.UseCases.Solicitudes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_MDaSoporteTI.Api.Controllers;

[AllowAnonymous]
[Route("api/public-solicitudes")]
public sealed class PublicSolicitudesController(
    CrearSolicitudHandler crearHandler) : CrmBaseController
{
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearSolicitudRequest request, CancellationToken ct)
    {
        var result = await crearHandler.HandleAsync(request, userId: null, ClientIp, SessionId, ct);

        // TODO (próxima iteración): IEmailService.NotificarNuevaSolicitudAsync(...) -> mgames@inixio.com

        return CreatedFromResult(
            result,
            actionName: nameof(SolicitudesController.ObtenerPorId),
            routeValues: new { controller = "Solicitudes", id = result.IsSuccess ? result.Value!.IdSolicitud : 0 });
    }
}