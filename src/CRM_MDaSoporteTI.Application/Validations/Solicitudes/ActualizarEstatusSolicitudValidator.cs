using FluentValidation;
using CRM_MDaSoporteTI.Application.DTOs.Requests;

namespace CRM_MDaSoporteTI.Application.Validations.Solicitudes;

public sealed class ActualizarEstatusSolicitudValidator : AbstractValidator<ActualizarEstatusSolicitudRequest>
{
    private static readonly string[] ValoresPermitidos = ["Pendiente", "EnProceso", "Resuelta", "Cancelada"];

    public ActualizarEstatusSolicitudValidator()
    {
        RuleFor(x => x.NuevoEstatus)
            .NotEmpty()
            .Must(v => ValoresPermitidos.Contains(v))
            .WithMessage($"NuevoEstatus debe ser uno de: {string.Join(", ", ValoresPermitidos)}.");
    }
}