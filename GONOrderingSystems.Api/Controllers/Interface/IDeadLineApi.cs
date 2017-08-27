using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace GONOrderingSystems.Controllers.Interface
{
    public interface IDeadLineApi
    {
        Task<DateTime> GetDeadlineByEventId(string eventId);
    }
}
