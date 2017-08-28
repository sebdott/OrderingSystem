using System;
using Microsoft.Extensions.DependencyInjection;
using GONOrderingSystems.Common.Providers.Interface;
using Microsoft.Extensions.Configuration;
using GONOrderingSystems.Common.Common;
using GONOrderingSystems.Common.Providers.Implementation;
using Confluent.Kafka;
using GONOrderingSystems.Common.DataModels;
using Newtonsoft.Json;
using GONOrderingSystems.Services.Log.Interface;

namespace GONOrderingSystems.Services.Log
{
    class Program
    {
        private static IPubSubProvider _pubSubProvider;
        private static IConfigurationRoot _configuration;
        private static KafkaSettings _kafkaSettings;
        private static ILogProvider _logProvider;
        private static ILogServiceManager _logServiceManager;
        private static Serilog.Core.Logger _logger;

        static void Main(string[] args)
        {

            if (!StartUpService()) return;

            using (var consumer = _pubSubProvider
                .GetConsumerProvider(_kafkaSettings.BrokerList,
                _kafkaSettings.LogTopic, 
                _kafkaSettings.ConsumerGroupId,
                _kafkaSettings.Partition))
            {

                while (true)
                {
                    var orderValue = new Common.DataModels.Order();

                    try
                    {
                        Message<Null, string> msg;
                        if (consumer.Consume(out msg, TimeSpan.FromMilliseconds(100)))
                        {
                            var logValue = Serializer.Deserialize<Common.DataModels.LogItem>(msg.Value);

                            _logServiceManager.PublishLog(logValue);

                            var committedOffsets = consumer.CommitAsync(msg).Result;
                        }
                    }
                    catch
                    {

                    }
                }
            }


        }

        private static bool StartUpService()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                              .AddJsonFile(Constant.AppSettingsFile);

                 _configuration = builder.Build();

                _kafkaSettings = new KafkaSettings();
                _configuration.GetSection("KafkaSettings").Bind(_kafkaSettings);

                var graylogSettings = new GraylogSettings();
                _configuration.GetSection("GraylogSettings").Bind(graylogSettings);
                
                var serviceProvider = new ServiceCollection()
                .AddSingleton<IPubSubProvider, KafkaProvider>()
                .AddSingleton<ILogProvider, LogProvider>()
                .BuildServiceProvider();
                
                var dbProvider = serviceProvider.GetService<IPubSubProvider>();
                _pubSubProvider = serviceProvider.GetService<IPubSubProvider>();
                _logProvider = serviceProvider.GetService<ILogProvider>();
                _logServiceManager = serviceProvider.GetService<ILogServiceManager>();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }
    }
}
