using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DoubleBufferedQueue.v1
{
    /// <summary>
    /// 玩具容器
    /// </summary>
    public class ToyContainer
    {
        public static ConcurrentQueue<Toy> constainer = new ConcurrentQueue<Toy>();
    }
}
