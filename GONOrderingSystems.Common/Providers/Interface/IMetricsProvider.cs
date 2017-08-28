using System.Threading.Tasks;

namespace GONOrderingSystems.Common.Providers.Interface
{
    public interface IMetricsProvider
    {
        void CounterIncrement(string MetricType);
        Task RestCounterIncrement(string url, string MetricType);
    }
}
