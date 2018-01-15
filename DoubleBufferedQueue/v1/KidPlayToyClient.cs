using System;
using System.Collections.Generic;
using System.Text;

namespace DoubleBufferedQueue.v1
{
    /// <summary>
    /// 孩子玩玩具客户端
    /// </summary>
    public class KidPlayToyClient
    {
        public static void Play()
        {
            var factory = new ToyFactory();
            factory.RunAsync();

            var kid = new Kid();
            kid.RunAsync();
        }
    }
}
