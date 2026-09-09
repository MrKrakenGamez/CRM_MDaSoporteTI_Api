using FluentValidation;
using CRM_MDaSoporteTI.Application.DTOs.Requests;

namespace CRM_MDaSoporteTI.Application.Validations.Solicitudes;

public sealed class ListarSolicitudesValidator : AbstractValidator<ListarSolicitudesRequest>
{
    public ListarSolicitudesValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

        RuleFor(x => x)
            .Must(x => x.FechaDesde is null || x.FechaHasta is null || x.FechaDesde <= x.FechaHasta)
            .WithMessage("FechaDesde no puede ser mayor a FechaHasta.")
            .WithName("FechaDesde");
    }
}