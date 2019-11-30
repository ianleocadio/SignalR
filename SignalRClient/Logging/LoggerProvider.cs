using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.EventLog;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SignalRClient.Logging
{
    public class LoggerProvider
    {
        private readonly IConfiguration _configuration;
        private readonly LoggerConfiguration _loggerConfiguration;

        public LoggerProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _loggerConfiguration = CreateLoggerFactory();
        }

        public ILogger GetLogger() => _loggerConfiguration.CreateLogger();

        private LoggerConfiguration CreateLoggerFactory() 
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.EventLog(_configuration["Logging:EventLogSourceName"], manageEventSource: true)
                .WriteTo.File(_configuration["Logging:LogFilePath"])
                .WriteTo.ColoredConsole();
        }
    }
}
