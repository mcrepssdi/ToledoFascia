using ToledoFasciaScans.Models;

namespace ToledoFasciaScans.DataProviders;

public interface IMysqlProviders
{
    public LoadInfo Loads(DateTime today);
}