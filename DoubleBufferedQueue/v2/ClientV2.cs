using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DoubleBufferedQueue.v2
{
    public class ClientV2
    {
        static DoubleBufferingQueue<string> dbq = new DoubleBufferingQueue<string>();
        static Random random = new Random();

        public static void Run()
        {
            ProducerFuncAsync(100);
            ConsumerFuncAsync();
        }

        public static Task ProducerFuncAsync(int count)
        {
            return Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    dbq.Enqueue(i.ToString());
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"生产\t{i}");
                    Task.Delay(random.Next(100, 1000));
                }
            });
        }

        public static Task ConsumerFuncAsync()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    var item = dbq.Dequeue();
                    if (item != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\t消费\t{item}");
                    }
                    Task.Delay(random.Next(10, 1000));
                }
            });
        }
    }
}
