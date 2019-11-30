using Microsoft.Extensions.Logging;
using SignalRServer.Caller.Models;
using System;
using System.Collections.Generic;
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
        public List<T> Callers { get; }

        public CallerController(ILogger logger)
        {
            _logger = logger;
            Callers = new List<T>();
        }

        public abstract void AddCaller(T t);

        protected virtual bool EnableCaller(T t)
        {
            var caller = Callers.FirstOrDefault(c => c.UserAuthentication == t.UserAuthentication);
            if (caller != null)
            {
                caller.Alive = true;
                caller.Caller = t.Caller;
                return true;
            }

            return false;
        }

        public virtual void DisableCaller(string userAuthentication)
        {
            var Caller = Callers.FirstOrDefault(c => c.UserAuthentication == userAuthentication);
            if (Caller != null)
                Caller.Alive = false;
        }

    }
}
