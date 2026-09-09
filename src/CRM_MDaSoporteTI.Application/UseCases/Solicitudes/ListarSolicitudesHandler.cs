using FluentValidation;
using CRM_MDaSoporteTI.Application.DTOs.Requests;
using CRM_MDaSoporteTI.Application.DTOs.Responses;
using CRM_MDaSoporteTI.Domain.Interfaces.Repositories;
using CRM_MDaSoporteTI.Shared.Errors;
using CRM_MDaSoporteTI.Shared.Result;

namespace CRM_MDaSoporteTI.Application.UseCases.Solicitudes;

public sealed class ListarSolicitudesHandler(
    ISolicitudRepository repo,
    IValidator<ListarSolicitudesRequest> validator)
{
    public async Task<Result<(IEnumerable<SolicitudListItemResponse> Items, int Total)>> HandleAsync(
        ListarSolicitudesRequest request, int? userId, CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result<(IEnumerable<SolicitudListItemResponse>, int)>.Failure(new ResultError(
                ErrorCodes.ErrorValidacion, "Parámetros de filtro inválidos.", 422,
                validation.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())));

        var result = await repo.ListarAsync(new ListarSolicitudesParams(
            request.Estatus, request.FechaDesde, request.FechaHasta,
            request.AsignadoA, request.TipoMonedero, request.Ambiente, request.Search,
            request.PageNumber, request.PageSize, userId), ct);

        if (result.IsFailure)
            return Result<(IEnumerable<SolicitudListItemResponse>, int)>.Failure(result.Error!);

        var items = result.Value.Items.Select(r => new SolicitudListItemResponse(
            r.IdSolicitud, r.FolioId, r.FechaSolicitud, r.NombreSolicitante, r.CorreoSolicitante,
            r.NombreProyecto, r.Ambiente, r.TipoMonedero, r.AsignadoA, r.CorreoAsignadoA,
            r.Estatus, r.TiempoTotalResolucion, r.SLA_Cumplido, r.DiasTranscurridos));

        return Result<(IEnumerable<SolicitudListItemResponse>, int)>.Success((items, result.Value.Total));
    }
}