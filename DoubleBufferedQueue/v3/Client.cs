using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DoubleBufferedQueue.v3
{
    public class Client
    {
        public void ProducerFunc()
        {
            int data = 0;
            var m_Random = new Random();

            for (int i = 0; i < 10000; i++)
            {
                data += 1;
                MessageHandler(data);
                Thread.Sleep(m_Random.Next(0, 2));
            }
        }

        private void MessageHandler(int data)
        {
            throw new NotImplementedException();
        }
    }
}
