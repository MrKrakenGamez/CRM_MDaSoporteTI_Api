namespace CRM_MDaSoporteTI.Infrastructure.Configuration;

public sealed class ConnectionStringOptions
{
    public const string SectionName = "ConnectionStrings";
    public string DefaultConnection { get; set; } = string.Empty;
}