using System.Data.SqlClient;
using System.Text;
using Dapper;
using NLog;
using ToledoFasciaScans.Enumerations;
using ToledoFasciaScans.Utilities;

namespace ToledoFasciaScans.DataProviders;

public class FasciaProvider : IMsSqlProvider
{
    
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly string _connStr;
    
    public FasciaProvider(string connStr)
    {
        _connStr = connStr;
    }
    
    public int Scans(DateTime today)
    {
        _logger.Trace("Entering...");
        DaysToSubtract daysToSubtract = today.GetDaysToSubtract();
        
        StringBuilder sb = new();
        sb.AppendLine("SELECT COUNT(*) AS Scans");
        sb.AppendLine(" FROM dbo.FasciaScan");
        if (daysToSubtract == DaysToSubtract.Monday)
        {
            sb.AppendLine(" WHERE CAST (LastScannedDateTime as date) BETWEEN CAST(GETDATE() - @Days as aate) ");
            sb.AppendLine(" AND CAST(GETDATE() - @PreviousDay as Date) ");
        }
        else
        {
            sb.AppendLine(" WHERE CAST (LastScannedDateTime as date) = CAST(GETDATE() - @Days as date)");
        }
        
        DynamicParameters dp = new();
        dp.Add("@Days", daysToSubtract);
        dp.Add("@PreviousDay", DaysToSubtract.PreviousDay);
        try
        {
            using SqlConnection conn = new(_connStr);
            int scans = conn.Query<int>(sb.ToString(), dp).FirstOrDefault();
            _logger.Trace($"Scans Found {scans}");
            return scans;
        }
        catch (Exception e)
        {
            _logger.Error($"Error fetch scans.  Msg: {e.Message}");
        }
        return 0;
    }
}