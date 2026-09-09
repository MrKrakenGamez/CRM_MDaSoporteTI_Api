// ============================================================
// Api/Controllers/SolicitudesController.cs
// ============================================================
using CRM_MDaSoporteTI.Application.DTOs.Requests;
using CRM_MDaSoporteTI.Application.UseCases.Solicitudes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM_MDaSoporteTI.Api.Controllers;

[Authorize]
public sealed class SolicitudesController(
    ActualizarSolicitudHandler actualizarHandler,
    AsignarSolicitudHandler asignarHandler,
    ActualizarEstatusSolicitudHandler actualizarEstatusHandler,
    ConcluirSolicitudHandler concluirHandler,
    ObtenerSolicitudHandler obtenerHandler,
    ListarSolicitudesHandler listarHandler,
    ObtenerEstadisticasHandler estadisticasHandler) : CrmBaseController
{
    /// <summary>Listado paginado con filtros — panel administrativo.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] ListarSolicitudesRequest request, CancellationToken ct)
    {
        var result = await listarHandler.HandleAsync(request, CurrentUserId, ct);
        return ToPagedAction(result, request.PageNumber, request.PageSize);
    }

    /// <summary>Detalle completo de una solicitud, incluyendo su historial de auditoría.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id, CancellationToken ct)
    {
        var result = await obtenerHandler.HandleAsync(id, CurrentUserId, ct);
        return ToAction(result);
    }

    /// <summary>Edita los campos administrables de la solicitud (no toca estatus/asignación).</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarSolicitudRequest request, CancellationToken ct)
    {
        var result = await actualizarHandler.HandleAsync(id, request, CurrentUserId, ClientIp, SessionId, ct);
        return ToAction(result);
    }

    /// <summary>Asigna (o reasigna) un técnico a la solicitud.</summary>
    [HttpPut("{id:int}/asignar")]
    [Authorize(Policy = "AdminOSoporte")]
    public async Task<IActionResult> Asignar(int id, [FromBody] AsignarSolicitudRequest request, CancellationToken ct)
    {
        var result = await asignarHandler.HandleAsync(id, request, CurrentUserId, ClientIp, SessionId, ct);
        return ToAction(result);
    }

    /// <summary>Cambio de estatus genérico (ej. Cancelar).</summary>
    [HttpPut("{id:int}/estatus")]
    [Authorize(Policy = "AdminOSoporte")]
    public async Task<IActionResult> ActualizarEstatus(int id, [FromBody] ActualizarEstatusSolicitudRequest request, CancellationToken ct)
    {
        var result = await actualizarEstatusHandler.HandleAsync(id, request, CurrentUserId, ClientIp, SessionId, ct);
        return ToAction(result);
    }

    /// <summary>Concluye la solicitud: calcula tiempo de resolución y determina cumplimiento de SLA.</summary>
    [HttpPut("{id:int}/concluir")]
    [Authorize(Policy = "AdminOSoporte")]
    public async Task<IActionResult> Concluir(int id, [FromBody] ConcluirSolicitudRequest request, CancellationToken ct)
    {
        var result = await concluirHandler.HandleAsync(id, request, CurrentUserId, ClientIp, SessionId, ct);
        return ToAction(result);
    }

    /// <summary>Indicadores para el dashboard: conteo por estatus, tiempo promedio, % SLA.</summary>
    [HttpGet("estadisticas")]
    public async Task<IActionResult> ObtenerEstadisticas(
        [FromQuery] DateTime? fechaDesde, [FromQuery] DateTime? fechaHasta, CancellationToken ct)
    {
        var result = await estadisticasHandler.HandleAsync(fechaDesde, fechaHasta, ct);
        return ToAction(result);
    }
}