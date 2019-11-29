using System;
using System.Collections.Generic;
using System.Text;

namespace SignalRClient.Configurations
{
    public class CustomConfigurationPOJO
    {

        #region Geral
        public Geral geral { get; set; }
        public partial class Geral
        {
            public string Unidade { get; set; }
        }
        #endregion

        #region Logging
        public Logging logging { get; set; }
        public partial class Logging
        {
            public string LogFilePath { get; set; }
            public string EventLogSourceName { get; set; }

        }
        #endregion

        #region ServerConnection
        public ServerConnection serverConnection { get; set; }

        public partial class ServerConnection
        {
            public string URI { get; set; }
        }
        #endregion

        #region Authentication
        public Authentication authentication { get; set; }
        public partial class Authentication
        {
            public string Username { get; set; }
            public string Password { get; set; }

            public string GetCredentials()
            {
                return Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                    .GetBytes($"{Username}" + ":" + $"{Password}"));
            }
        }
        #endregion
    }
}
