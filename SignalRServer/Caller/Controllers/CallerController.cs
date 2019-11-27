using SignalRServer.Caller.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalRServer.Caller.Controllers
{
    public class CallerController<T> where T : ACaller
    {

        public bool AtLeastOneCaller
        {
            get
            {
                return Callers.Count > 0;
            }
        }

        public List<T> Callers { get; }

        public CallerController()
        {
            Callers = new List<T>();
        }

        public void AddCaller(T t)
        {
            if (t == null)
            {
                Console.WriteLine("Caller não foi criado pois não foi passado informações do client");
                return;
            }

            if (!EnableCaller(t))
            {
                Callers.Add(t);
            }
        }

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
