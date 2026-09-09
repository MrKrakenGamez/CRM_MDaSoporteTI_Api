using CRM_MDaSoporteTI.Shared.Result;


namespace CRM_MDaSoporteTI.Domain.Interfaces.Repositories;
public interface ISolicitudRepository
{
    Task<Result<(int IdSolicitud, string FolioId)>> CrearAsync(CrearSolicitudParams p, CancellationToken ct = default);
    Task<Result<int>> ActualizarAsync(ActualizarSolicitudParams p, CancellationToken ct = default);
    Task<Result<int>> AsignarAsync(AsignarSolicitudParams p, CancellationToken ct = default);
    Task<Result<int>> ActualizarEstatusAsync(ActualizarEstatusSolicitudParams p, CancellationToken ct = default);
    Task<Result<int>> ConcluirAsync(ConcluirSolicitudParams p, CancellationToken ct = default);
    Task<Result<SolicitudDetalleRaw?>> ObtenerAsync(int idSolicitud, int? userId, CancellationToken ct = default);
    Task<Result<(IEnumerable<SolicitudListaRaw> Items, int Total)>> ListarAsync(ListarSolicitudesParams p, CancellationToken ct = default);
    Task<Result<EstadisticasSolicitudesRaw>> ObtenerEstadisticasAsync(DateTime? desde, DateTime? hasta, CancellationToken ct = default);
}

// ─────────────────────────────────────────────────────────────
// Parameter Records
// ─────────────────────────────────────────────────────────────

public record CrearSolicitudParams(
    string NombreSolicitante, string CorreoSolicitante,
    string NombreProyecto, string LiderProyecto, string CorreoLider,
    DateOnly? FechaInicioPrueba, DateOnly? FechaFinPrueba,
    string TipoMonedero, string Ambiente, int CantidadMonederos,
    bool RequiereAfiliacion,
    string? NombreAfiliado, string? CorreoAfiliado, string? TelefonoAfiliado,
    string? CorreoVBO, string? DescripcionUso, decimal? SaldoInicial, string? EstatusAprobacion,
    string? JustificacionSinAfiliacion,
    bool EsCuentaMonedero, bool OtpWhatsapp, bool MdaFisico, bool ReinicioDiario, bool ListaSegura,
    int? UserId, string? Ip, string? Session);

public record ActualizarSolicitudParams(
    int IdSolicitud,
    string? NombreProyecto, string? LiderProyecto, string? CorreoLider,
    DateOnly? FechaInicioPrueba, DateOnly? FechaFinPrueba,
    int? CantidadMonederos,
    bool? EsCuentaMonedero, bool? OtpWhatsapp, bool? MdaFisico, bool? ReinicioDiario, bool? ListaSegura,
    int? UserId, string? Ip, string? Session);

public record AsignarSolicitudParams(
    int IdSolicitud, string AsignadoA, string CorreoAsignadoA, bool MoverAEnProceso,
    int? UserId, string? Ip, string? Session);

public record ActualizarEstatusSolicitudParams(
    int IdSolicitud, string NuevoEstatus,
    int? UserId, string? Ip, string? Session);

public record ConcluirSolicitudParams(
    int IdSolicitud, int SlaMaximoMinutos,
    int? UserId, string? Ip, string? Session);

public record ListarSolicitudesParams(
    string? Estatus, DateTime? FechaDesde, DateTime? FechaHasta,
    string? AsignadoA, string? TipoMonedero, string? Ambiente, string? Search,
    int PageNumber, int PageSize, int? UserId);

// ─────────────────────────────────────────────────────────────
// Raw Data Models
// Todos los campos que el SP puede devolver NULL deben ser nullable aquí:
// Dapper solo mapea NULL de SQL a null de C# si el tipo lo permite.
// ─────────────────────────────────────────────────────────────

public class SolicitudDetalleRaw
{
    public int IdSolicitud { get; set; }
    public string FolioId { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public string NombreSolicitante { get; set; } = string.Empty;
    public string CorreoSolicitante { get; set; } = string.Empty;
    public string NombreProyecto { get; set; } = string.Empty;
    public string LiderProyecto { get; set; } = string.Empty;
    public string CorreoLider { get; set; } = string.Empty;
    public DateTime? FechaInicioPrueba { get; set; }
    public DateTime? FechaFinPrueba { get; set; }
    public string TipoMonedero { get; set; } = string.Empty;
    public string Ambiente { get; set; } = string.Empty;
    public int CantidadMonederos { get; set; }
    public bool RequiereAfiliacion { get; set; }
    public string? NombreAfiliado { get; set; }
    public string? CorreoAfiliado { get; set; }
    public string? TelefonoAfiliado { get; set; }
    public string? CorreoVBO { get; set; }
    public string? DescripcionUso { get; set; }
    public decimal? SaldoInicial { get; set; }
    public string? EstatusAprobacion { get; set; }
    public string? JustificacionSinAfiliacion { get; set; }
    public string? AsignadoA { get; set; }
    public string? CorreoAsignadoA { get; set; }
    public bool EsCuentaMonedero { get; set; }
    public bool OTP_WHATSAPP { get; set; }
    public bool MDA_Fisico { get; set; }
    public bool ReinicioDiario { get; set; }
    public bool ListaSegura { get; set; }
    public int? TiempoTotalResolucion { get; set; }
    public bool? SLA_Cumplido { get; set; }
    public int idEstatus { get; set; }
    public string Estatus { get; set; } = string.Empty;
    public DateTime? FechaAsignacion { get; set; }
    public DateTime? FechaResolucion { get; set; }
    public int DiasTranscurridos { get; set; }
}

public class SolicitudAuditoriaRaw
{
    public int AuditId { get; set; }
    public string OperationType { get; set; } = string.Empty;
    public string? FieldChanged { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public int? ChangedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SolicitudListaRaw
{
    public int IdSolicitud { get; set; }
    public string FolioId { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public string NombreSolicitante { get; set; } = string.Empty;
    public string CorreoSolicitante { get; set; } = string.Empty;
    public string NombreProyecto { get; set; } = string.Empty;
    public string Ambiente { get; set; } = string.Empty;
    public string TipoMonedero { get; set; } = string.Empty;
    public string? AsignadoA { get; set; }
    public string? CorreoAsignadoA { get; set; }
    public string Estatus { get; set; } = string.Empty;
    public int idEstatus { get; set; }
    public int? TiempoTotalResolucion { get; set; }
    public bool? SLA_Cumplido { get; set; }
    public int DiasTranscurridos { get; set; }
    public int TotalRecords { get; set; }
}

public class EstadisticasSolicitudesRaw
{
    public List<ConteoPorEstatusRaw> ConteoPorEstatus { get; set; } = [];
    public double? TiempoPromedioResolucionMinutos { get; set; }
    public int TotalConSLA { get; set; }
    public int TotalSinSLA { get; set; }
    public double PorcentajeCumplimientoSLA { get; set; }
}

public class ConteoPorEstatusRaw
{
    public string Estatus { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}