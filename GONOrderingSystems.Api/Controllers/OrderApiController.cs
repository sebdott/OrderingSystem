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
        public async Task<IActionResult> SubmitOrder([FromBody]SubmitOrderDto submitOrderDto)
        {
            try
            {

                _prometheusProvider.CounterIncrement(MetricCounter.OrderRequestCounter);

                var order = Mapper.Map<Order>(submitOrderDto);
                order.OrderSubmitTime = DateTime.Now;
                
                if (!await _orderManager.OrderValidation(order, true))
                {
                    _prometheusProvider.CounterIncrement(MetricCounter.FailedRequestCounter);
                    _prometheusProvider.CounterIncrement(MetricCounter.FailedValidationCounter);

                    return NotFound("Warning: Order Validation Fail");
                }

                await _orderManager.CreateOrder(order);

                if (!string.IsNullOrEmpty(order.OrderId))
                {
                   await _orderManager.SendToCommit(order);
                }
                else
                {
                    _prometheusProvider.CounterIncrement(MetricCounter.FailedRequestCounter);
                    return NotFound("Warning: Order Creation Fail");
                }

                return Ok(order.OrderId);
             
            }
            catch (Exception ex)
            {
                _prometheusProvider.CounterIncrement(MetricCounter.FailedRequestCounter);
                _prometheusProvider.CounterIncrement(MetricCounter.ExceptionCounter);
                _logProvider.PublishError("SubmitOrder([FromBody]OrderDto order)", "Order Fail" + JsonConvert.SerializeObject(submitOrderDto), ex);
                return NotFound("Warning: Error submit order, please contact admin person");
            }
        }        
    }
}
