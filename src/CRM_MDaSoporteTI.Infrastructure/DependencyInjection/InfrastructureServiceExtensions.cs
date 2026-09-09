using CRM_MDaSoporteTI.Domain.Interfaces.Repositories;
using CRM_MDaSoporteTI.Infrastructure.Configuration;
using CRM_MDaSoporteTI.Infrastructure.Persistence.Dapper.Context;
using CRM_MDaSoporteTI.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM_MDaSoporteTI.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        // Options
        services.Configure<ConnectionStringOptions>(
            config.GetSection(ConnectionStringOptions.SectionName));
        services.Configure<AuthOptions>(
            config.GetSection(AuthOptions.SectionName));
        services.Configure<EmailOptions>(
            config.GetSection(EmailOptions.SectionName));
        services.Configure<SlaOptions>(
            config.GetSection(SlaOptions.SectionName));

        // Dapper — un IDapperContext por request (Scoped)
        services.AddScoped<IDapperContext, DapperContext>();

        // Repositorios
        services.AddScoped<ISolicitudRepository, SolicitudRepository>();
        // services.AddScoped<IActividadRepository, ActividadRepository>();
        // services.AddScoped<ITodoRepository, TodoRepository>();

        return services;
    }
}