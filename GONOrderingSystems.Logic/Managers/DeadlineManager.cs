using GONOrderingSystems.Logic.Managers.Interface;
using System;
using System.Threading.Tasks;

namespace GONOrderingSystems.Logic.Managers
{
    public class DeadlineManager : IDeadlineManager
    {
        public Task<DateTime> GetDeadLineByEventId(string eventId)
        {
            var rnd = new Random();
            
            return Task.FromResult<DateTime>(DateTime.Now.AddSeconds(rnd.Next(-50, 100)));
        }
    }
}
