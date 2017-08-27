using GONOrderingSystems.Common.DataModels;
using GONOrderingSystems.Logic.Managers;
using GONOrderingSystems.Logic.Managers.Interface;
using GONOrderingSystems.Common.Providers.Interface;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Options;

namespace GONOrderingSystems.Tests.Logic.Managers
{
    public class TestIOrderManager
    {
        private IOrderManager _orderManager;
        private Order _order;
        private Mock<IDataProvider> _dataProviderMock;
        private Mock<IPubSubProvider> _pubSubProvider;
        private Mock<IOptions<KafkaSettings>> _kafkaSettings;
        private Mock<IDeadlineManager> _deadlineManager;

        public TestIOrderManager()
        {
            _dataProviderMock = new Mock<IDataProvider>();
            _kafkaSettings = new Mock<IOptions<KafkaSettings>>();
            _pubSubProvider = new Mock<IPubSubProvider>();
            _deadlineManager = new Mock<IDeadlineManager>();

            _dataProviderMock = new Mock<IDataProvider>();
            _orderManager = new OrderManager(_dataProviderMock.Object, _pubSubProvider.Object, _kafkaSettings.Object, _deadlineManager.Object);
            _order = new Order() { EventID = "TestEventID", OrderId = "TestOrderId", OrderSubmitTime = DateTime.Now, Username = Guid.NewGuid().ToString(), ValueA = "TestValueA", ValueB = "TestValueB" };
        }

        [Fact]
        public async void TestCreateOrderSuccess()
        {
            _dataProviderMock.Setup(x => x.Save(_order)).Returns(Task.FromResult<bool>(true))
                .Verifiable();
            try
            {
                var results = await _orderManager.CreateOrder(_order);

                Assert.True(results);

                _dataProviderMock.Verify();
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }
        [Fact]
        public async void TestCreateOrderFail()
        {
            _dataProviderMock.Setup(x => x.Save(_order)).Returns(Task.FromResult<bool>(false))
                .Verifiable();

            try
            {
                var results = await _orderManager.CreateOrder(_order);

                Assert.False(results);

                _dataProviderMock.Verify();
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public async void TestCommitOrderSuccess()
        {
            _dataProviderMock.Setup(x => x.Update(_order)).Returns(Task.FromResult<bool>(true))
                .Verifiable();
            try
            {
                var results = await _orderManager.CommitOrder(_order);

                Assert.True(results);

                _dataProviderMock.Verify();
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }
        [Fact]
        public async void TestCommitOrderFail()
        {
            _dataProviderMock.Setup(x => x.Update(_order)).Returns(Task.FromResult<bool>(false))
                .Verifiable();

            try
            {
                var results = await _orderManager.CommitOrder(_order);

                Assert.False(results);

                _dataProviderMock.Verify();
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

        [Fact]
        public async void TestGetOrderSuccess()
        {
            _dataProviderMock.Setup(x => x.Get(_order.EventID)).Returns(Task.FromResult<Order>(_order))
                .Verifiable();

            try
            {
                var returnOrder = await _orderManager.GetOrder(_order.EventID);

                Assert.Equal(_order.EventID, returnOrder.EventID);

                _dataProviderMock.Verify();
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }



        [Fact]
        public async void TestGetOrderFail()
        {
            var failOrder = new Order();

            _dataProviderMock.Setup(x => x.Get(_order.EventID)).Returns(Task.FromResult<Order>(failOrder))
                .Verifiable();

            try
            {
                var returnOrder = await _orderManager.GetOrder(_order.EventID);

                Assert.NotEqual(_order.EventID, returnOrder.EventID);

                _dataProviderMock.Verify();
            }
            catch (Exception)
            {
                Assert.True(false);
            }
        }

    }
    
   
}
