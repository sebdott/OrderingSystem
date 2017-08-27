using System;
using System.Collections.Generic;
using System.Text;

namespace GONOrderingSystems.Common.Common
{
    public class Constant
    {
        public const string LoggingFormat = "{0} - {1}";
    }

    public class Status
    {
        public const string New = "New";
        public const string Success = "Success";
        public const string Failure = "Failure";
    }

    public class MetricCounter
    {
        public const string OrderRequestCounter = "GONOrderingSystems_Api_OrderRequestCounter";
        public const string FailedRequestCounter = "GONOrderingSystems_Api_FailedRequestCounter";
        public const string FailedValidationCounter = "GONOrderingSystems_Api_FailedValidationCounter";
        public const string SuccessOrderCounter = "GONOrderingSystems_Api_SuccessOrderCounter";
        public const string ExceptionCounter = "GONOrderingSystems_Api_ExceptionCounter";
        public const string FailedCommitCounter = "GONOrderingSystems_Api_FailedCommitCounter";

    }
}
