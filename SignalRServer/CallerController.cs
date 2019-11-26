using System;
using System.Collections.Generic;
using System.Text;

namespace SignalRServer
{
    public class CallerController<T> where T : ACaller
    {

        public bool AtLeastOneCaller 
        {
            get
            {
                return (Callers.Count > 0);
            }
        }

        public List<T> Callers { get; }

        public CallerController()
        {
            Callers = new List<T>();
        }
    }
}
