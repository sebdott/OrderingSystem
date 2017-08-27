using GONOrderingSystems.Common.DataModels;
using GONOrderingSystems.Logic.Managers.Interface;
using GONOrderingSystems.Common.Providers.Interface;
using MongoDB.Bson;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using GONOrderingSystems.Common.Common;
using System;

namespace GONOrderingSystems.Logic.Managers
{
    public class OrderManager : IOrderManager
    {
        private IDataProvider _dataProvider;
        private IPubSubProvider _pubSubProvider;
        private IOptions<KafkaSettings> _kafkaSettings;
        private IDeadlineManager _deadlineManager;

        public OrderManager(IDataProvider dataProvider,
            IPubSubProvider pubSubProvider,
            IOptions<KafkaSettings> kafkaSettings,
            IDeadlineManager deadlineManager
            )
        {
            _pubSubProvider = pubSubProvider;
            _dataProvider = dataProvider;
            _kafkaSettings = kafkaSettings;
            _deadlineManager = deadlineManager;
        }

        public async Task<bool> CreateOrder(Order order)
        {
            order.OrderId = ObjectId.GenerateNewId().ToString();
            order.Status = Status.New;
            var saveResult = await _dataProvider.Save(order);

            return saveResult;
        }

        public async Task<bool> CommitOrder(Order order)
        {
            order.Status = Status.Failure;

            await ProcessOrder(order);
            var saveResult = await _dataProvider.Update(order);

            return saveResult;
        }

        public async Task<Order> GetOrder(string orderId)
        {
            return await _dataProvider.Get(orderId);
        }
        public async Task SendToCommit(Order order)
        {
            using (var producer = _pubSubProvider.GetPublishProvider(_kafkaSettings.Value.BrokerList,
                                      _kafkaSettings.Value.ProducerGroupId))
            {
                producer.ProduceAsync(_kafkaSettings.Value.Topic, null, Serializer.Serialize(order).ToString(), _kafkaSettings.Value.Partition);

                producer.Flush(100);
            }
        }

        public async Task<bool> OrderValidation(Order order, bool deadLineValidation)
        {
            try
            {
                var isValid = true;

                var deadLine = await _deadlineManager.GetDeadLineByEventId(order.EventID);

                if (string.IsNullOrWhiteSpace(order.Username) ||
                    string.IsNullOrWhiteSpace(order.EventID) ||
                    string.IsNullOrWhiteSpace(order.ValueA) ||
                    string.IsNullOrWhiteSpace(order.ValueB))
                {
                    isValid = false;
                }

                if (!deadLineValidation)
                {
                    return isValid;
                }

                if (deadLine < order.OrderSubmitTime)
                {
                    isValid = false;
                }

                return isValid;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task ProcessOrder(Order order)
        {
            order.ValueAValueB = order.ValueA + order.ValueB + "Done Processed";
            order.Status = Status.Success;
            await Task.Delay(100);
        }


    }
}
