using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fiskarn.Services
{
    public static class InteractionHandler
    {
        private static Task InteractionRunner;
        private static IList<Action> InteractionQueue = new List<Action>();

        public static void QueueInteraction(Action action)
        {
            InteractionQueue.Add(action);

            if (InteractionRunner == null)
            {
                StartInteracting();
            }
        }

        private static void StartInteracting()
        {
            InteractionRunner = new Task(() => {
                while (true)
                {
                    if (InteractionQueue.Count == 0)
                    {
                        Thread.Sleep(200);
                        continue;
                    }

                    var nextInteraction = InteractionQueue.FirstOrDefault();
                    nextInteraction();
                    InteractionQueue.RemoveAt(0);
                    Thread.Sleep(200);
                }
            });

            InteractionRunner.Start();
        }
    }
}
