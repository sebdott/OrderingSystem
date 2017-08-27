using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GONOrderingSystems.Logic.Managers.Interface;
using GONOrderingSystems.Common.Providers.Interface;
using GONOrderingSystems.Controllers.Interface;

namespace GONOrderingSystems.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DeadLineApiController : Controller , IDeadLineApi
    {
        ILogProvider _logProvider;
        IDataProvider _dataProvider;
        IDeadlineManager _deadlineManager;

        public DeadLineApiController( 
            IDataProvider dataProvider , 
            IDeadlineManager deadlineManager,
            ILogProvider logProvider
            )
        {
            _dataProvider = dataProvider;
            _deadlineManager = deadlineManager;

            _logProvider = logProvider;
        }

        [HttpGet("{eventId}")]
        public async Task<DateTime> GetDeadlineByEventId (string eventId)
        {
            return  await _deadlineManager.GetDeadLineByEventId(eventId);
        }
    }
}
