using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fiskarn.Services
{
    public static class TaskQueue
    {
        private static IList<Action> _queuedTasks  = new List<Action>();
        private static bool _running = false;

        public static void QueuePriorityTask(Action task)
        {
            _queuedTasks.Insert(0, task);
            RunIfNotStarted();
        }

        public static void QueueTask(Action task)
        {
            _queuedTasks.Add(task);
            RunIfNotStarted();
        }

        private static void RunIfNotStarted()
        {
            if (!_running)
            {
                _running = true;
                Task.Run(Update);
            }
        }

        private static void Update()
        {
            while (true)
            {
                var task = _queuedTasks.FirstOrDefault();
                if(task != null)
                {
                    task();
                    _queuedTasks.Remove(task);
                }
                Thread.Sleep(100);
            }
        }
    }
}
