using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DoubleBufferedQueue
{
    /// <summary>
    /// 表示一个“双缓冲队列”集合类
    /// </summary>
    public class DoubleBufferingQueue<TModel>
    {
        private ConcurrentQueue<TModel> _productQueue;//生产队列
        private ConcurrentQueue<TModel> _consumeQueue;//消费队列

        private volatile ConcurrentQueue<TModel> _currentQueue;//当前队列

        //生产者写入完成信号：当消费者试图切换队列时，需要等待生产者已完成对队列的写入。
        private ManualResetEvent _writeFinishedSignal;
        //消费者切换队列时，阻止生产者的信号
        private ManualResetEvent _blockProducerSignal;
        //生产队列中已存在可用数据，通知消费任务的信号
        private AutoResetEvent _existAvailableDataSignal;
        private int _consumeBase;//平滑消费基数

        /// <summary>
        /// 初始化一个 <see cref="DoubleBufferedQueue.v2.DoubleBufferingQueue{TModel}"/> 实例
        /// </summary>
        /// <param name="consumeBase">平滑消费基数</param>
        public DoubleBufferingQueue(int consumeBase = 0)
        {
            this._consumeBase = consumeBase;
            this._productQueue = new ConcurrentQueue<TModel>();
            this._consumeQueue = new ConcurrentQueue<TModel>();
            this._currentQueue = this._productQueue;//默认当前队列为消费队列
            this._blockProducerSignal = new ManualResetEvent(true);//默认不阻塞生产者
            this._writeFinishedSignal = new ManualResetEvent(false);//默认阻塞消费者
            this._existAvailableDataSignal = new AutoResetEvent(false);//默认消费队列无可用数据
            this.ConsumeQueueAsync();//开启消费任务
        }

        /// <summary>
        /// 一个“消费队列”的任务
        /// </summary>
        /// <returns></returns>
        private Task ConsumeQueueAsync()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    //第一次启动时，等待消费队列已存在可用数据
                    this._existAvailableDataSignal.WaitOne();
                    if (this._consumeQueue.IsEmpty &&
                        this._currentQueue.Count >= this._consumeBase)
                    {
                        //通知生产者先暂停生产
                        this._blockProducerSignal.Reset();
                        //等待生产者将剩余项成功写入消费队列
                        this._writeFinishedSignal.WaitOne();

                        //切换消费队列
                        this._productQueue = this._currentQueue == this._productQueue
                                             ? this._consumeQueue
                                             : this._productQueue;
                        //消费当前队列
                        this._consumeQueue = _currentQueue;
                        //重置当前队列
                        this._currentQueue = this._productQueue;


                        //通知生产者恢复生产
                        this._blockProducerSignal.Set();
                    }
                }
            });
        }

        /// <summary>
        /// 入列
        /// </summary>
        public void Enqueue(TModel model)
        {
            //等待切换队列完成，恢复生产
            this._blockProducerSignal.WaitOne();
            //给消费者发出信号，生产者正在写入数据
            this._writeFinishedSignal.Reset();

            if (this._productQueue != null)
            {
                this._productQueue.Enqueue(model);
            }

            //给消费者发出信号，生产者写入数据完成
            this._writeFinishedSignal.Set();
            //通知消费任务，生产队列中已存在可用数据
            this._existAvailableDataSignal.Set();
        }

        /// <summary>
        /// 出列
        /// </summary>
        /// <returns></returns>
        public TModel Dequeue()
        {
            if (!this._consumeQueue.IsEmpty)
            {
                if (this._consumeQueue.TryDequeue(out TModel result))
                {
                    return result;
                }
            }

            return default(TModel);
        }

        /// <summary>
        /// 消费队列是否为空
        /// </summary>
        public bool IsEmtyForConsumeQueue { get { return this._consumeQueue.IsEmpty; } }
    }
}
