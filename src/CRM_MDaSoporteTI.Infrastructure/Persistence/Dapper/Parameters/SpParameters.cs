using Dapper;
using CRM_MDaSoporteTI.Domain.Interfaces.Repositories;

namespace CRM_MDaSoporteTI.Infrastructure.Persistence.Dapper.Parameters;

// Todos los SP retornan al final: SELECT OperationType, [EntidadId], ErrorId, ErrorMessage, OperationDate
public class SpOutputRow
{
    public string? OperationType { get; set; }
    public int? IdSolicitud { get; set; }
    public string? FolioId { get; set; }
    public int? ErrorId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? OperationDate { get; set; }
}

public record SpResult(int? ErrorId, string? ErrorMessage)
{
    public bool IsSuccess => ErrorId is null or 0;
    public bool IsError => !IsSuccess;
}

public static class SolicitudSpParameters
{
    private static void AddAudit(DynamicParameters p, int? userId, string? ip, string? session)
    {
        p.Add("@ExecutedByUserId", userId);
        p.Add("@IpAddress", ip);
        p.Add("@SessionId", session);
    }

    public static DynamicParameters Crear(CrearSolicitudParams r)
    {
        var p = new DynamicParameters();
        p.Add("@OperationType", "insertsolicitud");
        p.Add("@NombreSolicitante", r.NombreSolicitante);
        p.Add("@CorreoSolicitante", r.CorreoSolicitante);
        p.Add("@NombreProyecto", r.NombreProyecto);
        p.Add("@LiderProyecto", r.LiderProyecto);
        p.Add("@CorreoLider", r.CorreoLider);
        p.Add("@FechaInicioPrueba", r.FechaInicioPrueba?.ToDateTime(TimeOnly.MinValue));
        p.Add("@FechaFinPrueba", r.FechaFinPrueba?.ToDateTime(TimeOnly.MinValue));
        p.Add("@TipoMonedero", r.TipoMonedero);
        p.Add("@Ambiente", r.Ambiente);
        p.Add("@CantidadMonederos", r.CantidadMonederos);
        p.Add("@RequiereAfiliacion", r.RequiereAfiliacion);
        p.Add("@NombreAfiliado", r.NombreAfiliado);
        p.Add("@CorreoAfiliado", r.CorreoAfiliado);
        p.Add("@TelefonoAfiliado", r.TelefonoAfiliado);
        p.Add("@CorreoVBO", r.CorreoVBO);
        p.Add("@DescripcionUso", r.DescripcionUso);
        p.Add("@SaldoInicial", r.SaldoInicial);
        p.Add("@EstatusAprobacion", r.EstatusAprobacion);
        p.Add("@JustificacionSinAfiliacion", r.JustificacionSinAfiliacion);
        p.Add("@EsCuentaMonedero", r.EsCuentaMonedero);
        p.Add("@OTP_WHATSAPP", r.OtpWhatsapp);
        p.Add("@MDA_Fisico", r.MdaFisico);
        p.Add("@ReinicioDiario", r.ReinicioDiario);
        p.Add("@ListaSegura", r.ListaSegura);
        AddAudit(p, r.UserId, r.Ip, r.Session);
        return p;
    }

    public static DynamicParameters Actualizar(ActualizarSolicitudParams r)
    {
        var p = new DynamicParameters();
        p.Add("@OperationType", "updatesolicitud");
        p.Add("@IdSolicitud", r.IdSolicitud);
        p.Add("@NombreProyecto", r.NombreProyecto);
        p.Add("@LiderProyecto", r.LiderProyecto);
        p.Add("@CorreoLider", r.CorreoLider);
        p.Add("@FechaInicioPrueba", r.FechaInicioPrueba?.ToDateTime(TimeOnly.MinValue));
        p.Add("@FechaFinPrueba", r.FechaFinPrueba?.ToDateTime(TimeOnly.MinValue));
        p.Add("@CantidadMonederos", r.CantidadMonederos);
        p.Add("@EsCuentaMonedero", r.EsCuentaMonedero);
        p.Add("@OTP_WHATSAPP", r.OtpWhatsapp);
        p.Add("@MDA_Fisico", r.MdaFisico);
        p.Add("@ReinicioDiario", r.ReinicioDiario);
        p.Add("@ListaSegura", r.ListaSegura);
        AddAudit(p, r.UserId, r.Ip, r.Session);
        return p;
    }

    public static DynamicParameters Asignar(AsignarSolicitudParams r)
    {
        var p = new DynamicParameters();
        p.Add("@OperationType", "assignsolicitud");
        p.Add("@IdSolicitud", r.IdSolicitud);
        p.Add("@AsignadoA", r.AsignadoA);
        p.Add("@CorreoAsignadoA", r.CorreoAsignadoA);
        p.Add("@MoverAEnProceso", r.MoverAEnProceso);
        AddAudit(p, r.UserId, r.Ip, r.Session);
        return p;
    }

    public static DynamicParameters ActualizarEstatus(ActualizarEstatusSolicitudParams r)
    {
        var p = new DynamicParameters();
        p.Add("@OperationType", "updatestatussolicitud");
        p.Add("@IdSolicitud", r.IdSolicitud);
        p.Add("@NuevoEstatus", r.NuevoEstatus);
        AddAudit(p, r.UserId, r.Ip, r.Session);
        return p;
    }

    public static DynamicParameters Concluir(ConcluirSolicitudParams r)
    {
        var p = new DynamicParameters();
        p.Add("@OperationType", "concludesolicitud");
        p.Add("@IdSolicitud", r.IdSolicitud);
        p.Add("@SlaMaximoMinutos", r.SlaMaximoMinutos);
        AddAudit(p, r.UserId, r.Ip, r.Session);
        return p;
    }

    public static DynamicParameters Obtener(int idSolicitud, int? userId)
    {
        var p = new DynamicParameters();
        p.Add("@OperationType", "getsolicitudbyid");
        p.Add("@IdSolicitud", idSolicitud);
        p.Add("@ExecutedByUserId", userId);
        return p;
    }

    public static DynamicParameters Listar(ListarSolicitudesParams r)
    {
        var p = new DynamicParameters();
        p.Add("@OperationType", "getsolicitudes");
        p.Add("@FilterEstatus", r.Estatus);
        p.Add("@FilterFechaDesde", r.FechaDesde);
        p.Add("@FilterFechaHasta", r.FechaHasta);
        p.Add("@FilterAsignadoA", r.AsignadoA);
        p.Add("@FilterTipoMonedero", r.TipoMonedero);
        p.Add("@FilterAmbiente", r.Ambiente);
        p.Add("@FilterSearch", r.Search);
        p.Add("@PageNumber", r.PageNumber);
        p.Add("@PageSize", r.PageSize);
        p.Add("@ExecutedByUserId", r.UserId);
        return p;
    }

    public static DynamicParameters Estadisticas(DateTime? desde, DateTime? hasta)
    {
        var p = new DynamicParameters();
        p.Add("@OperationType", "getestadisticas");
        p.Add("@StatsFechaDesde", desde);
        p.Add("@StatsFechaHasta", hasta);
        return p;
    }
}