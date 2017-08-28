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
        public string OrderTopic { get; set; }
        [ProtoMember(3)]
        public string LogTopic { get; set; }
        [ProtoMember(4)]
        public string ConsumerGroupId { get; set; }
        [ProtoMember(5)]
        public string ProducerGroupId { get; set; }
        [ProtoMember(6)]
        public int Partition { get; set; }
    }

    
}
