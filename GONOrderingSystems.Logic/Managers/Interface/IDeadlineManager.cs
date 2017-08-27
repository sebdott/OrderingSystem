using System;
using System.Threading.Tasks;

namespace GONOrderingSystems.Logic.Managers.Interface
{
    public interface IDeadlineManager
    {
        Task<DateTime> GetDeadLineByEventId(string eventId);
    }
}
