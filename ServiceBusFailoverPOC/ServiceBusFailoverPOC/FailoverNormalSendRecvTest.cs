using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusFailoverPOC
{
    public class FailoverNormalSendRecvTest
    {
        public void Start()
        {
            // Please update the connection string before test.
            string connStringMaster =
                @"Endpoint=sb://ranger-ha-01.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=UR/V+PT9H6WwenIm70hg+e9c3NZzs5ln8uqWqdtirns=";
            string connStringSlave =
                @"Endpoint=sb://ranger-ha-02.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=uJD7WqXtR+52/5IbuMqnZVToDK4zoSRKtCWbkynbuc8=";


            ServiceBusQueueManager<MessageTask> msgSender = new ServiceBusQueueManager<MessageTask>("fotest",
                connStringMaster, connStringSlave);

            //Create two receiver clients to pull the msg.

            ServiceBusQueueManager<MessageTask> msgReceiver = new ServiceBusQueueManager<MessageTask>("fotest",
    connStringMaster, connStringSlave);

            ServiceBusQueueManager<MessageTask> msgReceiver2 = new ServiceBusQueueManager<MessageTask>("fotest",
connStringMaster, connStringSlave);

            Console.WriteLine("Initilizing...");


            // it is better to ensure the queues have been created already before running the application. although the queue manager will create if not exsits, in case each of Initialize() functions are called parallelly, lock issue will happen.
            msgSender.Initialize();
            msgReceiver.Initialize();
            msgReceiver2.Initialize();



            var senderTask = new Task(async () =>
            {
                int messageId = 0;

                while (true)
                {
                    Task.Run(async () =>
                    {
                        Interlocked.Increment(ref messageId);

                        await msgSender.SendMessageAsync(new MessageTask()
                        {
                            TaskId = (messageId).ToString(),
                            TaskContent = "msgSender"
                        });


                    });

                    await Task.Delay(50);
                }
            }, TaskCreationOptions.LongRunning);

            var receiverTask = new Task(() =>
            {
                msgReceiver.StartReceiveMessages(async (msgTask, clientId) =>
                {
                    Console.WriteLine("MsgReceiver1, from {2}: ID={0} Content={1}", msgTask.TaskId, msgTask.TaskContent, clientId);
                    msgTask.Status = TaskStatus.Completed;

                });

                msgReceiver2.StartReceiveMessages(async (msgTask, clientId) =>
                {
                    Console.WriteLine("MsgReceiver2, from {2}: ID={0} Content={1}", msgTask.TaskId, msgTask.TaskContent, clientId);
                    msgTask.Status = TaskStatus.Completed;

                });

            }, TaskCreationOptions.LongRunning);

            Console.WriteLine("Start Sending.");
            senderTask.Start();

            Console.WriteLine("Start Receiving.");
            receiverTask.Start();

            Console.ReadLine();
            Console.WriteLine("Stopping...");
            Task.WaitAll(msgSender.StopAsync(), msgReceiver.StopAsync(), msgReceiver2.StopAsync());
        }
    }
}
