using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusFailoverPOC
{
    class Program
    {
        static void Main(string[] args)
        {
            PeriodErrorCounter counter = new PeriodErrorCounter(5);

 
            Random rnd = new Random();
            Task.Run(async () =>
            {
                while (true)
                {
                    int addThreadCount = 10;
                    int add = rnd.Next(100, 200);

                    for (int t = 0; t < addThreadCount; t++)
                    {
                        Task.Run(() => { counter.AddCount(add); });
                    }

                    Console.WriteLine(DateTime.Now.ToString() + " Adding:" + add * addThreadCount + " Active:" + counter.ActiveErrorCount);
 
                    await Task.Delay(1000);
                }

            });

            Console.ReadLine();
        }
    }
}
