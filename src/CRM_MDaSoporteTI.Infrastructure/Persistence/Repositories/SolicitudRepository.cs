using System.Data;
using Dapper;
using CRM_MDaSoporteTI.Domain.Interfaces.Repositories;
using CRM_MDaSoporteTI.Infrastructure.Persistence.Dapper.Context;
using CRM_MDaSoporteTI.Infrastructure.Persistence.Dapper.Parameters;
using CRM_MDaSoporteTI.Shared.Constants;
using CRM_MDaSoporteTI.Shared.Errors;
using CRM_MDaSoporteTI.Shared.Result;
using Microsoft.Extensions.Logging;

namespace CRM_MDaSoporteTI.Infrastructure.Persistence.Repositories;

// ─────────────────────────────────────────────────────────────────────────────
// SpHelper
//
// COMPORTAMIENTO REAL DE LOS SP MAESTROS (sp_Solicitudes / sp_Actividades / sp_Todos):
//
// CAMINO EXITOSO en operaciones de escritura:
//   A veces solo hay 1 RS: el SELECT final (EndProcedure) con ErrorId=NULL.
//   (A diferencia del gym, nuestros SP NO regresan una fila de "datos" separada
//    en insert/update/assign/etc. — todo viaja en el mismo SELECT final,
//    incluyendo IdSolicitud/FolioId cuando aplica.)
//
// CAMINO DE ERROR (GOTO EndProcedure anticipado):
//   También 1 RS: el mismo SELECT final, pero con ErrorId > 0.
//
// Por eso aquí el helper es más simple que en el gym: basta con leer
// el ÚLTIMO/ÚNICO result set como SpOutputRow.
// Para operaciones con múltiples RS (getsolicitudbyid con auditoría),
// se maneja explícitamente en el método correspondiente.
// ─────────────────────────────────────────────────────────────────────────────
file static class SpHelper
{
    public static async Task<SpOutputRow?> TryReadOutputAsync(SqlMapper.GridReader multi)
    {
        try
        {
            if (multi.IsConsumed) return null;
            return await multi.ReadFirstOrDefaultAsync<SpOutputRow>();
        }
        catch
        {
            return null;
        }
    }

    public static (int? ErrorId, string? ErrorMsg) GetError(SpOutputRow? output)
    {
        if (output is null) return (null, null);
        var eid = output.ErrorId is null or 0 ? null : output.ErrorId;
        return (eid, output.ErrorMessage);
    }
}

