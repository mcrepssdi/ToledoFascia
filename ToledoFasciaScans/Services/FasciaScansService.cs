using System.Data.SqlClient;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using Dapper;
using MySql.Data.MySqlClient;
using NLog;
using SdiEmailLib.Models;
using SdiEmailLib.Processes;
using SdiUtility;
using ToledoFasciaScans.DataProviders;
using ToledoFasciaScans.Enumerations;
using ToledoFasciaScans.Models;
using ToledoFasciaScans.ToledoConfigMgr;
using ToledoFasciaScans.Utilities;

namespace ToledoFasciaScans.Services;

public class ScansService : BackgroundService, IWorker
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private const string EmailDelimiter = ";";
    
    private readonly ConfigMgr _configMgr;
    private readonly IEmailSender _emailSender;
    
    private readonly IMsSqlProvider _msSqlProvider;
    private readonly IMysqlProviders _mysqlProvider;
    

    public ScansService(ConfigMgr cm,  IEmailSender emailSender, IMsSqlProvider msSqlProvider, IMysqlProviders mysqlProvider)
    {
        _configMgr = cm;
        _emailSender = emailSender;
        _msSqlProvider = msSqlProvider;
        _mysqlProvider = mysqlProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.Trace($"Worker running at: {DateTimeOffset.Now}");
            
            DateTime today = DateTime.Now;
            if (today.IsStaurdayOrSunday()) continue;
            
            int scans = _msSqlProvider.Scans(today);
            LoadInfo loadInfo = _mysqlProvider.Loads(today);
            
            await Send(Email(today, scans, loadInfo));
            
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
    
    public SdiEmail Email(DateTime today, int scans, LoadInfo loadInfo)
    {
        SdiEmail email = new ()
        {
            Subject = "Toledo Fascia Scans",
            Body = BuildForm(today, scans, loadInfo),
            To = _configMgr.AppEnvironment.SendTo.Split(EmailDelimiter).ToList()
        };

        return email;
    }

    public Task Send(SdiEmail email)
    {
        try
        {
            _emailSender.Send(email, _configMgr.EmailHost.FromAddress, MailPriority.Normal, true);
        }
        catch (Exception e)
        {
            _logger.Error($"Unable to send email.  Msg: {e.Message}");    
        }
        
        return Task.CompletedTask;
    }
    
    private string BuildForm(DateTime today, int scans, LoadInfo loadInfo)
    {
        StringBuilder sb = new();
        sb.AppendLine("<html>");
        sb.AppendLine("   <head style=\"font-size:40px;\">");
        sb.AppendLine("         Toledo AMR - FCA");
        sb.AppendLine("   </head>");
        sb.AppendLine("   <form>");
        sb.Append("             <div style=\"font-size:40px;\"><label>Date: ").Append(today.AddDays(-1).Format()).AppendLine("</label></div>");
        sb.Append("             <div style=\"font-size:40px;\"><label>Loads: ").Append(loadInfo.Loads).AppendLine("</label></div>");
        sb.Append("             <div style=\"font-size:40px;\"><label>Net Wgt: ").Append(loadInfo.NetWgt).AppendLine("</label></div>");
        sb.Append("             <div style=\"font-size:40px;\"><label>Scans: ").Append(scans).AppendLine("</label></div>");
        sb.AppendLine("   </form>");
        sb.AppendLine("</html>");
        return sb.ToString();
    }
    
}

