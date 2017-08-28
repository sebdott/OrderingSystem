using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Prometheus.Client;
using Prometheus.Client.Collectors;
using GONOrderingSystems.Common.Providers.Interface;
using GONOrderingSystems.Api.Controllers.Interface;
using GONOrderingSystems.Common.Common;
using System.Threading.Tasks;

namespace GONOrderingSystems.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MetricsController : Controller, IMetricsApi
    {
        IMetricsProvider _metricsProvider;
        ILogProvider _logProvider;
        public MetricsController(IMetricsProvider metricsProvider, ILogProvider logProvider)
        {
            _metricsProvider = metricsProvider;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var registry = CollectorRegistry.Instance;
            var acceptHeaders = Request.Headers["Accept"];
            var contentType = ScrapeHandler.GetContentType(acceptHeaders);
            Response.ContentType = contentType;
            string content;

            using (var outputStream = new MemoryStream())
            {
                var collected = registry.CollectAll();
                ScrapeHandler.ProcessScrapeRequest(collected, contentType, outputStream);
                content = Encoding.UTF8.GetString(outputStream.ToArray());
            }

            return Ok(content);
        }

        [HttpPost]
        public void CounterIncrease([FromBody]string metricType)
        {
            _metricsProvider.CounterIncrement(metricType);

        }


    }
}
