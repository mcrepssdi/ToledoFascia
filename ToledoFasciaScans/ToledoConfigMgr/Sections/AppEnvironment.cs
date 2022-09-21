namespace ToledoFasciaScans.ToledoConfigMgr.Sections;

public class AppEnvironment
{
    public int ApplicationId { get; set; }
    public string ApplicationName { get; init; } = string.Empty;
    public string Environment { get; init; }= string.Empty;
    public string LogDirectory { get; set; }= string.Empty;

    public string AppDb { get; init; } = string.Empty;
    public string AppServer { get; init; } = string.Empty;
    public string AppConnectionStr { get; set; } = string.Empty;
    public string SendTo { get; set; } = string.Empty;
}