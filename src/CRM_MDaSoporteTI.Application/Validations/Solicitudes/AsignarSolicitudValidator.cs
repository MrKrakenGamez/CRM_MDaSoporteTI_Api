using FluentValidation;
using CRM_MDaSoporteTI.Application.DTOs.Requests;

namespace CRM_MDaSoporteTI.Application.Validations.Solicitudes;

public sealed class AsignarSolicitudValidator : AbstractValidator<AsignarSolicitudRequest>
{
    public AsignarSolicitudValidator()
    {
        RuleFor(x => x.AsignadoA).NotEmpty().MaximumLength(150);
        RuleFor(x => x.CorreoAsignadoA).NotEmpty().EmailAddress().MaximumLength(100);
    }
}