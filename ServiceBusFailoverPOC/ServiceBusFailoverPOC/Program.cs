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


            TopicSubTest test = new TopicSubTest();
            test.Start();
        }
    }

    public class TopicSubTest
    {
        readonly HttpClient _httpClient = new HttpClient();

        public void Start()
        {
            string connStringMaster =
    @"Endpoint=sb://angiris-demo.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Qkcv5pt4C+1jUECZOHuGY3XAxiOEC8b5pW9KYvVF8Wk=";

            string topicPath = "testtopic";

            string[] subsList = new[] {"CateAP0", "CateAP1", "CateBP0", "CateBP1"};

            subsList.ToList().ForEach(subs => CreateNewSubsManagerAndReceive(connStringMaster, topicPath, subs));

            Console.WriteLine("SubsManagers ready, start sending");

            Task.Run(async () => await SendTestMessages(connStringMaster,topicPath));

            Console.ReadLine();

            Console.WriteLine("existing");


        }

        public async Task SendTestMessages(string connStringMaster, string topicPath)
        {
            TopicMgrProfile profile = new TopicMgrProfile()
            {
                ConnectionStringMaster = connStringMaster,
                TopicPath = topicPath
            };

            ServiceBusTopicManager<MessageTask> msgSender = new ServiceBusTopicManager<MessageTask>(profile);
            msgSender.Initialize();

            Random rnd = new Random();
            string[] taskCates = new[] { "A", "A", "B" };
            TaskPriority[] taskPs = new[]
            {
                    TaskPriority.High, TaskPriority.High,
                    TaskPriority.Normal, TaskPriority.Normal, TaskPriority.Normal,
                    TaskPriority.Low
                };

            while (true)
            {
                for (int i = 0; i < 10; i++)
                {
                    MessageTask msg = new MessageTask()
                    {
                        TaskCategory = taskCates[rnd.Next(0, taskCates.Length)],
                        PriorityLevel = taskPs[rnd.Next(0, taskPs.Length)],
                    };
                    msg.TaskId = msg.TaskCategory + ((int)msg.PriorityLevel) + "  " + Guid.NewGuid();

                    await msgSender.SendMessageAsync(msg);
                }

                await Task.Delay(1000);
            }
        }

        public ServiceBusSubscriptionManager<MessageTask> CreateNewSubsManagerAndReceive
            (string connStringMaster, string topicPath, string subsName)
        {
            SubscriptionMgrProfile profile = new SubscriptionMgrProfile()
            {
                ClientPrefetchCount = 10,
                MaxConcurrentCalls = 500,
                ConnectionStringMaster = connStringMaster,
                SubscriptionName = subsName,
                TopicPath = topicPath
            };

            ServiceBusSubscriptionManager<MessageTask> msgReceiver = new ServiceBusSubscriptionManager<MessageTask>(profile);

            msgReceiver.Initialize();


            msgReceiver.StartReceiveMessages(async (msgTask) =>
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
                    //var result = await _httpClient.GetAsync(resource);
                    msgTask.Status = TaskStatus.Completed;
                    Console.WriteLine(subsName + " > " + msgTask.TaskId);
                    await Task.Delay(100);

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
