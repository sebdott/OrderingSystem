using GONOrderingSystems.Common.Common;
using Xunit;

namespace GONOrderingSystems.Tests.Common
{
    public class TestCompressor
    {
        [Theory]
        [InlineData("TESTSADASDAS")]
        [InlineData("@#$@#$@#$!@#!@#!@")]
        [InlineData("@#$2+62+62+626+@#$@#$")]
        public void TestCompressDecompressString(string data)
        {
            var compressedData = Compressor.Compress(data);

            var decompressedData = Compressor.Decompress(compressedData);

            Assert.Equal(data, decompressedData);
        }
    }
    
   
}
