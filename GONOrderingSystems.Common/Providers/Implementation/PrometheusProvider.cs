using GONOrderingSystems.Common.Common;
using GONOrderingSystems.Common.Providers.Interface;
using Prometheus.Client;

namespace GONOrderingSystems.Common.Providers.Implementation
{
    public class PrometheusProvider : IMetricsProvider
    {
        private readonly Counter _exceptionCounter = Metrics.CreateCounter(MetricCounter.ExceptionCounter, "Exception Api Request Counter");
        private readonly Counter _orderRequestCounter = Metrics.CreateCounter(MetricCounter.OrderRequestCounter, "Api Request Counter");
        private readonly Counter _failedRequestCounter = Metrics.CreateCounter(MetricCounter.FailedRequestCounter, "Failed Api Request Counter");
        private readonly Counter _successOrderCounter = Metrics.CreateCounter(MetricCounter.SuccessOrderCounter, "Success Api Request Counter");
        private readonly Counter _failedValidationCounter = Metrics.CreateCounter(MetricCounter.FailedValidationCounter, "Failed Validation Api Request Counter");
        private readonly Counter _failedCommitCounter = Metrics.CreateCounter(MetricCounter.FailedCommitCounter, "Failed Commit Api Request Counter");

        public void CounterIncrement(string MetricType)
        {
            switch (MetricType)
            {
                case MetricCounter.FailedCommitCounter:
                    _failedCommitCounter.Inc();
                    break;
                case MetricCounter.ExceptionCounter:
                    _exceptionCounter.Inc();
                    break;
                case MetricCounter.SuccessOrderCounter:
                    _successOrderCounter.Inc();
                    break;
                case MetricCounter.FailedRequestCounter:
                    _failedRequestCounter.Inc();
                    break;
                case MetricCounter.OrderRequestCounter:
                    _orderRequestCounter.Inc();
                    break;
                case MetricCounter.FailedValidationCounter:
                    _failedValidationCounter.Inc();
                    break;
            }
        }
    }
}
