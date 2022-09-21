using SdiEmailLib.Models;
using ToledoFasciaScans.Models;

namespace ToledoFasciaScans.Services;

public interface IWorker
{
    public SdiEmail Email(DateTime today, int scans, LoadInfo loadInfo);
    public Task Send(SdiEmail email);
}