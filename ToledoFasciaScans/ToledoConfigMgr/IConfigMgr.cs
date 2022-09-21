using ToledoFasciaScans.ToledoConfigMgr.Sections;

namespace ToledoFasciaScans.ToledoConfigMgr;

public interface IConfigMgr
{
    public ConfigManagerSettings ConfigManagerSettings { get; set; }
    public AppEnvironment AppEnvironment { get; set; }
    public EmailHost EmailHost { get; set; }
    
    public SaiMySqlConnection SaiMySqlConnection { get; set; }
    public string Branch { get; set; }
    public List<string> Suppliers { get; set; }
}