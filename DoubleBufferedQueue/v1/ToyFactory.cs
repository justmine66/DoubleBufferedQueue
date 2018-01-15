using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DoubleBufferedQueue.v1
{
    /// <summary>
    /// 玩具工厂
    /// “不停地”生产玩具
    /// </summary>
    public class ToyFactory
    {
        object sysnObj = new object();
        public Task RunAsync()
        {
            return Task.Run(()=> {
                while (true)
                {
                    var toy = new Toy() { Name = "玩具" };
                    lock (sysnObj)
                    {
                        ToyContainer.constainer.Enqueue(toy);
                    };

                    Task.Delay(100);
                }
            });
        }
    }
}
