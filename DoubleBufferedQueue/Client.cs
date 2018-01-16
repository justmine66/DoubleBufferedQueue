using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DoubleBufferedQueue
{
    public class Client
    {
        static DoubleBufferingQueue<User> dbq = new DoubleBufferingQueue<User>();
        static Random random = new Random();

        public static void RunAsync(int count)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            ProducerFuncAsync(count)
                .ConfigureAwait(false);
            ConsumerFuncAsync(count)
                .ContinueWith(task =>
                {
                    stopwatch.Stop();
                    FluentConsole.Red.Line($"\n耗时：{stopwatch.Elapsed.TotalSeconds}秒\n");
                })
                .ConfigureAwait(false);
        }

        private static Task ProducerFuncAsync(int count)
        {
            return Task.Run(() =>
            {
                ProducerFunc(count);
            });
        }

        private static void ProducerFunc(int count)
        {
            for (int i = 0; i < count; i++)
            {
                User user = new User()
                {
                    Index = i,
                    Mobile = i.ToString().PadLeft(11, '0'),
                    Pwd = i.ToString().PadLeft(8, '8')
                };
                dbq.Enqueue(user);
                FluentConsole.White.Line($"生产\t{user.ToString()}");
            }
        }

        private static Task ConsumerFuncAsync(int max)
        {
            return Task.Run(() =>
            {
                ConsumerFunc(max);
            });
        }

        private static Task ConsumerFunc(int max)
        {
            while (true)
            {
                var item = dbq.Dequeue();
                if (item != null)
                {
                    FluentConsole.Green.Line($"\t消费\t{item.ToString()}");
                    if (item.Index == max - 1)
                    {
                        FluentConsole.DarkGreen.Line($"\n总消费个数：{++item.Index}\n");
                        return Task.CompletedTask;
                    }
                }
            }
        }
    }

    public class User
    {
        public int Index { get; set; }
        public string Mobile { get; set; }
        public string Pwd { get; set; }

        public override string ToString()
        {
            return $"{Index},{Mobile},{Pwd}";
        }
    }
}
