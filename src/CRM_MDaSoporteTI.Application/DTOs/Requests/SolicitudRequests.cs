namespace CRM_MDaSoporteTI.Application.DTOs.Requests;

public record CrearSolicitudRequest(
    string NombreSolicitante,
    string CorreoSolicitante,
    string NombreProyecto,
    string LiderProyecto,
    string CorreoLider,
    DateOnly? FechaInicioPrueba,
    DateOnly? FechaFinPrueba,
    string TipoMonedero,          // 'Productivo' | 'Pruebas'
    string Ambiente,              // 'UAT' | 'Prod'
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
    bool EsCuentaMonedero,
    bool OtpWhatsapp,
    bool MdaFisico,
    bool ReinicioDiario,
    bool ListaSegura
);

public record ActualizarSolicitudRequest(
    string? NombreProyecto,
    string? LiderProyecto,
    string? CorreoLider,
    DateOnly? FechaInicioPrueba,
    DateOnly? FechaFinPrueba,
    int? CantidadMonederos,
    bool? EsCuentaMonedero,
    bool? OtpWhatsapp,
    bool? MdaFisico,
    bool? ReinicioDiario,
    bool? ListaSegura
);

public record AsignarSolicitudRequest(
    string AsignadoA,
    string CorreoAsignadoA,
    bool MoverAEnProceso = true
);

public record ActualizarEstatusSolicitudRequest(
    string NuevoEstatus  // 'Cancelada' u otro estatus válido del catálogo
);

public record ConcluirSolicitudRequest(
    int? SlaMaximoMinutos  // opcional: si viene null, el handler usa el default de configuración
);

public record ListarSolicitudesRequest(
    string? Estatus,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    string? AsignadoA,
    string? TipoMonedero,
    string? Ambiente,
    string? Search,
    int PageNumber = 1,
    int PageSize = 20
);