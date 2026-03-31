using System.Runtime.CompilerServices;

namespace TaskForce.AP.Client.Core
{
    public interface ILogger
    {
        void Info(string msg, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0);
        void Warn(string msg, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0);
        void Fatal(string msg, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0);
    }
}
