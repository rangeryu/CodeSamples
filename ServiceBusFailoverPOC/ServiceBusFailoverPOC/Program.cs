using System;
using System.Collections.Generic;
using System.Linq;
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
            // Please update the connection string before test.
            string connStringMaster =
                @"Endpoint=sb://angiris-demo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Qkcv5pt4C+1jUECZOHuGY3XAxiOEC8b5pW9KYvVF8Wk=";
   


            ServiceBusQueueManager<MessageTask> msgSender = new ServiceBusQueueManager<MessageTask>("fotest",
                connStringMaster);

            //Create two receiver clients to pull the msg.


            Console.WriteLine("Initilizing...");


            // it is better to ensure the queues have been created already before running the application. although the queue manager will create if not exsits, in case each of Initialize() functions are called parallelly, lock issue will happen.
            msgSender.Initialize();




            var senderTask = new Task(async () =>
            {
                int messageId = 0;

                while (true)
                {
                    int countInBatch = 100;

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
                    Console.WriteLine("Message sent. Total Count: " + messageId);
                    await Task.Delay(1000);
                }
            }, TaskCreationOptions.LongRunning);


            Console.WriteLine("Start Sending.");
            senderTask.Start();

            Console.WriteLine("Start Receiving.");

             

            List<ServiceBusQueueManager<MessageTask>> queueMgrlist = new List<ServiceBusQueueManager<MessageTask>>();

            int lastTotalCount = 0;
            DateTime lastTotalTime = DateTime.UtcNow;

            while (true)
            {
                Console.WriteLine("Input: add/stat");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "add":
                    {
                        queueMgrlist.Add(CreateNewQueueManagerAndReceive(connStringMaster));
                        Console.WriteLine("Queue Managers total count: " + queueMgrlist.Count);
                        break;
                    }
                    case "stat":
                    {
                        int idx = 0;
                        queueMgrlist.ForEach(q =>
                        {
                            Console.WriteLine("QueueMgr:" + idx++ + " Total: " + q.TotalReceivedCount + " Concurrent: " + q.ConcurrentJobCount);
                        });

                        var totalCount = queueMgrlist.Select(q => q.TotalReceivedCount).Sum();
                        var throughput = (totalCount - lastTotalCount)/(DateTime.UtcNow - lastTotalTime).TotalSeconds;

                        Console.WriteLine("All queues Total: " + totalCount
                                          + " Concurrent: " + queueMgrlist.Select(q => q.ConcurrentJobCount).Sum()
                                          + " Throughput: " + throughput.ToString("F") + @"/s");

                        lastTotalCount = totalCount;
                        lastTotalTime = DateTime.UtcNow;

                        break;
                    }
                    default:break;
                }
            }
 
         }
 

        public ServiceBusQueueManager<MessageTask> CreateNewQueueManagerAndReceive(string connStringMaster)
        {
            ServiceBusQueueManager<MessageTask> msgReceiver = new ServiceBusQueueManager<MessageTask>("fotest",
connStringMaster);

            msgReceiver.Initialize();

            msgReceiver.StartReceiveMessages(async (msgTask, clientId) =>
            {
                //Console.WriteLine("MsgReceiver1, from {2}: ID={0} Content={1}", msgTask.TaskId, msgTask.TaskContent, clientId);
                await Task.Delay(1000);
                msgTask.Status = TaskStatus.Completed;

            });

            return msgReceiver;
        }

    }

    
}
