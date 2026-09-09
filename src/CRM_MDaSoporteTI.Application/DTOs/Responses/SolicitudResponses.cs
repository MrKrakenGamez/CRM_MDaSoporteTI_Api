namespace CRM_MDaSoporteTI.Application.DTOs.Responses;

public record CrearSolicitudResponse(
    int IdSolicitud,
    string FolioId,
    string Message
);

public record SolicitudDetalleResponse(
    int IdSolicitud,
    string FolioId,
    DateTime FechaSolicitud,
    string NombreSolicitante,
    string CorreoSolicitante,
    string NombreProyecto,
    string LiderProyecto,
    string CorreoLider,
    DateTime? FechaInicioPrueba,
    DateTime? FechaFinPrueba,
    string TipoMonedero,
    string Ambiente,
    int CantidadMonederos,
    bool RequiereAfiliacion,
    string? NombreAfiliado,
    string? CorreoAfiliado,
    string? TelefonoAfiliado,
    string? CorreoVBO,
    string? DescripcionUso,
    decimal? SaldoInicial,
    string? EstatusAprobacion,
    string? JustificacionSinAfiliacion,
    string? AsignadoA,
    string? CorreoAsignadoA,
    bool EsCuentaMonedero,
    bool OtpWhatsapp,
    bool MdaFisico,
    bool ReinicioDiario,
    bool ListaSegura,
    int? TiempoTotalResolucion,
    bool? SlaCumplido,
    string Estatus,
    DateTime? FechaAsignacion,
    DateTime? FechaResolucion,
    int DiasTranscurridos
);

public record SolicitudListItemResponse(
    int IdSolicitud,
    string FolioId,
    DateTime FechaSolicitud,
    string NombreSolicitante,
    string CorreoSolicitante,
    string NombreProyecto,
    string Ambiente,
    string TipoMonedero,
    string? AsignadoA,
    string? CorreoAsignadoA,
    string Estatus,
    int? TiempoTotalResolucion,
    bool? SlaCumplido,
    int DiasTranscurridos
);

public record EstadisticasResponse(
    List<ConteoPorEstatusResponse> ConteoPorEstatus,
    double? TiempoPromedioResolucionMinutos,
    int TotalConSLA,
    int TotalSinSLA,
    double PorcentajeCumplimientoSLA
);

public record ConteoPorEstatusResponse(string Estatus, int Cantidad);