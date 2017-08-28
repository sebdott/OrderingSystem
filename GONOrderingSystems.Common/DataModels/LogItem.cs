
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GONOrderingSystems.Common.DataModels
{
    [ProtoContract]
    public class LogItem
    {
        [ProtoMember(1)]
        public string Identifier { get; set; }
        [ProtoMember(2)]
        public string Message { get; set; }
        [ProtoMember(3)]
        public string Exception { get; set; }
        [ProtoMember(4)]
        public string Type { get; set; }
    }
}
