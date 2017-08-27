using System;
using Microsoft.Extensions.DependencyInjection;
using GONOrderingSystems.Common.Providers.Interface;
using GONOrderingSystems.Common.DataModels;
using Microsoft.Extensions.Configuration;
using GONOrderingSystems.Common.Common;
using GONOrderingSystems.Common.Providers.Implementation;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Confluent.Kafka;

namespace GONOrderingSystems.Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                     .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            
            var serviceProvider = new ServiceCollection()
            .AddTransient<IPubSubProvider, KafkaProvider>()
            .AddSingleton<IMetricsProvider, PrometheusProvider>()
            .BuildServiceProvider();
            
            var _pubSubProvider = serviceProvider.GetService<IPubSubProvider>();

            using (var consumer = _pubSubProvider
                .GetConsumerProvider(configuration["KafkaSettings:BrokerList"].ToString(),
                 configuration["KafkaSettings:Topic"].ToString(),
                 configuration["KafkaSettings:ConsumerGroupId"].ToString(), Int32.Parse(configuration["KafkaSettings:Partition"].ToString())))
            {

                while (true)
                {
                    Message<Null, string> msg;
                    if (consumer.Consume(out msg, TimeSpan.FromMilliseconds(100)))
                    {
                        var msgValue = Serializer.Deserialize<Order>(msg.Value);

                        if (msgValue == null)
                        {
                            continue;
                        }

                        using (var client = new HttpClient())
                        {
                            string stringData = JsonConvert.SerializeObject(msgValue);
                            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");

                            var commit = Task.Run(() => client.PostAsync(configuration["RestHostSettings:Host"].ToString(), contentData));

                            commit.Wait();

                            if (commit.Result.IsSuccessStatusCode)
                            {
                                var committedOffsets = consumer.CommitAsync(msg).Result;
                            }
                        }
                    }
                }
            }
        }
    }
}
