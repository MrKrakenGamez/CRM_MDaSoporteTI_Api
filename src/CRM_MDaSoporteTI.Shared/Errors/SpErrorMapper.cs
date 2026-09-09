using CRM_MDaSoporteTI.Shared.Result;


// ============================================================
// Traduce el ErrorId numérico devuelto por los SP (tabla ErrorHandling)
// a un ResultError de aplicación con código y HTTP status.
// ============================================================


namespace CRM_MDaSoporteTI.Shared.Errors;

public static class SpErrorMapper
{
    // Debe reflejar 1:1 el catálogo ErrorHandling de la base de datos.
    public static ResultError ToResultError(int errorId, string message) => errorId switch
    {
        1 => new ResultError(ErrorCodes.SolicitudDuplicada, message, 409),
        2 => new ResultError(ErrorCodes.SolicitudDatosFaltantes, message, 400),
        3 => new ResultError(ErrorCodes.ErrorInesperado, message, 500),
        4 => new ResultError(ErrorCodes.SolicitudNoEncontrada, message, 404),
        5 => new ResultError(ErrorCodes.SolicitudReglaNegocio, message, 422),
        6 => new ResultError(ErrorCodes.ErrorInesperado, message, 409), // lock timeout
        7 => new ResultError(ErrorCodes.SolicitudSinAsignar, message, 422),
        8 => new ResultError(ErrorCodes.SolicitudReferenciaInvalida, message, 400),
        9 => new ResultError(ErrorCodes.SolicitudReglaNegocio, message, 409),
        11 => new ResultError(ErrorCodes.ErrorInesperado, message, 400), // OperationType inválido
        _ => new ResultError(ErrorCodes.ErrorInesperado, message, 500),
    };
}