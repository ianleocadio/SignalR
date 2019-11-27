using System;
using System.Collections.Generic;
using System.Text;
using Topshelf.Logging;

namespace SignalRClient
{
    //[assembly: OwinStartup(typeof(Client))]
    public class FilialSignalRService : IDisposable
    {

        public static readonly LogWriter Log = HostLogger.Get<FilialSignalRService>();

        public FilialSignalRService()
        {

        }

        public void OnStart(string[] args)
        {
            Log.InfoFormat("FilialSignalRService: In OnStart");
        }
        public void OnStop()
        {
            Log.InfoFormat("FilialSignalRService: In OnStop");
        }

        public void Dispose()
        {
        }
    }
}
