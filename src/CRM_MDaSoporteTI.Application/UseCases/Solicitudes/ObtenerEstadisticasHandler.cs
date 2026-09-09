using CRM_MDaSoporteTI.Application.DTOs.Responses;
using CRM_MDaSoporteTI.Domain.Interfaces.Repositories;
using CRM_MDaSoporteTI.Shared.Result;

namespace CRM_MDaSoporteTI.Application.UseCases.Solicitudes;

public sealed class ObtenerEstadisticasHandler(ISolicitudRepository repo)
{
    public async Task<Result<EstadisticasResponse>> HandleAsync(
        DateTime? desde, DateTime? hasta, CancellationToken ct = default)
    {
        var result = await repo.ObtenerEstadisticasAsync(desde, hasta, ct);

        if (result.IsFailure)
            return Result<EstadisticasResponse>.Failure(result.Error!);

        var raw = result.Value;

        return Result<EstadisticasResponse>.Success(new EstadisticasResponse(
            raw.ConteoPorEstatus.Select(c => new ConteoPorEstatusResponse(c.Estatus, c.Cantidad)).ToList(),
            raw.TiempoPromedioResolucionMinutos, raw.TotalConSLA, raw.TotalSinSLA,
            raw.PorcentajeCumplimientoSLA));
    }
}