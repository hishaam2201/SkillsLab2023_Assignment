using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillsLab2023_Assignment.CustomExceptionHandler.AppLogger
{
    public interface ILogger
    {
        void Log(string message);
        void LogError(Exception exception);
    }
}
