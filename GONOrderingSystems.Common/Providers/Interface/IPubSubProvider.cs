using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;

namespace GONOrderingSystems.Common.Providers.Interface
{
    public interface IPubSubProvider
    {
        Consumer<Null, string> GetConsumerProvider(string brokerList, string topic,string groupId, int partition);
        Producer<Null, string> GetPublishProvider(string brokerList, string groupId);
    }
}
