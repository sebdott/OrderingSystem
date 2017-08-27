using ProtoBuf;
using System;

namespace GONOrderingSystems.Api.Model
{
    [ProtoContract]
    public class CommitOrderDto
    {
        [ProtoMember(1)]
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
    }
}
