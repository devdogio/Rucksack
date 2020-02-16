using System;

namespace Devdog.Rucksack
{
    public interface ILogger
    {
        void LogVerbose(string msg, object instance = null);
        void Log(string msg, object instance = null);
        void Warning(string msg, object instance = null);
        void Error(string msg, object instance = null);
        void Error(string msg, Error error, object instance = null);
        void Error(Exception exception, object instance = null);
    }
}