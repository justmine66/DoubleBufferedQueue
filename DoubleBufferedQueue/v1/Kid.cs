using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DoubleBufferedQueue.v1
{
    /// <summary>
    /// 小孩
    /// 不停从容器中取出玩具
    /// </summary>
    public class Kid
    {
        object sysnObj = new object();
        public Task RunAsync()
        {
            return Task.Run(() =>
            {
                int index = 0;
                DateTime removeBegin = DateTime.Now;
                while (true)
                {
                    lock (sysnObj)
                    {
                        if (ToyContainer.constainer.Count > 0)
                        {
                            ToyContainer.constainer.TryDequeue(out Toy toy);
                            index++;
                        }
                    };
                    if (index == 10000)
                    {
                        Console.WriteLine("用时：{0}", DateTime.Now.Subtract(removeBegin).TotalMilliseconds);
                    }
                }
            });
        }
    }
}
