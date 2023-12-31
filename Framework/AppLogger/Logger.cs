﻿using System;
using System.Diagnostics;
using System.IO;

namespace Framework.AppLogger
{
    public class Logger : ILogger
    {
        private readonly string _loggerFilePath;
        public Logger(string loggerFilePath = "ErrorLogs\\log.txt")
        {
            string rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _loggerFilePath = Path.Combine(rootDirectory, loggerFilePath);
        }

        public void Log(string message)
        {
            string errorLogMessage = $"{message}";

            try
            {
                if (!File.Exists(_loggerFilePath))
                {
                    File.Create(_loggerFilePath);
                }
                using (StreamWriter writer = File.AppendText(_loggerFilePath))
                {
                    writer.WriteLine(errorLogMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }

        public void LogError(Exception exception, Guid errorGuid)
        {
            string fullMessage = "---------------------------------------------------------";
            fullMessage += Environment.NewLine + $"Id: {errorGuid}";
            fullMessage += Environment.NewLine + $"Timestamp: {DateTime.Now}";
            fullMessage += Environment.NewLine + $"Exception Type: {this.GetType().FullName}";
            fullMessage += Environment.NewLine + $"Message: {exception.Message}";
            fullMessage += Environment.NewLine + $"Inner Exception: {exception.InnerException}";
            fullMessage += Environment.NewLine + $"Stack Trace: {exception.StackTrace}";
            fullMessage += Environment.NewLine + "\"---------------------------------------------------------\";";
            
            Log(fullMessage);
        }
    }
}