using CRM_MDaSoporteTI.Application.DTOs.Requests;
using CRM_MDaSoporteTI.Domain.Interfaces.Repositories;
using CRM_MDaSoporteTI.Infrastructure.Configuration;
using CRM_MDaSoporteTI.Shared.Result;
using Microsoft.Extensions.Options;

namespace CRM_MDaSoporteTI.Application.UseCases.Solicitudes;

public sealed class ConcluirSolicitudHandler(
    ISolicitudRepository repo,
    IOptions<SlaOptions> slaOptions)
{
    public async Task<Result<int>> HandleAsync(
        int idSolicitud, ConcluirSolicitudRequest request,
        int? userId, string? ip, string? session, CancellationToken ct = default)
    {
        var slaMax = request.SlaMaximoMinutos ?? slaOptions.Value.DefaultMaxMinutes;

        return await repo.ConcluirAsync(new ConcluirSolicitudParams(
            idSolicitud, slaMax, userId, ip, session), ct);

        // TODO: IEmailService.NotificarConclusionAsync(...) al solicitante.
    }
}