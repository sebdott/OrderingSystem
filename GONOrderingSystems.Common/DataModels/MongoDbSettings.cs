using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GONOrderingSystems.Common.DataModels
{
    [ProtoContract]
    public class MongoDbSettings
    {
        [ProtoMember(1)]
        public string Host { get; set; }
        [ProtoMember(2)]
        public string Database { get; set; }
        [ProtoMember(3)]
        public string OrderCollection { get; set; }
    }


}
