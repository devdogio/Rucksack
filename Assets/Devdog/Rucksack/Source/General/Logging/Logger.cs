using System;

namespace Devdog.Rucksack
{
    public sealed class Logger : ILogger
    {
        private readonly string _category;
        public Logger(string category = "[LOGGER] ")
        {
            this._category = category;
        }

        public void LogVerbose(string msg, object instance = null)
        {
            Console.WriteLine(_category + msg, instance);
        }

        public void Log(string msg, object instance = null)
        {
            Console.WriteLine(_category + msg, instance);
        }

        public void Warning(string msg, object instance = null)
        {
            Console.WriteLine(_category + msg, instance);
        }

        public void Error(string msg, object instance = null)
        {
            Console.WriteLine("[ERROR] " + _category + msg, instance);
        }
        
        public void Error(string msg, Error error, object instance = null)
        {
            if (error != null)
            {
                Console.WriteLine("[ERROR] " + _category + msg + "\n" + error, instance);
            }
        }

        public void Error(Exception exception, object instance = null)
        {
            Console.WriteLine("[EXCEPTION] " + _category + exception.Message, instance);
        }
    }
}