using Microsoft.Extensions.Logging;
using SignalRServer.Caller.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace SignalRServer.Caller.Controllers
{
    public abstract class CallerController<T> where T : ACaller
    {
        protected ILogger _logger { get; set; }
        public bool AtLeastOneCaller
        {
            get
            {
                return Callers.Count > 0;
            }
        }
        public ConcurrentDictionary<string, T> Callers { get; }

        public CallerController(ILogger logger)
        {
            _logger = logger;
            Callers = new ConcurrentDictionary<string, T>(Environment.ProcessorCount*2, 20);
        }

        public void AddCaller(T t)
        {
            Callers.AddOrUpdate(t.UserAuthentication, t,
                (key, caller) =>
                {
                    return t;
                });
        }

        public virtual bool RemoveCaller(string userAuthentication)
        {
            return Callers.TryRemove(userAuthentication, out _);
        }

    }
}
