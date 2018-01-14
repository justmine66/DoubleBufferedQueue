using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DoubleBufferedQueue
{
    /// <summary>
    /// ManualResetEvent测试类
    /// </summary>
    public class ManualResetEventTest
    {
        //无信号：阻塞状态
        private static ManualResetEvent mre = new ManualResetEvent(false);

        public static void EastWindComming()
        {
            EastWind eastWind = new EastWind(mre);
            Thread thread = new Thread(new ThreadStart(eastWind.WindComming));
            thread.Start();

            mre.WaitOne();//万事俱备只欠东风，事情卡在这里了，在东风来之前，诸葛亮没有进攻
            Console.WriteLine("诸葛亮大吼：东风来了，可以进攻了，满载燃料的大船接着东风冲向曹操的战船");
        }

        /// <summary>
        /// 东风
        /// </summary>
        public class EastWind
        {
            ManualResetEvent _mre;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="mre"></param>
            public EastWind(ManualResetEvent mre)
            {
                _mre = mre;
            }

            /// <summary>
            /// 风正在吹过来
            /// </summary>
            public void WindComming()
            {
                Console.WriteLine("东风正在吹过来");
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("东风吹啊吹，越来越近了...");
                }
                Console.WriteLine("东风终于到了");

                //通知诸葛亮东风已到，可以进攻了，通知阻塞的线程可以继续执行了
                _mre.Set();
            }
        }
    }
}
