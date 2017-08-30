using GONOrderingSystems.Common.DataModels;
using GONOrderingSystems.Common.Common;
using Serilog;
using Serilog.Sinks.Graylog;
using System;
using GONOrderingSystems.Services.Log.Interface;
using Microsoft.Extensions.Options;
using System.Threading;

namespace GONOrderingSystems.Services.Log.Managers
{
    public class LogServiceManager : ILogServiceManager
    {
        private static Serilog.Core.Logger _logger;

        public LogServiceManager(IOptions<GraylogSettings> graylogSettings)
        {

            var retry = true;
            var retryNo = 0;

            while (retry)
            {
                try
                {
                    var loggerConfig = new LoggerConfiguration()
                                     .WriteTo.Graylog(new GraylogSinkOptions
                                     {
                                         HostnameOrAdress = graylogSettings.Value.Host,
                                         Port = graylogSettings.Value.Port
                                     });

                    if (_logger == null)
                    {
                        _logger = loggerConfig.CreateLogger();
                    }

                    retry = false;
                }
                catch (Exception ex)
                {
                    retryNo++;
                    Thread.Sleep(5000);
                }

                if (retryNo > 3)
                {
                    retry = false;
                }

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
