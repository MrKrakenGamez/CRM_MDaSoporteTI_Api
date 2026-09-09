// ============================================================
// Códigos de error de aplicación, agrupados por entidad.
// Prefijos: SOL_ (Solicitudes) | ACT_ (Actividades) | TODO_ (Todos)
// ============================================================
namespace CRM_MDaSoporteTI.Shared.Errors;

public static class ErrorCodes
{
    // Genéricos (cualquier entidad)
    public const string ErrorValidacion = "VALIDATION_ERROR";
    public const string ErrorInesperado = "UNEXPECTED_ERROR";

    // Solicitudes
    public const string SolicitudDuplicada = "SOL_DUPLICADA";
    public const string SolicitudDatosFaltantes = "SOL_DATOS_FALTANTES";
    public const string SolicitudNoEncontrada = "SOL_NO_ENCONTRADA";
    public const string SolicitudReglaNegocio = "SOL_REGLA_NEGOCIO";
    public const string SolicitudSinAsignar = "SOL_SIN_ASIGNAR";
    public const string SolicitudReferenciaInvalida = "SOL_REFERENCIA_INVALIDA";

    // Actividades
    public const string ActividadNoEncontrada = "ACT_NO_ENCONTRADA";
    public const string ActividadDatosFaltantes = "ACT_DATOS_FALTANTES";
    public const string ActividadReferenciaInvalida = "ACT_REFERENCIA_INVALIDA";
    public const string ActividadEstadoInvalido = "ACT_ESTADO_INVALIDO";

    // Todos
    public const string TodoNoEncontrado = "TODO_NO_ENCONTRADO";
    public const string TodoDatosFaltantes = "TODO_DATOS_FALTANTES";
    public const string TodoReferenciaInvalida = "TODO_REFERENCIA_INVALIDA";
}