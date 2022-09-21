using SdiConfigManager;

namespace ToledoFasciaScans.ToledoConfigMgr.Sections;

public class SaiMySqlConnection
{
    public string ConnectionString { get; set; }
    
    public SaiMySqlConnection(ConfigGroup cg)
    {
        string server = cg.ci("Server");
        string port = cg.ci("Port");
        string user = cg.ci("User");
        string password = cg.ci("Password");
        string omniDatabase = cg.ci("OmniDatabase");
        ConnectionString = $"Server={server};Database={omniDatabase};Uid={user};Pwd={password};";
    }
}