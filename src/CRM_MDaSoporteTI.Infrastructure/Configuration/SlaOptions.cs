namespace CRM_MDaSoporteTI.Infrastructure.Configuration;

public sealed class SlaOptions
{
    public const string SectionName = "Sla";
    public int DefaultMaxMinutes { get; set; } = 120;
}