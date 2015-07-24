using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusFailoverPOC
{
    public class PeriodErrorCounter
    {
        public int TimeWindowSeconds { get; private set; }

        public int ActiveErrorCount
        {
            get { return _errorQueue.Count; }
        }

        private readonly ConcurrentQueue<DateTime> _errorQueue = new ConcurrentQueue<DateTime>();

        public PeriodErrorCounter(int timeWindowSeconds)
        {
            this.TimeWindowSeconds = timeWindowSeconds;

            var dequeueTask = new Task(async () =>
            {
                bool hang = false;
                while (true)
                {
                    if (hang)
                    {
                        await Task.Delay(1000);
                    }


                    DateTime peekDt;
                    if (_errorQueue.TryPeek(out peekDt) && peekDt < DateTime.UtcNow.AddSeconds(-1*TimeWindowSeconds))
                    {
                        _errorQueue.TryDequeue(out peekDt);
                        hang = false;
                    }
                    else
                    {
                        hang = true;
                    }
                }
            }, TaskCreationOptions.LongRunning);

            dequeueTask.Start();


        }



        public void AddCount(int count)
        {
            for (int i = 0; i < count; i++)
                _errorQueue.Enqueue(DateTime.UtcNow);
        }

    }
}
