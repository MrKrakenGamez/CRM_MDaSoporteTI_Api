using FluentValidation;
using CRM_MDaSoporteTI.Application.DTOs.Requests;
using CRM_MDaSoporteTI.Domain.Interfaces.Repositories;
using CRM_MDaSoporteTI.Shared.Errors;
using CRM_MDaSoporteTI.Shared.Result;

namespace CRM_MDaSoporteTI.Application.UseCases.Solicitudes;

public sealed class ActualizarSolicitudHandler(
    ISolicitudRepository repo,
    IValidator<ActualizarSolicitudRequest> validator)
{
    public async Task<Result<int>> HandleAsync(
        int idSolicitud, ActualizarSolicitudRequest request,
        int? userId, string? ip, string? session, CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result<int>.Failure(new ResultError(
                ErrorCodes.ErrorValidacion, "Uno o más campos son inválidos.", 422,
                validation.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())));

        return await repo.ActualizarAsync(new ActualizarSolicitudParams(
            idSolicitud, request.NombreProyecto, request.LiderProyecto, request.CorreoLider,
            request.FechaInicioPrueba, request.FechaFinPrueba, request.CantidadMonederos,
            request.EsCuentaMonedero, request.OtpWhatsapp, request.MdaFisico,
            request.ReinicioDiario, request.ListaSegura,
            userId, ip, session), ct);
    }
}