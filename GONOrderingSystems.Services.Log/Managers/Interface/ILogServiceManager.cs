
using GONOrderingSystems.Common.DataModels;

namespace GONOrderingSystems.Services.Log.Interface
{
    public interface ILogServiceManager
    {
        bool PublishLog(LogItem logItem);
    }
}
