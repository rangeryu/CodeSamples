using System.Collections;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace ServiceBusFailoverPOC
{
 
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

   

    public class ServiceBusQueueManager<TMsgBody> where TMsgBody : IQueuedTask
    {
        public string QueueName { get; private set; }

        public string ConnectionStringMaster
        {
            get;
            private set;
        }

        public string ConnectionStringSlave
        {
            get;
            private set;
        }

        private readonly ManualResetEvent _pauseProcessingEvent;
        private readonly TimeSpan _waitTime = TimeSpan.FromSeconds(10);

        private QueueClient _clientMaster;
        private QueueClient _clientSlave;


  
        public ServiceBusQueueManager(string queueName, string connectionStringMaster, string connectionStringSlave)
        {
            this.QueueName = queueName;
            this.ConnectionStringMaster = connectionStringMaster;
            this.ConnectionStringSlave = connectionStringSlave;

            this._pauseProcessingEvent = new ManualResetEvent(true);
        }
        
        public void Initialize()
        {
            _clientMaster = CreateQueueClient(this.ConnectionStringMaster, this.QueueName);
            _clientSlave = CreateQueueClient(this.ConnectionStringSlave, this.QueueName);

        }

        private QueueClient CreateQueueClient(string connString, string queueName)
        {
            QueueDescription queueDescription;
            // Check queue existence.
            var manager = NamespaceManager.CreateFromConnectionString(connString);

            try
            {
                queueDescription = manager.GetQueue(queueName);
            }
            catch (MessagingEntityNotFoundException)
            {
                try
                {
                    queueDescription = new QueueDescription(queueName)
                    {
                        LockDuration = TimeSpan.FromMinutes(1),
                        EnableBatchedOperations = true,
                        EnableExpress = true,
                        MaxDeliveryCount = 3
                    };


                    queueDescription = manager.CreateQueue(queueDescription);
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                    Trace.TraceWarning(
                        "MessagingEntityAlreadyExistsException Creating Queue - Queue likely already exists for path: {0}", queueName);
                }
                catch (MessagingException ex)
                {
                    var webException = ex.InnerException as WebException;
                    if (webException != null)
                    {
                        var response = webException.Response as HttpWebResponse;

                        if (response == null || response.StatusCode != HttpStatusCode.Conflict)
                        {
                            throw;
                        }

                        Trace.TraceWarning("MessagingException HttpStatusCode.Conflict - Queue likely already exists or is being created or deleted for path: {0}", queueName);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("ServiceBusQueueManager Initialize " + ex.Message);
            }


            return QueueClient.CreateFromConnectionString(connString, queueName);
        }

        public async Task<bool> SendMessage(TMsgBody msg)
        {
            var brokeredMsg = new BrokeredMessage(msg) { MessageId = msg.TaskId };

            var retryStrategy = new Incremental(3, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(500));
            var retryPolicy = new RetryPolicy<ServiceBusTransientErrorDetectionStrategy>(retryStrategy);
            try
            {
                await retryPolicy.ExecuteAsync(async() =>
                {
                    await _clientMaster.SendAsync(brokeredMsg);
                });
                return true;
            }
            catch  (Exception ex)
            {
                Trace.TraceWarning("Sending from Slave instance. Master instance Error:" + ex.Message);
                try
                {
                    retryPolicy.ExecuteAction(() =>
                    {
                        _clientSlave.Send(brokeredMsg);
                    });
                    return true;
                }
                catch (Exception secondEx)
                {
                    Trace.TraceError("Slave instance Error:" + secondEx.Message);
                    return false; //we may throw the exception directly based on the api design.
                }
            }

        }

        public async Task<bool> SendMessages(IEnumerable<TMsgBody> messages)
        {
            var msgList = messages.Select(msg => new BrokeredMessage(msg) { MessageId = msg.TaskId }).ToList();

            var retryStrategy = new Incremental(3, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(500));
            var retryPolicy = new RetryPolicy<ServiceBusTransientErrorDetectionStrategy>(retryStrategy);
            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await _clientMaster.SendBatchAsync(msgList);
                });
                
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Sending from Slave instance. Master instance Error:" + ex.Message);
                try
                {
                    retryPolicy.ExecuteAction(() =>
                    {
                        _clientSlave.SendBatch(msgList);
                    });
                    return true;
                }
                catch (Exception secondEx)
                {
                    Trace.TraceWarning("Slave instance Error:" + secondEx.Message);
                    return false; //we may throw the exception directly based on the api design.
                }
            }


        }



        /// <summary>
        /// bind action to client.OnMessageAsync. so it must be called only once.
        /// </summary>
        /// <param name="processMessageTask"></param>
        public void StartReceiveMessages(Func<TMsgBody, Task> processMessageTask)
        {
            // Setup the options for the message pump.
            var options = new OnMessageOptions
            {
                AutoComplete = false,
            };

            // When AutoComplete is disabled, you have to manually complete/abandon the messages and handle errors, if any.
            options.ExceptionReceived += this.OptionsOnExceptionReceived;

            // Use of Service Bus OnMessage message pump. The OnMessage method must be called once, otherwise an exception will occur.

            Func<BrokeredMessage,Task> msgProcessFunc = async (BrokeredMessage msg) =>
            {
                // Will block the current thread if Stop is called.
                this._pauseProcessingEvent.WaitOne();
                try
                {
                    var msgBody = msg.GetBody<TMsgBody>();

                    Trace.TraceInformation("Start to process task {0} @{1}", msgBody.TaskId, DateTime.Now);
                    // Execute processing task here
                    await processMessageTask(msgBody);

                    Trace.TraceInformation("done task {0} @{1}", msgBody.TaskId, DateTime.Now);
                    switch (msgBody.Status)
                    {
                        case TaskStatus.Completed:
                            await msg.CompleteAsync();
                            break;
                        case TaskStatus.InDeadletter:
                            await msg.DeadLetterAsync();
                            break;
                        default:
                            await msg.AbandonAsync();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Exception in QueueClient.OnMessageAsync: {0}", ex.Message);
                    msg.Abandon();
                }
            };


            //both Master & Slave  message receiver subscribes the queue and process
            this._clientMaster.OnMessageAsync(msgProcessFunc,options);
            this._clientSlave.OnMessageAsync(msgProcessFunc, options);
        }

        public async Task Stop()
        {
            // Pause the processing threads
            this._pauseProcessingEvent.Reset();

            // There is no clean approach to wait for the threads to complete processing.
            // We simply stop any new processing, wait for existing thread to complete, then close the message pump and then return
            Thread.Sleep(_waitTime);

            await _clientMaster.CloseAsync();
            await _clientSlave.CloseAsync();
        }

        private void OptionsOnExceptionReceived(object sender, ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            if (exceptionReceivedEventArgs != null && exceptionReceivedEventArgs.Exception != null)
            {
                var exceptionMessage = exceptionReceivedEventArgs.Exception.Message;
                Trace.TraceError("Exception in QueueClient.ExceptionReceived: {0}", exceptionMessage);
            }
        }



    }
}

