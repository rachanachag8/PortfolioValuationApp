using System;

namespace PortfolioValuationApp.Infrastructure.Logging
{
    public class ConsoleLogger
    {
        //currently info has been commented 
        public void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[INFO] {message}");
            Console.ResetColor();
        }

        public void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARNING] {message}");
            Console.ResetColor();
        }

        public void Error(string message, Exception? ex = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {message}");
            if (ex != null)
                Console.WriteLine(ex.ToString());
            Console.ResetColor();
        }
    }
}
