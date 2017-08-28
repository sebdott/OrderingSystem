using GONOrderingSystems.Common.DataModels;
using GONOrderingSystems.Logic.Managers.Interface;
using GONOrderingSystems.Common.Providers.Interface;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using GONOrderingSystems.Common.Common;

namespace GONOrderingSystems.Services.Order.Managers
{
    public class OrderServiceManager : IOrderServiceManager
    {
        private IDataProvider _dataProvider;

        public OrderServiceManager(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        public async Task<bool> CommitOrder(Common.DataModels.Order order)
        {
            order.Status = Status.Failure;

            await ProcessOrder(order);
            var saveResult = await _dataProvider.Update(order);

            return saveResult;
        }
        
        private async Task ProcessOrder(Common.DataModels.Order order)
        {
            order.ValueAValueB = order.ValueA + order.ValueB + "Done Processed";
            order.Status = Status.Success;
            await Task.Delay(100);
        }


    }
}
