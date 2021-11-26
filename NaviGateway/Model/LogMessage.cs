using System;

namespace NaviGateway.Model
{
    public enum LogMessageType
    {
        Info,
        Error
    }
    
    public class LogMessage
    {
        public string Id { get; set; }
            
        public DateTime LogCreatedAt { get; set; }
            
        public LogMessageType LogType { get; set; }
            
        public string LogService { get; set; } // such as AuthenticationService, like service identifier.
            
        public string Message { get; set; }
    }
}