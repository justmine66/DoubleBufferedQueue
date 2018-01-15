using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DoubleBufferedQueue.v2
{
    /// <summary>
    /// 双缓冲队列
    /// </summary>
    public class DoubleBufferingQueue<TModel>
    {
        private ConcurrentQueue<TModel> _productQueue;
        private ConcurrentQueue<TModel> _consumeQueue;

        private volatile ConcurrentQueue<TModel> _currentQueue;

        private AutoResetEvent _dataEvent;

        public DoubleBufferingQueue()
        {
            this._productQueue = new ConcurrentQueue<TModel>();
            this._consumeQueue = new ConcurrentQueue<TModel>();
            this._currentQueue = _productQueue;
            _dataEvent = new AutoResetEvent(false);
            this.ConsumeQueueAsync();
        }

        /// <summary>
        /// 一个“消费队列”的任务
        /// </summary>
        /// <returns></returns>
        private Task ConsumeQueueAsync()
        {
            return Task.Run(() =>
            {
                this._currentQueue = this._currentQueue == this._productQueue
                                     ? this._consumeQueue
                                     : this._productQueue;

            });
        }

        /// <summary>
        /// 入列
        /// </summary>
        public void Enqueue(TModel model)
        {
            if (this._productQueue != null)
            {
                this._productQueue.Enqueue(model);
            }
        }

        /// <summary>
        /// 出列
        /// </summary>
        /// <returns></returns>
        public TModel Dequeue()
        {
            if (this._consumeQueue != null &&
                this._consumeQueue.Count > 0)
            {
                if (this._consumeQueue.TryDequeue(out TModel result))
                {
                    return result;
                }
            }

            return default(TModel);
        }
    }
}
