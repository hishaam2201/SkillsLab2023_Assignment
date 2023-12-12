using System;

namespace Framework.AppLogger
{
    public interface ILogger
    {
        void Log(string message);
        void LogError(Exception exception);
    }
}
