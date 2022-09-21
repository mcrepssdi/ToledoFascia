using System.Text;
using Dapper;
using MySql.Data.MySqlClient;
using NLog;
using ToledoFasciaScans.Enumerations;
using ToledoFasciaScans.Models;
using ToledoFasciaScans.Utilities;

namespace ToledoFasciaScans.DataProviders;

public class SaiProvider : IMysqlProviders
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly string _connStr;
    private readonly string _branch;
    private readonly List<string> _supplier;

    public SaiProvider(string connStr, string branch,  List<string> suppliers)
    {
        _connStr = connStr;
        _branch = branch;
        _supplier = suppliers;
    }
    
    public LoadInfo Loads(DateTime today)
    {
        _logger.Trace("Entering...");
        DaysToSubtract daysToSubtract = today.GetDaysToSubtract();
        
        StringBuilder sb = new();
        sb.AppendLine("SELECT Sum(VehNetWt) AS NetWgt, Count(*) As Loads FROM Pur_Ticket_Hdr ");
        sb.AppendLine(" WHERE Branch = @Branch ");
        sb.Append(" AND SupplierNo IN ( ").Append(_supplier.InClause()).AppendLine(") ");
        
        if (daysToSubtract == DaysToSubtract.Monday)
        {
            sb.AppendLine("AND ShipmentDate BETWEEN DATE_SUB(CURDATE(), INTERVAL @Days DAY) ");
            sb.AppendLine("AND DATE_SUB(CURDATE(), INTERVAL @PreviousDays DAY) ");
        }
        else
        {
            sb.AppendLine("AND ShipmentDate = DATE_SUB(CURDATE(), INTERVAL @Days DAY) ");
        }
        DynamicParameters dp = new();
        dp.Add("@Branch", _branch);
        _supplier.ForEach(p =>
        {
            dp.Add($"@{p}", p);
        });
        dp.Add("@Days", daysToSubtract);
        dp.Add("@PreviousDays", DaysToSubtract.PreviousDay);
        
        try
        {
            using MySqlConnection conn = new(_connStr);
            conn.Open();
            LoadInfo loadinfo = conn.Query<LoadInfo>(sb.ToString(), dp).FirstOrDefault() ?? new LoadInfo();
            
            return loadinfo;
            
        }
        catch (Exception e)
        {
            _logger.Error($"Error fetch scans.  Msg: {e.Message}");
        }

        return new LoadInfo();
    }
}