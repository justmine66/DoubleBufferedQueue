using System;
using System.Threading;

namespace DoubleBufferedQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbq = new DoubleBufferedQueue<string>();

            for (int i = 0; i < 1000; i++)
            {
                dbq.Equeue(i.ToString());
                Thread.Sleep(100);
            }

            Console.Read();
        }
    }
}
