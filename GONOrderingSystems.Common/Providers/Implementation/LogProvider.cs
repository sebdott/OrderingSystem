using GONOrderingSystems.Common.Common;
using GONOrderingSystems.Common.DataModels;
using GONOrderingSystems.Common.Providers.Interface;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace GONOrderingSystems.Common.Providers.Implementation
{
    public class LogProvider : ILogProvider
    {
        private IPubSubProvider _pubSubProvider;
        private IOptions<KafkaSettings> _kafkaSettings;

        public LogProvider(IPubSubProvider pubSubProvider, IOptions<KafkaSettings> kafkaSettings)
        {
            _pubSubProvider = pubSubProvider;
            _kafkaSettings = kafkaSettings;
        }

        public async Task PublishError(string identifier, string message, Exception exception)
        {

            var _logItem = new LogItem()
            {
                Identifier = identifier,
                Message = message,
                Exception = exception.Message + " " + JsonConvert.SerializeObject(exception.StackTrace),
                Type = LogType.Error
            };

            await PublishLog(_logItem);
        }

        public async Task PublishInfo(string identifier, string message)
        {
            var _logItem = new LogItem()
            {
                Identifier = identifier,
                Message = message,
                Type = LogType.Information
            };

            await PublishLog(_logItem);
        }

        private async Task PublishLog(LogItem logitem)
        {
            using (var producer = _pubSubProvider.GetPublishProvider(_kafkaSettings.Value.BrokerList,
                                     _kafkaSettings.Value.ProducerGroupId))
            {
                var deliveryReport = producer.ProduceAsync(_kafkaSettings.Value.LogTopic, null, Serializer.Serialize(logitem).ToString(), _kafkaSettings.Value.Partition);
                deliveryReport.ContinueWith(task =>
                {
                    Console.WriteLine($"Partition: {task.Result.Partition}, Offset: {task.Result.Offset}");
                });

                producer.Flush(1000);
            }
        }
    }
}
