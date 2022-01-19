using System;

namespace DependencyInjection
{
    public class EmailLog : ILog, IConsole
    {
        const string adminEmail = "ovidiu.gabor@gmail.com";
        public void Write(string message)
        {
            Console.WriteLine($"Email send to {adminEmail} : {message}");
        }
    }
}
