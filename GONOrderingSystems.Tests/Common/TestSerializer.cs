using GONOrderingSystems.Common.Common;
using GONOrderingSystems.Common.DataModels;
using System;
using Xunit;

namespace GONOrderingSystems.Tests.Common
{
    public class TestSerializer
    {
        [Fact]
        public void TestSerializerObject()
        {
            var order = new Order() { EventID = "TestEventID4", OrderId = "TestOrderId4", OrderSubmitTime = DateTime.Now, Username = Guid.NewGuid().ToString(), ValueA = "TestValueA4", ValueB = "TestValueB4" };

            var finalResult = TestSerializedDeserliazedData(order);

            Assert.Equal(finalResult.OrderId, order.OrderId);
            Assert.Equal(finalResult.EventID, order.EventID);

            var kafkaSettings = new KafkaSettings() { BrokerList = "TestBrokerList1", ConsumerGroupId = "TestConsumerGroupId", ProducerGroupId = "TestProducerGroupId", LogTopic = "TestTopic" };

            var finalResult2 = TestSerializedDeserliazedData(kafkaSettings);

            Assert.Equal(finalResult2.ProducerGroupId, kafkaSettings.ProducerGroupId);
            Assert.Equal(finalResult2.BrokerList, kafkaSettings.BrokerList);


            var logItem = new LogItem() { Identifier = "TestIdentifier", Message = "TestMessage", Exception = "TestException", Type = "TestType" };

            var finalResult3 = TestSerializedDeserliazedData(logItem);

            Assert.Equal(finalResult3.Identifier, logItem.Identifier);
            Assert.Equal(finalResult3.Message, logItem.Message);
        }

        [Theory]
        [InlineData("TESTSADASDAS")]
        [InlineData("@#$@#$@#$!@#!@#!@")]
        [InlineData("@#$2+62+62+626+@#$@#$")]
        public void TestSerializerString(string data)
        {
            var finalResult = TestSerializedDeserliazedData(data);

            Assert.Equal(finalResult, data);
        }

        private T TestSerializedDeserliazedData<T>(T data) where T : class
        {
            string serializedData = Serializer.Serialize<T>(data);

            var deSerializedData = Serializer.Deserialize<T>(serializedData);

            return deSerializedData;
        }

    }
}
