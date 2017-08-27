using System;
using System.IO;

namespace GONOrderingSystems.Common.Common
{
    public class Serializer
    {
        public static string Serialize<T>(T record) where T : class
        {
            if (null == record) return null;

            try
            {
                using (var stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(stream, record);
                    return Compressor.Compress(Convert.ToBase64String(stream.ToArray()));
                }
            }
            catch
            {
                return null;
            }
        }

        public static T Deserialize<T>(string data) where T : class
        {
            if (null == data) return null;

            try
            {
                var deCompressedData = Compressor.Decompress(data);
                byte[] compressed = Convert.FromBase64String(deCompressedData);
                using (var stream = new MemoryStream(compressed))
                {
                    return ProtoBuf.Serializer.Deserialize<T>(stream);
                }
            }
            catch
            {
                return null;
            }
        }

    }
}
