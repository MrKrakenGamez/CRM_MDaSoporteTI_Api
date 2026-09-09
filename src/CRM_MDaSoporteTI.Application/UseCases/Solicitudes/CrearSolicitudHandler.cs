using FluentValidation;
using CRM_MDaSoporteTI.Application.DTOs.Requests;
using CRM_MDaSoporteTI.Application.DTOs.Responses;
using CRM_MDaSoporteTI.Domain.Interfaces.Repositories;
using CRM_MDaSoporteTI.Shared.Errors;
using CRM_MDaSoporteTI.Shared.Result;

namespace CRM_MDaSoporteTI.Application.UseCases.Solicitudes;

public sealed class CrearSolicitudHandler(
    ISolicitudRepository repo,
    IValidator<CrearSolicitudRequest> validator)
{
    public async Task<Result<CrearSolicitudResponse>> HandleAsync(
        CrearSolicitudRequest request, int? userId, string? ip, string? session,
        CancellationToken ct = default)
    {
        var validation = await validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result<CrearSolicitudResponse>.Failure(BuildValidationError(validation));

        var result = await repo.CrearAsync(new CrearSolicitudParams(
            request.NombreSolicitante, request.CorreoSolicitante,
            request.NombreProyecto, request.LiderProyecto, request.CorreoLider,
            request.FechaInicioPrueba, request.FechaFinPrueba,
            request.TipoMonedero, request.Ambiente, request.CantidadMonederos,
            request.RequiereAfiliacion,
            request.NombreAfiliado, request.CorreoAfiliado, request.TelefonoAfiliado,
            request.CorreoVBO, request.DescripcionUso, request.SaldoInicial, request.EstatusAprobacion,
            request.JustificacionSinAfiliacion,
            request.EsCuentaMonedero, request.OtpWhatsapp, request.MdaFisico,
            request.ReinicioDiario, request.ListaSegura,
            userId, ip, session), ct);

        if (result.IsFailure)
            return Result<CrearSolicitudResponse>.Failure(result.Error!);

        var (idSolicitud, folioId) = result.Value;

        return Result<CrearSolicitudResponse>.Success(new CrearSolicitudResponse(
            idSolicitud, folioId,
            $"Solicitud registrada correctamente. Tu folio es {folioId}."));
    }

    private static ResultError BuildValidationError(FluentValidation.Results.ValidationResult validation)
    {
        var details = validation.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return new ResultError(ErrorCodes.ErrorValidacion, "Uno o más campos son inválidos.", 422, details);
    }
}