using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusFailoverPOC
{
    class Program
    {
        static void Main(string[] args)
        {
            //FailoverNormalSendRecvTest sendRecvTest = new FailoverNormalSendRecvTest();
            //sendRecvTest.Start();


            ConcurrentTest test = new ConcurrentTest();
            test.Start();
        }
    }

    public class ConcurrentTest
    {
        public void Start()
        {
            ServicePointManager.DefaultConnectionLimit = 50;

            // Please update the connection string before test.
            string connStringMaster =
                @"Endpoint=sb://angiris-demo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Qkcv5pt4C+1jUECZOHuGY3XAxiOEC8b5pW9KYvVF8Wk=";
   


            ServiceBusQueueManager<MessageTask> msgSender = new ServiceBusQueueManager<MessageTask>("fotest",
                connStringMaster);

            //Create two receiver clients to pull the msg.


            Console.WriteLine("Initilizing...");


            // it is better to ensure the queues have been created already before running the application. although the queue manager will create if not exsits, in case each of Initialize() functions are called parallelly, lock issue will happen.
            msgSender.Initialize();

  

            Console.WriteLine("Start Receiving.");

             

            List<ServiceBusQueueManager<MessageTask>> queueMgrlist = new List<ServiceBusQueueManager<MessageTask>>();

            Task statShowTask = new Task(async () =>
            {
                int lastTotalCount = 0;
                DateTime lastTotalTime = DateTime.UtcNow;

                while (true)
                {
                    int idx = 0;
                    queueMgrlist.ForEach(q =>
                    {
                        Console.WriteLine("QueueMgr:" + idx++ + " Total: " + q.TotalReceivedCount + " Concurrent: " + q.ConcurrentJobCount);
                    });

                    var totalCount = queueMgrlist.Select(q => q.TotalReceivedCount).Sum();
                    var throughput = (totalCount - lastTotalCount) / (DateTime.UtcNow - lastTotalTime).TotalSeconds;

                    Console.WriteLine("All queues Total: " + totalCount
                                      + " Concurrent: " + queueMgrlist.Select(q => q.ConcurrentJobCount).Sum()
                                      + " Throughput: " + throughput.ToString("F") + @"/s");
                    Console.WriteLine();

                    lastTotalCount = totalCount;
                    lastTotalTime = DateTime.UtcNow;

                    await Task.Delay(3000);
                }
            }, TaskCreationOptions.LongRunning);

            statShowTask.Start();

            int messageId = 0;
            while (true)
            {
                Console.WriteLine("Input: add/send");
                string input = Console.ReadLine();
                switch (input.Split(' ').FirstOrDefault())
                {
                    case "add":
                    {
                        queueMgrlist.Add(CreateNewQueueManagerAndReceive(connStringMaster));
                        Console.WriteLine("Queue Managers total count: " + queueMgrlist.Count);
                        break;
                    }
                    case "send":
                    {
                        Task.Run(async () =>
                        {
                            int countInBatch = 100;
                            int.TryParse(input.Split(' ').LastOrDefault(), out countInBatch);

                            List<MessageTask> msgList = new List<MessageTask>();
                            for (int i = 0; i < countInBatch; i++)
                            {
                                Interlocked.Increment(ref messageId);

                                msgList.Add(new MessageTask()
                                {
                                    TaskId = (messageId).ToString(),
                                    TaskContent = DateTime.UtcNow.ToString()
                                });
                            }

                            await msgSender.SendMessagesAsync(msgList);
                            Trace.WriteLine("Message sent. Total Count: " + messageId);

                        });
                       break;
                    }
                    default:break;
                }
            }
 
         }



        HttpClient httpClient = new HttpClient();
 
        public ServiceBusQueueManager<MessageTask> CreateNewQueueManagerAndReceive(string connStringMaster)
        {
            ServiceBusQueueManager<MessageTask> msgReceiver = new ServiceBusQueueManager<MessageTask>("fotest",
                connStringMaster, "", maxConcurrentCalls: 500, clientPrefetchCount: 50);

            msgReceiver.Initialize();


            msgReceiver.StartReceiveMessages(async (msgTask, clientId) =>
            {
                //Console.WriteLine("MsgReceiver1, from {2}: ID={0} Content={1}", msgTask.TaskId, msgTask.TaskContent, clientId);
                //fake call leveraging http protocol
                string[] fakeResource = { "http://www.microsoft.com/privacystatement/en-us/bingandmsn/default.aspx"
                                                 , "http://www.bing.com/search?q=azure%20service%20bus"
                                                 , "http://azure.microsoft.com/blog/"
                                                 , "https://msdn.microsoft.com/en-US/" };

                Random rnd = new Random();
                var resource = fakeResource[rnd.Next(0, fakeResource.Length)] + "?rnd=" + rnd.Next().ToString();
                try
                {
                    var result = await httpClient.GetAsync(resource);
                    msgTask.Status = TaskStatus.Completed;
                }
                catch (Exception ex)
                {
                    Trace.TraceError("http error", ex.Message);
                 }


            });

            return msgReceiver;
        }

    }

    
}
