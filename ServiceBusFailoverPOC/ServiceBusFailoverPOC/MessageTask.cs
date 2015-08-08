using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceBusFailoverPOC
{


    public interface IQueuedTask
    {
        string TaskId { get; set; }
        string TaskContent { get; set; }

        TaskStatus Status { get; set; }

        TaskPriority PriorityLevel { get; set; }
        string TaskCategory { get; set; }
    }

    public class MessageTask : IQueuedTask
    {

        public string TaskId
        {
            get;
            set;
        }

        public string TaskContent
        {
            get;
            set;
        }

        public TaskStatus Status
        {
            get;
            set;
        }

        public TaskPriority PriorityLevel { get; set; }
        public string TaskCategory { get; set; }
    }



    public enum TaskPriority
    {
        High = 0,
        Normal = 1,
        Low = 2
    }


    public enum TaskStatus
    {
        New = 0,
        Queueing = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4,
        InDeadletter = 5,
        TimedOut = 6

    }
}
