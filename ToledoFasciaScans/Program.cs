using NLog;
using SdiEmailLib.Dao;
using SdiEmailLib.Models;
using SdiEmailLib.Processes;
using SdiUtility;
using ToledoFasciaScans.DataProviders;
using ToledoFasciaScans.Services;
using ToledoFasciaScans.ToledoConfigMgr;

IHost host = Host.CreateDefaultBuilder(args)
    
    .ConfigureServices((hostContext, services) =>
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        IHostEnvironment hostEnv = hostContext.HostingEnvironment;
        
        IConfigurationBuilder builder = new ConfigurationBuilder();
        const string basePath = "appsettings.json";
        
        builder.SetBasePath(AppContext.BaseDirectory).AddJsonFile(basePath, false, true);
        IConfiguration configuration = builder.Build();
        ConfigMgr configMgr = new(configuration, "Production");
        
        NLogExtension.UpdateNLogConfig(configMgr.AppEnvironment.LogDirectory, "database", configMgr.ConfigManagerSettings.ConnectionString);
        VersionUtil vu = new();
        logger.Info($"Starting SDI to Concur File Transfer.");
        new VersionUtil().LogVersionInfo(logger);
        logger.Debug($"=========================== Branch:            {vu.GitBranch}");
        logger.Debug($"=========================== Commit ID:         {vu.CommitId}");
        logger.Debug($"=========================== App Name:          {configMgr.AppEnvironment.ApplicationName}");
        logger.Debug($"=========================== Prod:              {hostEnv.IsProduction()}");
        logger.Debug($"=========================== Staging:           {hostEnv.IsStaging()}");
        logger.Debug($"=========================== Development:       {hostEnv.IsDevelopment()}");
        logger.Debug($"=========================== CM Server:         {configMgr.ConfigManagerSettings.CmServer}");
        logger.Debug($"=========================== CM DB:             {configMgr.ConfigManagerSettings.CmDb}");
        logger.Debug($"=========================== App Server:        {configMgr.AppEnvironment.AppDb}");
        logger.Debug($"=========================== App DB:            {configMgr.AppEnvironment.AppConnectionStr}");
        
        HostInfo hostInfo = new ()
        {
            Password = configMgr.EmailHost.Password,
            Port = configMgr.EmailHost.Port,
            Username = configMgr.EmailHost.UserName,
            FromAddress = configMgr.EmailHost.FromAddress,
            HostName = configMgr.EmailHost.HostName
        };
        
        IEmailSender emailSender = new EmailSender(hostInfo,
            new EmailDaoImpl(configMgr.ConfigManagerSettings.ConnectionString, logger),
            configMgr.AppEnvironment.ApplicationId, logger);

        IMsSqlProvider msSqlProvider = new FasciaProvider(configMgr.AppEnvironment.AppConnectionStr);
        IMysqlProviders mysqlProvider = new SaiProvider(configMgr.SaiMySqlConnection.ConnectionString, configMgr.Branch, configMgr.Suppliers);

        /* Add All Dependency Injection (DI) Classes */
        services.AddSingleton(configMgr);
        services.AddSingleton(msSqlProvider);
        services.AddSingleton(mysqlProvider);
        services.AddSingleton(emailSender);
        services.AddHostedService<ScansService>();
    })
    .ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders())
    .Build();

await host.RunAsync();