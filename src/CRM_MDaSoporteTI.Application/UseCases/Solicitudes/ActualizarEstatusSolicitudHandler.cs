using FluentValidation;
using CRM_MDaSoporteTI.Application.DTOs.Requests;
using CRM_MDaSoporteTI.Domain.Interfaces.Repositories;
using CRM_MDaSoporteTI.Shared.Errors;
using CRM_MDaSoporteTI.Shared.Result;

namespace CRM_MDaSoporteTI.Application.UseCases.Solicitudes;

public sealed class ActualizarEstatusSolicitudHandler(
    ISolicitudRepository repo,
    IValidator<ActualizarEstatusSolicitudRequest> validator)
{
    public async Task<Result<int>> HandleAsync(
        int idSolicitud, ActualizarEstatusSolicitudRequest request,
        int? userId, string? ip, string? session, CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result<int>.Failure(new ResultError(
                ErrorCodes.ErrorValidacion, "Uno o más campos son inválidos.", 422,
                validation.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())));

        return await repo.ActualizarEstatusAsync(new ActualizarEstatusSolicitudParams(
            idSolicitud, request.NuevoEstatus, userId, ip, session), ct);
    }
}