public sealed class SolicitudRepository(
    IDapperContext db,
    ILogger<SolicitudRepository> log) : ISolicitudRepository
{
    public async Task<Result<(int IdSolicitud, string FolioId)>> CrearAsync(
        CrearSolicitudParams p, CancellationToken ct = default) =>
        await db.ExecuteAsync(async conn =>
        {
            var prm = SolicitudSpParameters.Crear(p);
            using var multi = await conn.QueryMultipleAsync(
                StoredProcedures.Solicitudes, prm, commandType: CommandType.StoredProcedure);

            var output = await SpHelper.TryReadOutputAsync(multi);
            var (eid, emsg) = SpHelper.GetError(output);

            if (eid.HasValue)
            {
                log.LogWarning("CrearSolicitud ErrorId={E} Msg={M}", eid, emsg);
                return Result<(int, string)>.Failure(
                    SpErrorMapper.ToResultError(eid.Value, emsg ?? "Error al crear la solicitud."));
            }

            log.LogInformation("Solicitud creada FolioId={Folio}", output?.FolioId);
            return Result<(int, string)>.Success((output?.IdSolicitud ?? 0, output?.FolioId ?? ""));
        }, ct);

    public async Task<Result<int>> ActualizarAsync(
        ActualizarSolicitudParams p, CancellationToken ct = default) =>
        await db.ExecuteAsync(async conn =>
        {
            var prm = SolicitudSpParameters.Actualizar(p);
            using var multi = await conn.QueryMultipleAsync(
                StoredProcedures.Solicitudes, prm, commandType: CommandType.StoredProcedure);

            var output = await SpHelper.TryReadOutputAsync(multi);
            var (eid, emsg) = SpHelper.GetError(output);

            if (eid.HasValue)
                return Result<int>.Failure(SpErrorMapper.ToResultError(eid.Value, emsg ?? "Error al actualizar la solicitud."));

            return Result<int>.Success(p.IdSolicitud);
        }, ct);

    public async Task<Result<int>> AsignarAsync(
        AsignarSolicitudParams p, CancellationToken ct = default) =>
        await db.ExecuteAsync(async conn =>
        {
            var prm = SolicitudSpParameters.Asignar(p);
            using var multi = await conn.QueryMultipleAsync(
                StoredProcedures.Solicitudes, prm, commandType: CommandType.StoredProcedure);

            var output = await SpHelper.TryReadOutputAsync(multi);
            var (eid, emsg) = SpHelper.GetError(output);

            if (eid.HasValue)
                return Result<int>.Failure(SpErrorMapper.ToResultError(eid.Value, emsg ?? "Error al asignar la solicitud."));

            log.LogInformation("Solicitud {Id} asignada a {Asignado}", p.IdSolicitud, p.AsignadoA);
            return Result<int>.Success(p.IdSolicitud);
        }, ct);

    public async Task<Result<int>> ActualizarEstatusAsync(
        ActualizarEstatusSolicitudParams p, CancellationToken ct = default) =>
        await db.ExecuteAsync(async conn =>
        {
            var prm = SolicitudSpParameters.ActualizarEstatus(p);
            using var multi = await conn.QueryMultipleAsync(
                StoredProcedures.Solicitudes, prm, commandType: CommandType.StoredProcedure);

            var output = await SpHelper.TryReadOutputAsync(multi);
            var (eid, emsg) = SpHelper.GetError(output);

            if (eid.HasValue)
                return Result<int>.Failure(SpErrorMapper.ToResultError(eid.Value, emsg ?? "Error al cambiar el estatus."));

            return Result<int>.Success(p.IdSolicitud);
        }, ct);

    public async Task<Result<int>> ConcluirAsync(
        ConcluirSolicitudParams p, CancellationToken ct = default) =>
        await db.ExecuteAsync(async conn =>
        {
            var prm = SolicitudSpParameters.Concluir(p);
            using var multi = await conn.QueryMultipleAsync(
                StoredProcedures.Solicitudes, prm, commandType: CommandType.StoredProcedure);

            var output = await SpHelper.TryReadOutputAsync(multi);
            var (eid, emsg) = SpHelper.GetError(output);

            if (eid.HasValue)
                return Result<int>.Failure(SpErrorMapper.ToResultError(eid.Value, emsg ?? "Error al concluir la solicitud."));

            log.LogInformation("Solicitud {Id} concluida.", p.IdSolicitud);
            return Result<int>.Success(p.IdSolicitud);
        }, ct);

    // getsolicitudbyid: RS1=detalle | RS2=auditoría | RS3=output
    public async Task<Result<SolicitudDetalleRaw?>> ObtenerAsync(
        int idSolicitud, int? userId, CancellationToken ct = default) =>
        await db.ExecuteAsync(async conn =>
        {
            var prm = SolicitudSpParameters.Obtener(idSolicitud, userId);
            using var multi = await conn.QueryMultipleAsync(
                StoredProcedures.Solicitudes, prm, commandType: CommandType.StoredProcedure);

            // Leer primer RS como dynamic: puede ser el detalle O el output de error
            var firstRows = (await multi.ReadAsync<dynamic>()).ToList();
            var first = firstRows.FirstOrDefault() as IDictionary<string, object>;

            if (first is not null && first.ContainsKey("ErrorId"))
            {
                var eid = first.TryGetValue("ErrorId", out var e) && e is not null ? Convert.ToInt32(e) : (int?)null;
                var emsg = first.TryGetValue("ErrorMessage", out var m) ? m?.ToString() : null;

                if (eid is > 0)
                    return Result<SolicitudDetalleRaw?>.Failure(
                        SpErrorMapper.ToResultError(eid.Value, emsg ?? "Solicitud no encontrada."));

                return Result<SolicitudDetalleRaw?>.Success(null);
            }

            // Camino exitoso: consumir RS2 (auditoría) y RS3 (output)
            _ = (await multi.ReadAsync<SolicitudAuditoriaRaw>()).ToList();
            _ = await SpHelper.TryReadOutputAsync(multi);

            if (first is null) return Result<SolicitudDetalleRaw?>.Success(null);

            return Result<SolicitudDetalleRaw?>.Success(MapDetalle(first));
        }, ct);

    // getsolicitudes: 1 solo RS con filas + TotalRecords via COUNT(*) OVER()
    public async Task<Result<(IEnumerable<SolicitudListaRaw> Items, int Total)>> ListarAsync(
        ListarSolicitudesParams p, CancellationToken ct = default) =>
        await db.ExecuteAsync(async conn =>
        {
            var prm = SolicitudSpParameters.Listar(p);
            var rows = (await conn.QueryAsync<SolicitudListaRaw>(
                StoredProcedures.Solicitudes, prm,
                commandType: CommandType.StoredProcedure)).ToList();

            var total = rows.FirstOrDefault()?.TotalRecords ?? 0;
            return Result<(IEnumerable<SolicitudListaRaw>, int)>.Success((rows, total));
        }, ct);

    // getestadisticas: RS1=conteo por estatus | RS2=métricas de resolución
    public async Task<Result<EstadisticasSolicitudesRaw>> ObtenerEstadisticasAsync(
        DateTime? desde, DateTime? hasta, CancellationToken ct = default) =>
        await db.ExecuteAsync(async conn =>
        {
            var prm = SolicitudSpParameters.Estadisticas(desde, hasta);
            using var multi = await conn.QueryMultipleAsync(
                StoredProcedures.Solicitudes, prm, commandType: CommandType.StoredProcedure);

            var conteo = (await multi.ReadAsync<ConteoPorEstatusRaw>()).ToList();
            var metrica = await multi.ReadFirstOrDefaultAsync<dynamic>();

            var result = new EstadisticasSolicitudesRaw { ConteoPorEstatus = conteo };

            if (metrica is not null)
            {
                var d = (IDictionary<string, object>)metrica;
                result.TiempoPromedioResolucionMinutos = d.TryGetValue("TiempoPromedioResolucionMinutos", out var t) && t is not null ? Convert.ToDouble(t) : null;
                result.TotalConSLA = d.TryGetValue("TotalConSLA", out var c1) && c1 is not null ? Convert.ToInt32(c1) : 0;
                result.TotalSinSLA = d.TryGetValue("TotalSinSLA", out var c2) && c2 is not null ? Convert.ToInt32(c2) : 0;
                result.PorcentajeCumplimientoSLA = d.TryGetValue("PorcentajeCumplimientoSLA", out var pc) && pc is not null ? Convert.ToDouble(pc) : 0;
            }

            return Result<EstadisticasSolicitudesRaw>.Success(result);
        }, ct);

    private static SolicitudDetalleRaw MapDetalle(IDictionary<string, object> r)
    {
        T? Get<T>(string key, T? def = default) =>
            r.TryGetValue(key, out var v) && v is not null ? (T)Convert.ChangeType(v, typeof(T)) : def;

        string? GetStr(string key) => r.TryGetValue(key, out var v) && v is not null ? v.ToString() : null;

        return new SolicitudDetalleRaw
        {
            IdSolicitud = Get<int>("IdSolicitud"),
            FolioId = GetStr("FolioId") ?? "",
            FechaSolicitud = Get<DateTime>("FechaSolicitud"),
            NombreSolicitante = GetStr("NombreSolicitante") ?? "",
            CorreoSolicitante = GetStr("CorreoSolicitante") ?? "",
            NombreProyecto = GetStr("NombreProyecto") ?? "",
            LiderProyecto = GetStr("LiderProyecto") ?? "",
            CorreoLider = GetStr("CorreoLider") ?? "",
            FechaInicioPrueba = r.TryGetValue("FechaInicioPrueba", out var fi) && fi is not null ? Convert.ToDateTime(fi) : null,
            FechaFinPrueba = r.TryGetValue("FechaFinPrueba", out var ff) && ff is not null ? Convert.ToDateTime(ff) : null,
            TipoMonedero = GetStr("TipoMonedero") ?? "",
            Ambiente = GetStr("Ambiente") ?? "",
            CantidadMonederos = Get<int>("CantidadMonederos"),
            RequiereAfiliacion = Get<bool>("RequiereAfiliacion"),
            NombreAfiliado = GetStr("NombreAfiliado"),
            CorreoAfiliado = GetStr("CorreoAfiliado"),
            TelefonoAfiliado = GetStr("TelefonoAfiliado"),
            CorreoVBO = GetStr("CorreoVBO"),
            DescripcionUso = GetStr("DescripcionUso"),
            SaldoInicial = r.TryGetValue("SaldoInicial", out var si) && si is not null ? Convert.ToDecimal(si) : null,
            EstatusAprobacion = GetStr("EstatusAprobacion"),
            JustificacionSinAfiliacion = GetStr("JustificacionSinAfiliacion"),
            AsignadoA = GetStr("AsignadoA"),
            CorreoAsignadoA = GetStr("CorreoAsignadoA"),
            EsCuentaMonedero = Get<bool>("EsCuentaMonedero"),
            OTP_WHATSAPP = Get<bool>("OTP_WHATSAPP"),
            MDA_Fisico = Get<bool>("MDA_Fisico"),
            ReinicioDiario = Get<bool>("ReinicioDiario"),
            ListaSegura = Get<bool>("ListaSegura"),
            TiempoTotalResolucion = r.TryGetValue("TiempoTotalResolucion", out var tr) && tr is not null ? Convert.ToInt32(tr) : null,
            SLA_Cumplido = r.TryGetValue("SLA_Cumplido", out var sla) && sla is not null ? Convert.ToBoolean(sla) : null,
            idEstatus = Get<int>("idEstatus"),
            Estatus = GetStr("Estatus") ?? "",
            FechaAsignacion = r.TryGetValue("FechaAsignacion", out var fa) && fa is not null ? Convert.ToDateTime(fa) : null,
            FechaResolucion = r.TryGetValue("FechaResolucion", out var fr) && fr is not null ? Convert.ToDateTime(fr) : null,
            DiasTranscurridos = Get<int>("DiasTranscurridos"),
        };
    }
}