using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.EventLog;
using Serilog;
using SignalRClient.Configurations;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SignalRClient.Logging
{
    public class LoggerProvider
    {

        private readonly CustomConfiguration _configuration;
        private readonly LoggerConfiguration _loggerConfiguration;

        public LoggerProvider(CustomConfiguration configuration)
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
                .WriteTo.EventLog(_configuration.Data.logging.EventLogSourceName, manageEventSource: true)
                .WriteTo.File(_configuration.Data.logging.LogFilePath)
                .WriteTo.ColoredConsole();
        }
    }
}
