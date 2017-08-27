using ProtoBuf;

namespace GONOrderingSystems.Api.Model
{
    [ProtoContract]
    public class SubmitOrderDto
    {
        [ProtoMember(1)]
        public string Username { get; set; }
        [ProtoMember(2)]
        public string EventID { get; set; }
        [ProtoMember(3)]
        public string ValueA { get; set; }
        [ProtoMember(4)]
        public string ValueB { get; set; }
    }
}
