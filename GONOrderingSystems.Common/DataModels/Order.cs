
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GONOrderingSystems.Common.DataModels
{
    [ProtoContract]
    public class Order
    {
        public Order()
        {
            OrderId = string.Empty;
            Username = string.Empty;
            EventID = string.Empty;
            OrderSubmitTime = new DateTime();
            ValueA = string.Empty;
            ValueB = string.Empty;
            ValueAValueB = string.Empty;
            Status = string.Empty;
        }

        [ProtoMember(1)]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string OrderId { get; set; }
        [ProtoMember(2)]
        public string Username { get; set; }
        [ProtoMember(3)]
        public string EventID { get; set; }
        [ProtoMember(4)]
        public DateTime OrderSubmitTime { get; set; }
        [ProtoMember(5)]
        public string ValueA { get; set; }
        [ProtoMember(6)]
        public string ValueB { get; set; }
        [ProtoMember(7)]
        public string Status { get; set; }
        [ProtoMember(8)]
        public string ValueAValueB { get; set; }
    }
}
