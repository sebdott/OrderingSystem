using GONOrderingSystems.Common.Common;
using GONOrderingSystems.Common.Providers.Interface;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Graylog;
using System;
using System.Collections.Generic;
using System.Text;

namespace GONOrderingSystems.Common.Providers.Implementation
{
    public class LogProvider : ILogProvider
    {
        private readonly Logger _logger;

        public LogProvider(string hostName, int portNumber)
        {
            _logger = GetLogger(hostName, portNumber);
        }

        public void PublishError(string eventID, string message, Exception exception)
        {
            _logger.Error(string.Format(Constant.LoggingFormat, eventID , message), exception);
        }

        public void PublishInfo(string eventID, string message)
        {
            _logger.Information(string.Format(Constant.LoggingFormat, eventID, message));
        }

        private Logger GetLogger(string hostName, int portNumber)
        {
            var loggerConfig = new LoggerConfiguration()
            .WriteTo.Graylog(new GraylogSinkOptions
            {
                HostnameOrAdress = hostName,
                Port = portNumber
            });

            return loggerConfig.CreateLogger();

        }
    }
}
