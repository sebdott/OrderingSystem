using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using GONOrderingSystems.Common.Providers.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace GONOrderingSystems.Common.Providers.Implementation
{
    public class KafkaProvider : IPubSubProvider , IDisposable
    {
        private Producer<Null, string> _producer;
        private Consumer<Null, string> _consumer;


        public Consumer<Null, string> GetConsumerProvider(string brokerList, string topic, string groupId, int partition)
        {
            var config = new Dictionary<string, object>
            {
                { "group.id",groupId },
                { "partition.assignment.strategy", "roundrobin" },
                { "enable.auto.commit", true },
                { "auto.commit.interval.ms", 1 },
                { "statistics.interval.ms", 1 },
                { "queue.buffering.max.ms", 1 },
                { "bootstrap.servers", brokerList },
                { "default.topic.config", new Dictionary<string, object>()
                    {
                        { "auto.offset.reset", "smallest" }
                    }
                }
            };

            _consumer = new Consumer<Null, string>(config, null, new StringDeserializer(Encoding.UTF8));

            _consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(topic, 0, 0) });

            _consumer.OnPartitionEOF += (_, end)
              => Console.WriteLine($"Reached end of topic {end.Topic} partition {end.Partition}, next message will be at offset {end.Offset}");

            _consumer.OnError += (_, error)
                => Console.WriteLine($"Error: {error}");

            _consumer.OnPartitionsAssigned += (_, partitions) =>
            {
                _consumer.Assign(partitions);
            };

            _consumer.OnPartitionsRevoked += (_, partitions) =>
            {
                _consumer.Unassign();
            };

            _consumer.Subscribe(topic);

            return _consumer;
        }

        public Producer<Null, string> GetPublishProvider(string brokerList, string groupId)
        {
            var config = new Dictionary<string, object>
            {
                { "group.id", groupId },
                { "bootstrap.servers", brokerList }
            };
            
            _producer = new Producer<Null, string>(config, null, new StringSerializer(Encoding.UTF8));

            return _producer;
        }

        public void Dispose()
        {
            if (_consumer != null)
            {
                _consumer.Dispose();
            }

            if (_producer != null)
            {
                _producer.Dispose();
            }
        }
    }
}
