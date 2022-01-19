using System;

namespace DependencyInjection
{
    public class ConsoleLog : ILog, IConsole
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
}
