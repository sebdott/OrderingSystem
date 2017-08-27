namespace GONOrderingSystems.Common.Providers.Interface
{
    public interface IMetricsProvider
    {
        void CounterIncrement(string MetricType);
    }
}
