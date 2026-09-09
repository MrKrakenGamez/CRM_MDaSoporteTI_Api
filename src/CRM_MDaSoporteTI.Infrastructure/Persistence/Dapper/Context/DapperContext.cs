using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CRM_MDaSoporteTI.Infrastructure.Configuration;

namespace CRM_MDaSoporteTI.Infrastructure.Persistence.Dapper.Context;

/// <summary>
/// Punto único de acceso a conexiones SQL.
/// Toda operación de base de datos pasa exclusivamente por aquí.
/// Inyectado como Scoped: una conexión por request HTTP.
/// </summary>
public interface IDapperContext
{
    IDbConnection CreateConnection();

    Task<T> ExecuteAsync<T>(
        Func<IDbConnection, Task<T>> operation,
        CancellationToken ct = default);
}

public sealed class DapperContext : IDapperContext
{
    private readonly string _connectionString;
    private readonly ILogger<DapperContext> _log;

    public DapperContext(
        IOptions<ConnectionStringOptions> opts,
        ILogger<DapperContext> log)
    {
        _connectionString = opts.Value.DefaultConnection;
        _log = log;

        if (string.IsNullOrWhiteSpace(_connectionString))
            throw new InvalidOperationException(
                "ConnectionString 'DefaultConnection' no está configurada. " +
                "Revisa appsettings.json o User Secrets.");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<T> ExecuteAsync<T>(
        Func<IDbConnection, Task<T>> operation,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        // IMPORTANTE: 'using' síncrono, NO 'await using'.
        // 'await using' cierra la conexión antes de que el GridReader termine
        // de leer, causando 'The reader has been disposed'.
        using var conn = new SqlConnection(_connectionString);

        try
        {
            await conn.OpenAsync(ct);
            return await operation(conn);
        }
        catch (SqlException ex)
        {
            _log.LogError(ex,
                "SqlException al ejecutar operación. Number={Number} Severity={Severity} State={State}",
                ex.Number, ex.Class, ex.State);
            throw;
        }
        catch (OperationCanceledException)
        {
            _log.LogWarning("Operación de base de datos cancelada por el cliente.");
            throw;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error inesperado en operación de base de datos.");
            throw;
        }
    }
}