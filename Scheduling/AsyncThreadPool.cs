using System.Linq;
using System.Threading;

namespace ImperialStudio.Core.Scheduling
{
    public class AsyncThreadPool
    {
        private readonly UnityTaskScheduler m_TaskScheduler;

        public AsyncThreadPool(UnityTaskScheduler scheduler)
        {
            m_TaskScheduler = scheduler;
        }

        private Thread m_TaskThread;

        public void Start()
        {
            m_TaskThread = new Thread(ContinousThreadLoop);
            m_TaskThread.Start();
        }

        private void ContinousThreadLoop()
        {
            while (true)
            {
                var cpy = m_TaskScheduler.Tasks.Where(c => !c.IsFinished && !c.IsCancelled).ToList(); // we need a copy because the task list may be modified at runtime

                foreach (ITask task in cpy)
                {
                    if(task.Period == null || (task.Period != null  && task.ExecutionTarget != ExecutionTargetContext.Async))         
                        if (task.ExecutionTarget != ExecutionTargetContext.EveryAsyncFrame)
                            continue;

                    m_TaskScheduler.RunTask(task);
                }

                foreach (ITask task in cpy)
                {
                    if (task.ExecutionTarget != ExecutionTargetContext.NextAsyncFrame &&
                        task.ExecutionTarget != ExecutionTargetContext.Async)
                        continue;

                    m_TaskScheduler.RunTask(task);
                }

                Thread.Sleep(20);
            }
        }
    }
}
