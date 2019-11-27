using SignalRServer.Caller.Models;
using System.Collections.Generic;

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
    }
}
