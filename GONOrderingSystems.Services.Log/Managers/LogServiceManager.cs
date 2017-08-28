using GONOrderingSystems.Common.DataModels;
using GONOrderingSystems.Common.Common;
using Serilog;
using Serilog.Sinks.Graylog;
using System;
using GONOrderingSystems.Services.Log.Interface;

namespace GONOrderingSystems.Services.Log.Managers
{
    public class LogServiceManager : ILogServiceManager
    {
        private static Serilog.Core.Logger _logger;

        public LogServiceManager(GraylogSettings graylogSettings)
        {
            var loggerConfig = new LoggerConfiguration()
                  .WriteTo.Graylog(new GraylogSinkOptions
                  {
                      HostnameOrAdress = graylogSettings.Host,
                      Port = graylogSettings.Port
                  });

            if (_logger != null)
            {
                _logger = loggerConfig.CreateLogger();
            }
        }

        public bool PublishLog(LogItem logItem)
        {
            switch (logItem.Type)
            {
                case LogType.Information:
                    PublishInfo(logItem.Identifier, logItem.Message);
                    return true;
                case LogType.Error:
                    PublishError(logItem.Identifier, logItem.Message,  logItem.Exception);
                    return true;
            }

            return false;
        }
        private void PublishError(string eventID, string message, string exception)
        {
            _logger.Error(string.Format(Constant.LoggingFormat, eventID, message), exception);
        }

        private void PublishInfo(string eventID, string message)
        {
            _logger.Information(string.Format(Constant.LoggingFormat, eventID, message));
        }

    }
}
