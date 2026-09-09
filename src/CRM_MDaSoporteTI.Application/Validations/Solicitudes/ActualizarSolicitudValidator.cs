using FluentValidation;
using CRM_MDaSoporteTI.Application.DTOs.Requests;

namespace CRM_MDaSoporteTI.Application.Validations.Solicitudes;

public sealed class ActualizarSolicitudValidator : AbstractValidator<ActualizarSolicitudRequest>
{
    public ActualizarSolicitudValidator()
    {
        RuleFor(x => x)
            .Must(x => x.FechaInicioPrueba is null || x.FechaFinPrueba is null || x.FechaInicioPrueba < x.FechaFinPrueba)
            .WithMessage("FechaInicioPrueba debe ser menor a FechaFinPrueba.")
            .WithName("FechaInicioPrueba");

        RuleFor(x => x.CantidadMonederos).GreaterThan(0).When(x => x.CantidadMonederos.HasValue);
    }
}