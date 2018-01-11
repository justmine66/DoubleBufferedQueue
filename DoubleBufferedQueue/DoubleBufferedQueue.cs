using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace DoubleBufferedQueue
{
    /// <summary>
    /// 双缓冲队列
    /// </summary>
    public class DoubleBufferedQueue<TModel>
    {
        //专门负责数据写入的队列
        private readonly Queue<TModel> _write = new Queue<TModel>();
        //专门负责数据读取的队列
        private readonly Queue<TModel> _read = new Queue<TModel>();

        //消费线程默认阻塞，生产线程默认非阻塞
        private readonly ManualResetEvent lock1 = new ManualResetEvent(true);
        private readonly ManualResetEvent lock2 = new ManualResetEvent(false);
        private readonly AutoResetEvent _autoReset = new AutoResetEvent(true);

        private volatile Queue<TModel> _currentQueue;//当前要入数据的队列

        public DoubleBufferedQueue()
        {
            this._currentQueue = this._write;
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += this.BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerAsync();
        }

        public void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                this._autoReset.WaitOne();

                this.lock2.Reset();
                this.lock1.WaitOne();

                //把当前队列放入消费操作中，把另一个队列重置为当前生产队列
                Queue<TModel> readQueue = this._currentQueue;
                this._currentQueue = this._currentQueue == this._write ? this._read : this._write;

                this.lock2.Set();

                this.WriteToConsole(readQueue);
            }
        }

        private void WriteToConsole(Queue<TModel> readQueue)
        {
            while (readQueue.Count > 0)
            {
                TModel item = readQueue.Dequeue();
                Console.WriteLine($"{readQueue.GetHashCode()}\t{item}");

                Thread.Sleep(110);
            }
        }

        public void Equeue(TModel item)
        {
            this.lock2.WaitOne();
            this.lock1.Reset();

            this._currentQueue.Enqueue(item);

            this.lock1.Set();
            this._autoReset.Set();
        }
    }
}
