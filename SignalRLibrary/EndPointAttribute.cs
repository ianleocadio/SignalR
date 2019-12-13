using System;

namespace SignalRLibrary
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class EndPointAttribute : Attribute
    {
        public EndPointAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
