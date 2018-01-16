using DoubleBufferedQueue.v1;
using DoubleBufferedQueue.v2;
using System;
using System.Threading;

namespace DoubleBufferedQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            //ManualResetEventTest.EastWindComming();
            //KidPlayToyClient.Play();

            ClientV2.Run();

            Console.Read();
        }
    }
}
