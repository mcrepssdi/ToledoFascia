namespace ToledoFasciaScans.DataProviders;

public interface IMsSqlProvider
{
    /// <summary>
    /// Get Scans from Previous Days
    /// </summary>
    /// <param name="today"></param>
    /// <returns></returns>
    public int Scans(DateTime today);
}