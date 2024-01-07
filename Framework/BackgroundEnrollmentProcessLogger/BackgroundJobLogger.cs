using System;
using System.Diagnostics;
using System.IO;

namespace Framework.BackgroundEnrollmentProcessLogger
{
    public class BackgroundJobLogger : IBackgroundJobLogger
    {
        private readonly string _loggerFilePath;
        public BackgroundJobLogger(string loggerFilePath = "BackgroundJobLogs\\backgroundLog.txt")
        {
            string rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _loggerFilePath = Path.Combine(rootDirectory, loggerFilePath);
        }

        public void Log(string message)
        {
            string backgroundLogMessage = $"{message}";

            try
            {
                if (!File.Exists(_loggerFilePath))
                {
                    File.Create(_loggerFilePath);
                }
                using (StreamWriter writer = File.AppendText(_loggerFilePath))
                {
                    writer.WriteLine(backgroundLogMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }

        public void LogInformation(string message)
        {
            string fullMessage = "---------------------------------------------------------";
            fullMessage += Environment.NewLine + $"Timestamp: {DateTime.Now}";
            fullMessage += Environment.NewLine + $"{message}";
            fullMessage += Environment.NewLine + "\"---------------------------------------------------------\";";
            Log(fullMessage);
        }
    }
}
