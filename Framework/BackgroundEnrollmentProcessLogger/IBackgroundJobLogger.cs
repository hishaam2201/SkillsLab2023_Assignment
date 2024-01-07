
namespace Framework.BackgroundEnrollmentProcessLogger
{
    public interface IBackgroundJobLogger
    {
        void Log(string message);
        void LogInformation(string message);
    }
}
