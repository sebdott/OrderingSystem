namespace GONOrderingSystems.Common.Providers.Interface
{
    public interface ILogProvider
    {
        void PublishError(string eventID, string message, System.Exception exception);
        void PublishInfo(string eventID, string message);

    }
}
