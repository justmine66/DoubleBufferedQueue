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
    public class DoubleBufferedQueue<TModel> : IDisposable
    {
        private readonly int _millisecond;

        private readonly Queue<TModel> _write = new Queue<TModel>(); //生产队列
        private readonly Queue<TModel> _read = new Queue<TModel>();//消费队列

        //生产线程默认非阻塞(有信号)
        private readonly ManualResetEvent _equeueLock = new ManualResetEvent(true);
        //消费线程默认阻塞(无信号)
        private readonly ManualResetEvent _dequeuelock = new ManualResetEvent(false);

        //任务还未入列时，阻塞整个消费线程的信号量(有信号)
        private readonly AutoResetEvent _autoReset = new AutoResetEvent(true);

        private volatile Queue<TModel> _currentQueue;//当前要入数据的队列
        private readonly BackgroundWorker _backgroundWorker;

        /// <summary>
        /// 初始化一个<see cref="DoubleBufferedQueue.DoubleBufferedQueue{TModel}">的实例
        /// </summary>
        /// <param name="millisecond">消费者线程处理一批后，需要延时的时间，实现定时间隔操作</param>
        public DoubleBufferedQueue(int millisecond = 0)
        {
            _millisecond = millisecond;
            _currentQueue = _write;

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += BackgroundWorker_DoWork;
            _backgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// 消息者处理方法
        /// </summary>
        public Action<Queue<TModel>> ConsumerAction { get; set; }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                var readQueue = this.GetDequeue();
                ConsumerAction?.Invoke(readQueue);

                if (_millisecond > 0)
                    Thread.Sleep(TimeSpan.FromMilliseconds(_millisecond));
            }
        }

        private Queue<TModel> GetDequeue()
        {
            this._autoReset.WaitOne();  //这个信号量是保证在没有成员入队列的时，不进行其它操作

            this._dequeuelock.Reset();  //注意两把锁的顺序，不然会造成死锁的问题
            this._equeueLock.WaitOne();

            var readQueue = _currentQueue;
            _currentQueue = (_currentQueue == _write) ? _read : _write;

            this._dequeuelock.Set();
            return readQueue;
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

        public void Equeue(TModel item, Action<TModel> action = null)
        {
            this._dequeuelock.WaitOne();
            this._equeueLock.Reset();

            _currentQueue.Enqueue(item);
            action?.Invoke(item);

            _equeueLock.Set();
            _autoReset.Set();
        }

        public void Dispose()
        {
            _dequeuelock.Reset();

            ConsumerAction?.Invoke(_write);
            ConsumerAction?.Invoke(_read);

            _backgroundWorker?.Dispose();
        }
    }
}
