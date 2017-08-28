using Microsoft.AspNetCore.Mvc;

namespace GONOrderingSystems.Api.Controllers.Interface
{
    public interface IMetricsApi
    {
        IActionResult Get();

        void CounterIncrease(string metricType);
    }
}
