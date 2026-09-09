using System.Reflection;
using FluentValidation;
using CRM_MDaSoporteTI.Application.UseCases.Solicitudes;
using Microsoft.Extensions.DependencyInjection;

namespace CRM_MDaSoporteTI.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);

        // UseCases — Solicitudes
        services.AddScoped<CrearSolicitudHandler>();
        services.AddScoped<ActualizarSolicitudHandler>();
        services.AddScoped<AsignarSolicitudHandler>();
        services.AddScoped<ActualizarEstatusSolicitudHandler>();
        services.AddScoped<ConcluirSolicitudHandler>();
        services.AddScoped<ObtenerSolicitudHandler>();
        services.AddScoped<ListarSolicitudesHandler>();
        services.AddScoped<ObtenerEstadisticasHandler>();

        return services;
    }
}