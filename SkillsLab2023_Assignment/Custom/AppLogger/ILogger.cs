using System;

namespace SkillsLab2023_Assignment.AppLogger
{
    public interface ILogger
    {
        void Log(string message);
        void LogError(Exception exception);
    }
}
