using GONOrderingSystems.Common.DataModels;
using System.Threading.Tasks;

namespace GONOrderingSystems.Logic.Managers.Interface
{
    public interface IOrderManager
    {
        Task<bool> CreateOrder(Order order);
        Task<Order> GetOrder(string orderId);
        Task SendToCommit(Order order);
        Task<bool> OrderValidation(Order order, bool deadLineValidation);

    }
}
