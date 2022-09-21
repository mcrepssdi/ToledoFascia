using SdiConfigManager;

namespace ToledoFasciaScans.ToledoConfigMgr.Sections;

public class EmailHost
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public int Port { get; set; } = 22;
    
    public EmailHost(ConfigGroup cg)
    {
        UserName = cg.ci("UserName");
        Password = cg.ci("Password");
        HostName = cg.ci("Host");
        FromAddress = cg.ci("From");

        bool success = int.TryParse(cg.ci("Port"), out int port);
        if (!success)
        {
            throw new InvalidCastException($"Unable to cast the Port to a valid number.  Port: {cg.ci("Port")}");
        }
        Port = port;
    }

    public EmailHost() { }
}