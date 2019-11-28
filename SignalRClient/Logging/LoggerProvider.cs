using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.EventLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SignalRClient.Logging
{
    public class LoggerProvider : IDisposable
    {

        private readonly ILoggerFactory loggerFactory;
        private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();
        
        private static LoggerProvider _instance;

        public LoggerProvider()
        {
            loggerFactory = CreateLoggerFactory();
        }

        public static ILogger<T> GetLogger<T>()
        {
            if (_instance == null)
                _instance = new LoggerProvider();

            return _instance.CreateLogger<T>(typeof(T).Name);
        }

        private ILogger<T> CreateLogger<T>(string name)
        {
            return (ILogger<T>)_loggers.GetOrAdd(name, name => loggerFactory.CreateLogger<T>());
        }

        public ILoggerFactory CreateLoggerFactory() 
        {
            return LoggerFactory.Create(builder =>
            {
                builder
                    .ClearProviders()
                    .AddProvider(new EventLogLoggerProvider())
                    .AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Information)
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    //.AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole()
                    .AddEventLog();
            });
        }

    public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
