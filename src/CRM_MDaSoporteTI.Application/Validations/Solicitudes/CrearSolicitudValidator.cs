using FluentValidation;
using CRM_MDaSoporteTI.Application.DTOs.Requests;

namespace CRM_MDaSoporteTI.Application.Validations.Solicitudes;

public sealed class CrearSolicitudValidator : AbstractValidator<CrearSolicitudRequest>
{
    public CrearSolicitudValidator()
    {
        RuleFor(x => x.NombreSolicitante).NotEmpty().MaximumLength(150);
        RuleFor(x => x.CorreoSolicitante).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.NombreProyecto).NotEmpty().MaximumLength(150);
        RuleFor(x => x.LiderProyecto).NotEmpty().MaximumLength(150);
        RuleFor(x => x.CorreoLider).NotEmpty().EmailAddress().MaximumLength(100);

        RuleFor(x => x.TipoMonedero).NotEmpty().Must(v => v is "Productivo" or "Pruebas")
            .WithMessage("TipoMonedero debe ser 'Productivo' o 'Pruebas'.");

        RuleFor(x => x.Ambiente).NotEmpty().Must(v => v is "UAT" or "Prod")
            .WithMessage("Ambiente debe ser 'UAT' o 'Prod'.");

        RuleFor(x => x.CantidadMonederos).GreaterThan(0);

        RuleFor(x => x)
            .Must(x => x.FechaInicioPrueba is null || x.FechaFinPrueba is null || x.FechaInicioPrueba < x.FechaFinPrueba)
            .WithMessage("FechaInicioPrueba debe ser menor a FechaFinPrueba.")
            .WithName("FechaInicioPrueba");

        RuleFor(x => x.CantidadMonederos)
            .LessThanOrEqualTo(6)
            .When(x => x.RequiereAfiliacion)
            .WithMessage("CantidadMonederos no puede exceder 6 cuando la solicitud requiere afiliación.");

        When(x => x.RequiereAfiliacion, () =>
        {
            RuleFor(x => x.NombreAfiliado).NotEmpty().WithMessage("NombreAfiliado es obligatorio cuando RequiereAfiliacion = true.");
            RuleFor(x => x.CorreoAfiliado).NotEmpty().EmailAddress().WithMessage("CorreoAfiliado es obligatorio y debe ser válido cuando RequiereAfiliacion = true.");
        });

        When(x => !x.RequiereAfiliacion, () =>
        {
            RuleFor(x => x.JustificacionSinAfiliacion).NotEmpty()
                .WithMessage("JustificacionSinAfiliacion es obligatoria cuando RequiereAfiliacion = false.");
        });
    }
}