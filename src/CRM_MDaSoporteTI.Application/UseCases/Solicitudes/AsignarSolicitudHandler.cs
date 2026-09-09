
using FluentValidation;
using CRM_MDaSoporteTI.Application.DTOs.Requests;
using CRM_MDaSoporteTI.Domain.Interfaces.Repositories;
using CRM_MDaSoporteTI.Shared.Errors;
using CRM_MDaSoporteTI.Shared.Result;

namespace CRM_MDaSoporteTI.Application.UseCases.Solicitudes;

public sealed class AsignarSolicitudHandler(
    ISolicitudRepository repo,
    IValidator<AsignarSolicitudRequest> validator)
{
    public async Task<Result<int>> HandleAsync(
        int idSolicitud, AsignarSolicitudRequest request,
        int? userId, string? ip, string? session, CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result<int>.Failure(new ResultError(
                ErrorCodes.ErrorValidacion, "Uno o más campos son inválidos.", 422,
                validation.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())));

        return await repo.AsignarAsync(new AsignarSolicitudParams(
            idSolicitud, request.AsignadoA, request.CorreoAsignadoA, request.MoverAEnProceso,
            userId, ip, session), ct);

        // TODO (siguiente iteración): disparar IEmailService.NotificarAsignacionAsync(...)
        // una vez que exista el módulo de Infrastructure/Services/EmailService.
    }
}