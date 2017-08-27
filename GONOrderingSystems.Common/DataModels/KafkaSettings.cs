using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GONOrderingSystems.Common.DataModels
{
    [ProtoContract]
    public class KafkaSettings
    {
        [ProtoMember(1)]
        public string BrokerList { get; set; }
        [ProtoMember(2)]
        public string Topic { get; set; }
        [ProtoMember(3)]
        public string ConsumerGroupId { get; set; }
        [ProtoMember(4)]
        public string ProducerGroupId { get; set; }
        [ProtoMember(5)]
        public int Partition { get; set; }
    }

    
}
