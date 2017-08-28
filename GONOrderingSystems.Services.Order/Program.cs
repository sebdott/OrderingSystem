using System;
using Microsoft.Extensions.DependencyInjection;
using GONOrderingSystems.Common.Providers.Interface;
using Microsoft.Extensions.Configuration;
using GONOrderingSystems.Common.Common;
using GONOrderingSystems.Common.Providers.Implementation;
using System.Threading.Tasks;
using Confluent.Kafka;
using GONOrderingSystems.Logic.Managers.Interface;
using GONOrderingSystems.Services.Order.Managers;
using GONOrderingSystems.Common.DataModels;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;

namespace GONOrderingSystems.Services.Order
{
    class Program
    {
        private static IPubSubProvider _pubSubProvider;
        private static IOrderServiceManager _orderServiceManager;
        private static IConfigurationRoot _configuration;
        private static KafkaSettings _kafkaSettings;
        private static IMetricsProvider _metricsProvider;
        private static ILogProvider _logProvider;

        static void Main(string[] args)
        {

            if (!StartUpService()) return;

            using (var consumer = _pubSubProvider
                .GetConsumerProvider(_kafkaSettings.BrokerList,
                _kafkaSettings.OrderTopic, 
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
                            orderValue = Serializer.Deserialize<Common.DataModels.Order>(msg.Value);
                            
                            var commit = Task.Run(() => _orderServiceManager.CommitOrder(orderValue));

                            commit.Wait();

                            if (commit.Result)
                            {
                                if (!string.IsNullOrEmpty(orderValue.OrderId))
                                {   
                                    Task.Run(() => _logProvider.PublishInfo(orderValue.EventID, "Order Success -" + JsonConvert.SerializeObject(orderValue)));
                                    _metricsProvider.RestCounterIncrement(_configuration["RestHostSettings:Host"].ToString(), MetricCounter.SuccessOrderCounter);
                                }
                                else
                                {
                                    Task.Run(() => _logProvider.PublishInfo("GONOrderingSystems.Services.Order", "Fail to Commit " + JsonConvert.SerializeObject(orderValue)));
                                    _metricsProvider.RestCounterIncrement(_configuration["RestHostSettings:Host"].ToString(), MetricCounter.FailedCommitCounter);

                                }

                                var committedOffsets = consumer.CommitAsync(msg).Result;

                                
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Task.Run(() => _logProvider.PublishError("GONOrderingSystems.Services.Order", "Error on Commit " + JsonConvert.SerializeObject(orderValue), ex));
                    }
                }
            }


        }

        private static bool StartUpService()
        {
            try
            {

                var builder = new ConfigurationBuilder()
               .AddJsonFile(Constant.AppSettingsFile, optional: false, reloadOnChange: true);

                _configuration = builder.Build();

                _kafkaSettings = new KafkaSettings();
                _configuration.GetSection("KafkaSettings").Bind(_kafkaSettings);

                var mongoDbSettings = new MongoDbSettings();
                _configuration.GetSection("MongoDbSettings").Bind(mongoDbSettings);

                var serviceProvider = new ServiceCollection()
                .AddSingleton<IPubSubProvider, KafkaProvider>()
                .AddSingleton<IOrderServiceManager, OrderServiceManager>()
                .AddSingleton<ILogProvider, LogProvider>()
                .AddSingleton<IMetricsProvider, PrometheusProvider>()
                .AddSingleton<IDataProvider, MongoDBProvider>(settings =>
                   new MongoDBProvider(mongoDbSettings.Host, mongoDbSettings.Database, mongoDbSettings.OrderCollection))
                .Configure<KafkaSettings>(_configuration.GetSection("KafkaSettings"))
                .AddOptions()
                .BuildServiceProvider();
                
                _pubSubProvider = serviceProvider.GetService<IPubSubProvider>();
                _orderServiceManager = serviceProvider.GetService<IOrderServiceManager>();
                _logProvider = serviceProvider.GetService<ILogProvider>();
                _metricsProvider = serviceProvider.GetService<IMetricsProvider>();
                var dbProvider = serviceProvider.GetService<IDataProvider>();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

        }

        public Action<object> Convert<T>(Action<T> myActionT)
        {
            if (myActionT == null) return null;
            else return new Action<object>(o => myActionT((T)o));
        }
    }
}
