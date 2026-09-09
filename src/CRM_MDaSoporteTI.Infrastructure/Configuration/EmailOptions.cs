namespace CRM_MDaSoporteTI.Infrastructure.Configuration;

public sealed class EmailOptions
{
    public const string SectionName = "Email";
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromDisplayName { get; set; } = string.Empty;
    public string TeamNotificationEmail { get; set; } = string.Empty;
}