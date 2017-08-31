using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Prometheus.Client;
using Prometheus.Client.Collectors;
using GONOrderingSystems.Common.Providers.Interface;
using GONOrderingSystems.Api.Controllers.Interface;

namespace GONOrderingSystems.Api.Controllers
{
    [Route("api/[controller]")]
    public class MetricsController : Controller, IMetricsApi
    {
        IMetricsProvider _metricsProvider;
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

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost, Route("[action]")]
        public void CounterIncrease([FromBody]string metricType)
        {
            _metricsProvider.CounterIncrement(metricType);

        }


    }
}
