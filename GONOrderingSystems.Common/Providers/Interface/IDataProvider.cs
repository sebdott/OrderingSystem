using GONOrderingSystems.Common.DataModels;
using System.Threading.Tasks;

namespace GONOrderingSystems.Common.Providers.Interface
{
    public interface IDataProvider
    {
        Task<bool> Save(Order order);
        Task<Order> Get(string ID);
        Task<bool> Update(Order order);
    }
}
