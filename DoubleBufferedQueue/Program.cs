using System;
using System.Threading;

namespace DoubleBufferedQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            Client.RunAsync(1000);

            Console.Read();
        }
    }
}
