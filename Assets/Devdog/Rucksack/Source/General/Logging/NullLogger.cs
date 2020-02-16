using System;

namespace Devdog.Rucksack
{
    public sealed class NullLogger : ILogger
    {
        public NullLogger()
        { }

        public void LogVerbose(string msg, object instance = null)
        { }

        public void Log(string msg, object instance = null)
        { }

        public void Warning(string msg, object instance = null)
        { }

        public void Error(string msg, object instance = null)
        { }
        
        public void Error(string msg, Error error, object instance = null)
        { }

        public void Error(Exception exception, object instance = null)
        { }
    }
}