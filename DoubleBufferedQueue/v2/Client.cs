using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DoubleBufferedQueue.v2
{
    public class Client
    {
        public static void Test()
        {

        }

        private void WriteToConsole<TModel>(Queue<TModel> queue)
        {
            while (queue.Count > 0)
            {
                TModel item = queue.Dequeue();
                Console.WriteLine($"{queue.GetHashCode()}\t{item}");

                Thread.Sleep(100);
            }
        }
    }
}
