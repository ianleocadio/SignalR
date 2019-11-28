using System;
using System.Collections.Generic;
using System.Text;

namespace SignalRClient.Configurations
{
    public class CustomConfiguration
    {

        public partial class ServerConnection
        {
            public string URI { get; set; }
        }

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
    }
}
