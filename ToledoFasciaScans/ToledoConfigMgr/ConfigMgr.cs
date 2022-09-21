using SdiConfigManager;
using SdiEmailLib.Utilities.cs;
using ToledoFasciaScans.ToledoConfigMgr.Sections;

namespace ToledoFasciaScans.ToledoConfigMgr;

public class ConfigMgr: IConfigMgr
{
    private readonly IConfiguration _configuration;
    private readonly string _environment;
    private readonly string _applicationName;
    public ConfigManagerSettings ConfigManagerSettings { get; set; } = new();
    public AppEnvironment AppEnvironment { get; set; } = new();
    public EmailHost EmailHost { get; set; }
    public SaiMySqlConnection SaiMySqlConnection { get; set; }
    public string Branch { get; set; }
    public List<string> Suppliers { get; set; } = new();
    
    public ConfigMgr(IConfiguration configuration, string environment)
    {
        if (environment.IsEmpty()) throw new Exception("Environment cannot be empty or null");
        _applicationName = configuration.GetValue<string>("ApplicationName");
        _environment = environment;
        _configuration = configuration;
        
        Branch = configuration.GetValue<string>("Branch");
        Suppliers = new List<string>(configuration.GetValue<string>("Suppliers").Split(","));
        
        SetEnvironment();
        ConfigManager cm = new(GetKeyValuePairs());
        
        AppEnvironment.ApplicationId = cm.ApplicationId;

        EmailHost = new EmailHost(cm.GetConfigGroup("EmailNoReplyOmnisource"));
        SaiMySqlConnection = new SaiMySqlConnection(cm.GetConfigGroup("SaiSupportMySqlConnection"));
        ConfigManagerSettings.ConnectionString = cm.ConnectionString;
    }

    private List<KeyValuePair<string, string>> GetKeyValuePairs()
    {
        List<KeyValuePair<string, string>> kv = new()
        {
            new KeyValuePair<string, string>("ApplicationName", AppEnvironment.ApplicationName),
            new KeyValuePair<string, string>("DbName", ConfigManagerSettings.CmDb),
            new KeyValuePair<string, string>("Server", ConfigManagerSettings.CmServer)
        };
        
        IConfigurationSection section = _configuration.GetSection("ConfigMgrGroups");
        kv.AddRange(section.GetChildren().Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value)));

        return kv;
    }

    private void SetEnvironment()
    {
        IConfigurationSection section = GetSection(_environment);
        AppEnvironment = new AppEnvironment
        {
            ApplicationName = _applicationName,
            Environment = _environment,
            AppServer = section.GetValue<string>("AppServer"),
            AppDb = section.GetValue<string>("AppDb"),
            LogDirectory = section.GetValue<string>("LogDirectory"),
            SendTo = section.GetValue<string>("SendTo")
        };
        AppEnvironment.AppConnectionStr = $"Server={AppEnvironment.AppServer};Database={AppEnvironment.AppDb};Trusted_Connection=True;";
        
        ConfigManagerSettings = new ConfigManagerSettings
        {
            CmServer = section.GetValue<string>("CmServer"),
            CmDb = section.GetValue<string>("CmDb"),
        };
    }
    
    private IConfigurationSection GetSection(string sectionName)
    {
        IConfigurationSection section = _configuration.GetSection(sectionName);
        if (section == null)
        {
            throw new Exception($"Cannot find suitable environment or environment is null.  Section: {sectionName}");
        }

        return section;
    }
}