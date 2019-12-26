using Microsoft.Extensions.Logging;

namespace SignalRLibrary
{
    public abstract class Plugin
    {
        private ILogger _logger;
        public ILogger Logger 
        {
            get => _logger;
            set 
            {
                if (_logger == null)
                    _logger = value;
            } 
        }
    }
}
