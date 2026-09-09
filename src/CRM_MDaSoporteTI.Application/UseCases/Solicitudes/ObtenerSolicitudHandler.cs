using CRM_MDaSoporteTI.Application.DTOs.Responses;
using CRM_MDaSoporteTI.Domain.Interfaces.Repositories;
using CRM_MDaSoporteTI.Shared.Errors;
using CRM_MDaSoporteTI.Shared.Result;

namespace CRM_MDaSoporteTI.Application.UseCases.Solicitudes;

public sealed class ObtenerSolicitudHandler(ISolicitudRepository repo)
{
    public async Task<Result<SolicitudDetalleResponse>> HandleAsync(
        int idSolicitud, int? userId, CancellationToken ct = default)
    {
        var result = await repo.ObtenerAsync(idSolicitud, userId, ct);

        if (result.IsFailure)
            return Result<SolicitudDetalleResponse>.Failure(result.Error!);

        if (result.Value is null)
            return Result<SolicitudDetalleResponse>.Failure(
                ErrorCodes.SolicitudNoEncontrada, "La solicitud especificada no existe.", 404);

        var raw = result.Value;

        return Result<SolicitudDetalleResponse>.Success(new SolicitudDetalleResponse(
            raw.IdSolicitud, raw.FolioId, raw.FechaSolicitud,
            raw.NombreSolicitante, raw.CorreoSolicitante,
            raw.NombreProyecto, raw.LiderProyecto, raw.CorreoLider,
            raw.FechaInicioPrueba, raw.FechaFinPrueba,
            raw.TipoMonedero, raw.Ambiente, raw.CantidadMonederos,
            raw.RequiereAfiliacion, raw.NombreAfiliado, raw.CorreoAfiliado, raw.TelefonoAfiliado,
            raw.CorreoVBO, raw.DescripcionUso, raw.SaldoInicial, raw.EstatusAprobacion,
            raw.JustificacionSinAfiliacion, raw.AsignadoA, raw.CorreoAsignadoA,
            raw.EsCuentaMonedero, raw.OTP_WHATSAPP, raw.MDA_Fisico, raw.ReinicioDiario, raw.ListaSegura,
            raw.TiempoTotalResolucion, raw.SLA_Cumplido, raw.Estatus,
            raw.FechaAsignacion, raw.FechaResolucion, raw.DiasTranscurridos));
    }
}