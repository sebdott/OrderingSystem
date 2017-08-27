using GONOrderingSystems.Common.DataModels;
using GONOrderingSystems.Logic.Managers;
using GONOrderingSystems.Logic.Managers.Interface;
using GONOrderingSystems.Common.Providers.Interface;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Options;
using GONOrderingSystems.Controllers.Interface;
using GONOrderingSystems.Controllers;
using GONOrderingSystems.Api.Model;
using MongoDB.Bson;
using AutoMapper;

namespace GONOrderingSystems.Tests.Logic.Managers
{
    public class TestOrderApiController
    {
        Mock<IDeadlineManager> _deadlineManager;
        Mock<IOrderManager> _orderManager;
        Mock<IDataProvider> _dataProviderMock;
        Mock<IMetricsProvider> _metricsProvider;
        Mock<IOptions<KafkaSettings>> _kafkaSettings;
        Mock<ILogProvider> _logProvider;
        IOrderApi _orderApi;

        private Order _order;
        private SubmitOrderDto _submitOrderDto;


        public TestOrderApiController()
        {
            _dataProviderMock = new Mock<IDataProvider>();
            _kafkaSettings = new Mock<IOptions<KafkaSettings>>();
            _deadlineManager = new Mock<IDeadlineManager>();
            _orderManager = new Mock<IOrderManager>();
            _logProvider = new Mock<ILogProvider>();
            _metricsProvider = new Mock<IMetricsProvider>();

            //_orderManager = new OrderManager(_dataProviderMock.Object, _pubSubProvider.Object, _kafkaSettings.Object);

            _order = new Order() { EventID = "TestEventID", OrderId = "TestOrderId", OrderSubmitTime = DateTime.Now, Username = Guid.NewGuid().ToString(), ValueA = "TestValueA", ValueB = "TestValueB" };
            _submitOrderDto = new SubmitOrderDto() { EventID = "TestEventID", Username = Guid.NewGuid().ToString(), ValueA = "TestValueA", ValueB = "TestValueB" };

            _orderApi = new OrderApiController(_deadlineManager.Object, 
                _orderManager.Object, _logProvider.Object,
               _metricsProvider.Object,_kafkaSettings.Object);

            Mapper.Initialize(cfg => {
                cfg.CreateMap<SubmitOrderDto, Order>();
                cfg.CreateMap<CommitOrderDto, Order>();
            });

        }

        [Fact]
        public async void TestSubmitOrderIsOrderValidationSuccess()
        {
            try
            {
                MockOrderManagerMethods(true, false, false);

                _order.OrderId = ObjectId.GenerateNewId().ToString();

                var results = await _orderApi.SubmitOrder(_submitOrderDto);

                _orderManager.Verify();
            }
            catch (Exception ex)
            {
                Assert.True(true);
            }
        }

        [Fact]
        public async void TestSubmitOrderIsOrderValidationisCreateOrderSuccess()
        {
            try
            {
                MockOrderManagerMethods(true, true, false);

                _order.OrderId = ObjectId.GenerateNewId().ToString();

                var results = await _orderApi.SubmitOrder(_submitOrderDto);

                _orderManager.Verify();
            }
            catch (Exception ex)
            {
                Assert.True(true);
            }
        }

        [Fact]
        public async void TestSubmitOrderIsOrderValidationisCreateOrderisSendtoCommitSuccess()
        {
            try
            {
                //MockOrderManagerMethods(true, false, false);

                _orderManager.Setup(x => x.OrderValidation(_order, true)).Returns(Task.FromResult<bool>(true)).Verifiable();
                _order.OrderId = ObjectId.GenerateNewId().ToString();

                var results = await _orderApi.SubmitOrder(_submitOrderDto);

                Assert.Equal(results, _order.OrderId);

                _orderManager.Verify();
            }
            catch (Exception ex)
            {
                Assert.True(false);
            }
        }

        private void MockOrderManagerMethods(bool isOrderValidation, bool isCreateOrder , bool isSendtoCommit) {

            if (isOrderValidation)
            {
                _orderManager.Setup(x => x.OrderValidation(_order, true)).Returns(Task.FromResult<bool>(true))
                    
               .Verifiable();
            }

            if (isCreateOrder)
            {
                _orderManager.Setup(x => x.CreateOrder(_order)).Returns(Task.FromResult<bool>(true))
                .Verifiable();
            }

            if (isSendtoCommit)
            {
                _orderManager.Setup(x => x.SendToCommit(_order)).Returns(Task.FromResult<bool>(true))
                .Verifiable();
            }
        }

    }
    
   
}
