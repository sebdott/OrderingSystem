using GONOrderingSystems.Common.DataModels;
using System.Threading.Tasks;

namespace GONOrderingSystems.Logic.Managers.Interface
{
    public interface IOrderServiceManager
    {
        Task<bool> CommitOrder(Order order);

    }
}
