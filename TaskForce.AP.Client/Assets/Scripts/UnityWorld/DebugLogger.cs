using System.Runtime.CompilerServices;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    public class DebugLogger : Core.ILogger
    {
        private readonly IApplication _application;

        public DebugLogger(IApplication application)
        {
            _application = application;
        }

        private string FormatLogMessage(string msg, string filePath, int lineNumber)
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            return $"[{fileName}:{lineNumber}] {msg}";
        }

        public void Info(
            string msg,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Debug.Log(msg);
        }

        public void Warn(
            string msg,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Debug.LogWarning(FormatLogMessage(msg, filePath, lineNumber));
        }

        public void Fatal(
            string msg,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            string formattedMsg = FormatLogMessage(msg, filePath, lineNumber);

            Debug.LogError($"[FATAL ERROR] {formattedMsg}");

            _application.Shutdown();
        }
    }
}
