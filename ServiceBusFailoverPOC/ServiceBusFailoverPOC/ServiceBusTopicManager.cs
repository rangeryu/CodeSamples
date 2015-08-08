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

    public class TopicMgrProfile
    {
        public string TopicPath { get; set; }
        public string ConnectionStringMaster { get; set; }
        public string ConnectionStringSlave { get; set;}
    }

    public class SubscriptionMgrProfile : TopicMgrProfile
    {
        public int MaxConcurrentCalls { get; set; }
        public int ClientPrefetchCount { get; set; }
        public string SubscriptionName { get; set; }

    }

    public class ServiceBusSubscriptionManager<TMsgBody> where TMsgBody : IQueuedTask
    {
        public SubscriptionMgrProfile SubsProfile { get; private set; }

        private readonly ManualResetEvent _pauseProcessingEvent;
        private readonly TimeSpan _waitTime = TimeSpan.FromSeconds(5);

        private SubscriptionClient _clientMaster;
        private SubscriptionClient _clientSlave;

        public bool SlaveEnabled { get; private set; }

        int _totalReceivedCount = 0;
        int _concurrentJobCount = 0;

        public int TotalReceivedCount => _totalReceivedCount;

        public int ConcurrentJobCount => _concurrentJobCount;

        public ServiceBusSubscriptionManager(SubscriptionMgrProfile profile)
        {
            this.SubsProfile = profile;
            this._pauseProcessingEvent = new ManualResetEvent(true);
        }

        public void Initialize()
        {
            _clientMaster = SubscriptionClient.CreateFromConnectionString
            (this.SubsProfile.ConnectionStringMaster, this.SubsProfile.TopicPath, this.SubsProfile.SubscriptionName);

            _clientMaster.PrefetchCount = this.SubsProfile.ClientPrefetchCount;

            if (!string.IsNullOrEmpty(this.SubsProfile.ConnectionStringSlave))
            {
                _clientSlave = SubscriptionClient.CreateFromConnectionString
            (this.SubsProfile.ConnectionStringSlave, this.SubsProfile.TopicPath, this.SubsProfile.SubscriptionName);

                if (_clientSlave != null)
                {
                    this._clientSlave.PrefetchCount = this.SubsProfile.ClientPrefetchCount;
                    this.SlaveEnabled = true;

                }
            }

        }


        /// <summary>
        /// bind action to client.OnMessageAsync. so it must be called only once.
        /// </summary>
        /// <param name="processMessageTask"></param>
        public void StartReceiveMessages(Func<TMsgBody, Task> processMessageTask)
        {
            var options = new OnMessageOptions
            {
                AutoComplete = false,
                MaxConcurrentCalls = this.SubsProfile.MaxConcurrentCalls

            };

            options.ExceptionReceived += this.OptionsOnExceptionReceived;

            Func<BrokeredMessage, Task> msgProcessFunc = async (BrokeredMessage msg) =>
            {
                // Will block the current thread if Stop is called.
                this._pauseProcessingEvent.WaitOne();
                try
                {
                    Interlocked.Increment(ref _totalReceivedCount);
                    Interlocked.Increment(ref _concurrentJobCount);

                    var msgBody = msg.GetBody<TMsgBody>();

                    Trace.TraceInformation("{2} Start to process task {0} @{1}", msgBody.TaskId, DateTime.Now, this.SubsProfile.SubscriptionName);
                    // Execute processing task here
                    await processMessageTask(msgBody);

                    Trace.TraceInformation("{2} done task {0} @{1}", msgBody.TaskId, DateTime.Now, this.SubsProfile.SubscriptionName);

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
                    Trace.TraceError("Exception in QueueClient.OnMessageAsync.msgProcessFunc: {0} {1}", msg.MessageId, ex.Message);
                    msg.Abandon();
                }
                finally
                {
                    Interlocked.Decrement(ref _concurrentJobCount);
                }
            };

            try
            {
                this._clientMaster.OnMessageAsync((m) => msgProcessFunc(m), options);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception in ClientMaster.OnMessageAsync: {0}", ex.Message);
            }

            if (SlaveEnabled)
            {
                try
                {
                    this._clientSlave.OnMessageAsync((m) => msgProcessFunc(m), options);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Exception in ClientSlave.OnMessageAsync: {0}", ex.Message);

                }
            }


        }

        public async Task StopAsync()
        {
            // Pause the processing threads
            this._pauseProcessingEvent.Reset();

            // There is no clean approach to wait for the threads to complete processing.
            // We simply stop any new processing, wait for existing thread to complete, then close the message pump and then return
            Thread.Sleep(_waitTime);

            await _clientMaster.CloseAsync();

            if (SlaveEnabled)
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

    public class ServiceBusTopicManager<TMsgBody> where TMsgBody : IQueuedTask
    {
        public TopicMgrProfile TopicProfile { get; private set; }

        private readonly ManualResetEvent _pauseProcessingEvent;
        private readonly TimeSpan _waitTime = TimeSpan.FromSeconds(5);

        private TopicClient _clientMaster;
        private TopicClient _clientSlave;
        private readonly PeriodErrorCounter _errorCounter;
        private bool _directSlave;
        public bool SlaveEnabled { get; private set; }



        public ServiceBusTopicManager(TopicMgrProfile profile)
        {
            this.TopicProfile = profile;
            this._pauseProcessingEvent = new ManualResetEvent(true);
            _errorCounter = new PeriodErrorCounter(30);
        }

        public void Initialize()
        {
            _clientMaster = CreateTopicClient(this.TopicProfile.ConnectionStringMaster,
                this.TopicProfile.TopicPath);


            if (!string.IsNullOrEmpty(this.TopicProfile.ConnectionStringSlave))
            {
                _clientSlave = CreateTopicClient(this.TopicProfile.ConnectionStringSlave,
                this.TopicProfile.TopicPath);
                if (_clientSlave != null)
                {
                    SlaveEnabled = true;
                }
            }


        }


        private TopicClient CreateTopicClient(string connString, string topicPath)
        {
            TopicDescription topicDescription;
            // Check queue existence.
            var manager = NamespaceManager.CreateFromConnectionString(connString);

            try
            {
                topicDescription = manager.GetTopic(topicPath);
            }
            catch (MessagingEntityNotFoundException)
            {
                try
                {
                    topicDescription = new TopicDescription(topicPath)
                    {
                        EnableBatchedOperations = true,
                        EnableExpress = true,
                        EnablePartitioning = true,
                        RequiresDuplicateDetection = true,
                    };

                    topicDescription = manager.CreateTopic(topicDescription);
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                    Trace.TraceWarning(
                        "MessagingEntityAlreadyExistsException Creating Topic " +
                        "- Topic likely already exists for path: {0}", topicPath);
                }
                catch (MessagingException ex)
                {
                    var webException = ex.InnerException as WebException;
                    if (webException != null)
                    {
                        var response = webException.Response as HttpWebResponse;

                        if (response == null || response.StatusCode != HttpStatusCode.Conflict)
                        {
                            throw ex;
                        }

                        Trace.TraceWarning("MessagingException HttpStatusCode.Conflict " +
                                           "- Topic likely already exists or is being created or deleted for path: {0}", topicPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("ServiceBusQueueManager Initialize " + ex.Message);
            }
            
            return TopicClient.CreateFromConnectionString(connString, topicPath);
        }

        public async Task<bool> SendMessageAsync(TMsgBody msg)
        {
            // Will block the current thread if Stop is called.
            this._pauseProcessingEvent.WaitOne();
            
            var brokeredMsg = new BrokeredMessage(msg) { MessageId = msg.TaskId };

            //need to abstract the properties pairs for topic filtering, from the TMsgBody
            brokeredMsg.Properties["PriorityLevel"] = (int)msg.PriorityLevel;
            brokeredMsg.Properties["TaskCategory"] = msg.TaskCategory;

            var retryStrategy = new Incremental(3, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(500));
            var retryPolicy = new RetryPolicy<ServiceBusTransientErrorDetectionStrategy>(retryStrategy);
            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    if (!SlaveEnabled || !_directSlave)
                        await _clientMaster.SendAsync(brokeredMsg);
                    else
                        await _clientSlave.SendAsync(brokeredMsg);
                });
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Master instance Error:" + ex.Message);

                // if in the last (_errorCounter.TimeWindowSeconds, 30s set in constructor) seconds, the error count reachs 20, send all following message via slave directly.
                _errorCounter.AddCount(1);

                if (SlaveEnabled)
                {
                    if (_errorCounter.ActiveErrorCount > 20)
                        _directSlave = true;

                    try
                    {
                        brokeredMsg = new BrokeredMessage(msg) { MessageId = msg.TaskId };//require to init the brokeredMsg again
                        retryPolicy.ExecuteAction(() =>
                        {
                            _clientSlave.Send(brokeredMsg);
                        });
                        return true;
                    }
                    catch (Exception secondEx)
                    {
                        Trace.TraceError("Slave instance Error:" + secondEx.Message);
                    }
                }
                return false;
            }

        }

        public async Task<bool> SendMessagesAsync(IEnumerable<TMsgBody> messages)
        {
            // Will block the current thread if Stop is called.
            this._pauseProcessingEvent.WaitOne();

            var sbMsgList = new List<BrokeredMessage>();

            messages.ToList().ForEach(msg =>
            {
                var sbMsg = new BrokeredMessage(msg) { MessageId = msg.TaskId };

                //need to abstract the properties pairs for topic filtering, from the TMsgBody
                sbMsg.Properties["PriorityLevel"] = msg.PriorityLevel;
                sbMsg.Properties["TaskCategory"] = msg.TaskCategory;

                sbMsgList.Add(sbMsg);
            });


            var retryStrategy = new Incremental(3, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(500));
            var retryPolicy = new RetryPolicy<ServiceBusTransientErrorDetectionStrategy>(retryStrategy);
            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    if (!_directSlave)
                        await _clientMaster.SendBatchAsync(sbMsgList);
                    else
                        await _clientSlave.SendBatchAsync(sbMsgList);
                });

                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Master instance Error:" + ex.Message);

                _errorCounter.AddCount(1);

                if (SlaveEnabled)
                {
                    if (_errorCounter.ActiveErrorCount > 20)
                        _directSlave = true;

                    try
                    {
                        //sb msg entities is not going to be re-used.
                        sbMsgList.Clear();
                        messages.ToList().ForEach(msg =>
                        {
                            var sbMsg = new BrokeredMessage(msg) { MessageId = msg.TaskId };

                            //need to abstract the properties pairs for topic filtering, from the TMsgBody
                            sbMsg.Properties["PriorityLevel"] = msg.PriorityLevel;
                            sbMsg.Properties["TaskCategory"] = msg.TaskCategory;

                            sbMsgList.Add(sbMsg);
                        });

                        
                        retryPolicy.ExecuteAction(() =>
                        {
                            _clientSlave.SendBatch(sbMsgList);
                        });
                        return true;
                    }
                    catch (Exception secondEx)
                    {
                        Trace.TraceWarning("Slave instance Error:" + secondEx.Message);
                    }
                }
                return false;
            }


        }




        public async Task StopAsync()
        {
            // Pause the processing threads
            this._pauseProcessingEvent.Reset();

            // There is no clean approach to wait for the threads to complete processing.
            // We simply stop any new processing, wait for existing thread to complete, then close the message pump and then return
            Thread.Sleep(_waitTime);

            await _clientMaster.CloseAsync();

            if (SlaveEnabled)
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

