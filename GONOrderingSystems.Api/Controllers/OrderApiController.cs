using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GONOrderingSystems.Logic.Managers.Interface;
using GONOrderingSystems.Common.DataModels;
using GONOrderingSystems.Api.Model;
using AutoMapper;
using GONOrderingSystems.Common.Providers.Interface;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using GONOrderingSystems.Controllers.Interface;
using GONOrderingSystems.Common.Common;

namespace GONOrderingSystems.Controllers
{
    [Route("api/[controller]/[action]")]
    public class OrderApiController : Controller, IOrderApi
    {
        IDeadlineManager _deadlineManager;
        IOrderManager _orderManager;
        ILogProvider _logProvider;
        IMetricsProvider _prometheusProvider;

        private IOptions<KafkaSettings> _kafkaSettings;

        public OrderApiController(
            IDeadlineManager deadlineManager, IOrderManager orderManager,
            ILogProvider logProvider,
            IMetricsProvider prometheusProvider,
            IOptions<KafkaSettings> kafkaSettings)
        {
            _deadlineManager = deadlineManager;
            _orderManager = orderManager;
            _logProvider = logProvider;
            _kafkaSettings = kafkaSettings;
            _prometheusProvider = prometheusProvider;
        }

        [HttpGet("{orderId}")]
        public async Task<Order> FetchOrder(string orderId)
        {
            try
            {
                var order = await _orderManager.GetOrder(orderId);

                return order;
            }
            catch (Exception ex)
            {
                _prometheusProvider.CounterIncrement(MetricCounter.ExceptionCounter);
                _logProvider.PublishError("FetchOrder(string orderId)", "Fetch Order Fail " + orderId, ex);
                return new Order();
            }
        }

        [HttpPost]
        public async Task<string> SubmitOrder([FromBody]SubmitOrderDto submitOrderDto)
        {
            _prometheusProvider.CounterIncrement(MetricCounter.OrderRequestCounter);

            try
            {
                var order = Mapper.Map<Order>(submitOrderDto);
                order.OrderSubmitTime = DateTime.Now;
                
                if (!await _orderManager.OrderValidation(order, true))
                {
                    _prometheusProvider.CounterIncrement(MetricCounter.FailedValidationCounter);
                    
                    throw new Exception("Warning: Order Validation Fail");
                }

                await _orderManager.CreateOrder(order);

                if (!string.IsNullOrEmpty(order.OrderId))
                {
                   await _orderManager.SendToCommit(order);
                }
                else
                {
                    throw new Exception("Warning: Order Creation Fail");
                }

                return order.OrderId;
             
            }
            catch (Exception ex)
            {
                _prometheusProvider.CounterIncrement(MetricCounter.ExceptionCounter);
                _prometheusProvider.CounterIncrement(MetricCounter.FailedRequestCounter);
                _logProvider.PublishError("SubmitOrder([FromBody]OrderDto order)", "Order Fail" + JsonConvert.SerializeObject(submitOrderDto), ex);
                throw new Exception("Warning: Error submit order, please contact admin person");
            }
        }

        [HttpPost]
        public async Task<string> CommitOrder([FromBody]CommitOrderDto commitOrderDto)
        {
            try
            {
                var order = Mapper.Map<Order>(commitOrderDto);
                if (!await _orderManager.OrderValidation(order, false))
                {
                    throw new Exception("Warning: Order Validation Fail");
                }

                var orderToCommit = Mapper.Map<Order>(commitOrderDto);
                
                var success = await _orderManager.CommitOrder(orderToCommit);

                if (success && !string.IsNullOrEmpty(orderToCommit.OrderId))
                {
                    _prometheusProvider.CounterIncrement(MetricCounter.SuccessOrderCounter);
                    _logProvider.PublishInfo(commitOrderDto.EventID, "Order Success -" + JsonConvert.SerializeObject(commitOrderDto));

                    return orderToCommit.OrderId;
                }
                else
                {
                    _logProvider.PublishInfo("CommitOrder([FromBody]CommitOrderDto commitOrderDto)", "Fail to Commit " + JsonConvert.SerializeObject(commitOrderDto));
                    _prometheusProvider.CounterIncrement(MetricCounter.OrderRequestCounter);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _prometheusProvider.CounterIncrement(MetricCounter.ExceptionCounter);
                _prometheusProvider.CounterIncrement(MetricCounter.FailedCommitCounter);
                _logProvider.PublishError("CommitOrder([FromBody]OrderDto order)", "Order Fail" + JsonConvert.SerializeObject(commitOrderDto), ex);
                throw new Exception("Warning: Error commit order, please contact admin person");
            }
        }
        
        
    }
}
