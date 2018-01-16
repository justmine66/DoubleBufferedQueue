using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace DoubleBufferedQueue.v2
{
    public class ClientV2
    {
        static DoubleBufferingQueue<string> dbq = new DoubleBufferingQueue<string>();
        static Random random = new Random();

        public static void Run()
        {
            var watch = Stopwatch.StartNew();
            ProducerFunc(100000);
            ConsumerFunc();
            watch.Stop();
            Console.WriteLine($"耗时：{watch.Elapsed.TotalSeconds}秒");
        }

        public static void ProducerFunc(int count)
        {
            for (int i = 0; i < count; i++)
            {
                dbq.Enqueue(i.ToString());
                Console.WriteLine($"生产 \t{i}");
                //Thread.Sleep(random.Next(100, 1000));
            }
        }

        public static void ConsumerFunc()
        {
            while (true)
            {
                var item = dbq.Dequeue();
                if (item != null)
                {
                    Console.WriteLine($"消费 \t{item}");
                }
                //Thread.Sleep(random.Next(10, 1000));
            }
        }
    }
}
