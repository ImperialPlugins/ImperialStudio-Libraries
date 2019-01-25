using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using ImperialStudio.Core.DependencyInjection;
using ImperialStudio.Core.Eventing;
using ImperialStudio.Core.Logging;
using UnityEngine;
using ILogger = ImperialStudio.Core.Logging.ILogger;

namespace ImperialStudio.Core.Scheduling
{
    public class UnityTaskScheduler : MonoBehaviour, ITaskScheduler
    {
        public IEnumerable<ITask> Tasks => m_Tasks.AsReadOnly();

        private static volatile int m_NextTaskId;

        [AutoInject]
        private ILogger m_Logger { get; set; }

        [AutoInject]
        private IEventBus m_EventBus { get; set; }

        private readonly List<ITask> m_Tasks = new List<ITask>();

        protected virtual void Awake()
        {
            (new AsyncThreadPool(this)).Start();
        }

        protected virtual void OnDestroy()
        {
            m_Logger.LogDebug("[UnityTaskScheduler] OnDestroy");

            foreach (var task in Tasks)
                task.Cancel();
            m_Tasks.Clear();
        }

        public virtual ITask ScheduleUpdate(object @object, Action action, string taskName, ExecutionTargetContext target)
        {
            UnityTask task = new UnityTask(++m_NextTaskId, taskName, this, @object, action, target);

            TriggerEvent(task, (sender, @event) =>
            {
                if (target != ExecutionTargetContext.Sync)
                    return;

                if (@event != null && ((ICancellableEvent)@event).IsCancelled)
                    return;

                action();
                m_Tasks.Remove(task);
            });

            return task;
        }

        public virtual void TriggerEvent(UnityTask task, EventCallback cb = null)
        {
            TaskScheduleEvent e = new TaskScheduleEvent(task);

            if (m_EventBus == null)
            {
                m_Tasks.Add(task);
                cb?.Invoke(task.Owner, null);
                return;
            }

            m_EventBus.Emit(task.Owner, e, @event =>
            {
                task.IsCancelled = e.IsCancelled;

                if (!e.IsCancelled)
                    m_Tasks.Add(task);

                cb?.Invoke(task.Owner, @event);
            });
        }

        public virtual bool CancelTask(ITask task)
        {
            if (task.IsFinished || task.IsCancelled)
                return false;

            ((UnityTask)task).IsCancelled = true;
            return true;
        }

        public ITask SchedulePeriodically(object @object, Action action, string taskName, TimeSpan period,
                                          TimeSpan? delay = null, bool runAsync = false)
        {
            UnityTask task = new UnityTask(++m_NextTaskId, taskName, this, @object, action,
                runAsync ? ExecutionTargetContext.Async : ExecutionTargetContext.Sync)
            {
                Period = period
            };

            if (delay != null)
                task.StartTime = DateTime.Now + delay;

            TriggerEvent(task);
            return task;
        }

        public ITask ScheduleAt(object @object, Action action, string taskName, DateTime date, bool runAsync = false)
        {
            UnityTask task = new UnityTask(++m_NextTaskId, taskName, this, @object, action,
                runAsync ? ExecutionTargetContext.Async : ExecutionTargetContext.Sync)
            {
                StartTime = date
            };

            TriggerEvent(task);
            return task;
        }

        protected virtual void Update()
        {
            var cpy = Tasks.ToList(); // we need a copy because the task list may be modified at runtime
            foreach (ITask task in cpy.Where(c => !c.IsFinished && !c.IsCancelled))
            {
                if(task.Period == null || (task.Period != null  && task.ExecutionTarget != ExecutionTargetContext.Sync)) 
                    if (task.ExecutionTarget != ExecutionTargetContext.EveryFrame
                        && task.ExecutionTarget != ExecutionTargetContext.NextFrame)
                        continue;

                RunTask(task);
            }
        }

        protected virtual void FixedUpdate()
        {
            var cpy = Tasks.ToList(); // we need a copy because the task list may be modified at runtime
            foreach (ITask task in cpy.Where(c => !c.IsFinished && !c.IsCancelled))
            {
                if (task.ExecutionTarget != ExecutionTargetContext.EveryPhysicsUpdate
                    && task.ExecutionTarget != ExecutionTargetContext.NextPhysicsUpdate)
                    continue;

                RunTask(task);
            }
        }

        protected internal virtual void RunTask(ITask task)
        {
            if (task.StartTime != null && task.StartTime > DateTime.Now)
                return;

            if (task.EndTime != null && task.EndTime < DateTime.Now)
            {
                ((UnityTask)task).EndTime = DateTime.Now;
                RemoveTask(task);
                return;
            }

            if (task.Period != null
                && ((UnityTask) task).LastRunTime != null
                && DateTime.Now - ((UnityTask) task).LastRunTime < task.Period)
                return;

            try
            {
                task.Action.Invoke();
                ((UnityTask)task).LastRunTime = DateTime.Now;
            }
            catch (Exception e)
            {
                m_Logger.LogError("An exception occured in task: " + task.Name, e);
            }

            if (task.ExecutionTarget == ExecutionTargetContext.NextFrame
                || task.ExecutionTarget == ExecutionTargetContext.NextPhysicsUpdate
                || task.ExecutionTarget == ExecutionTargetContext.Async
                || task.ExecutionTarget == ExecutionTargetContext.NextAsyncFrame
                || task.ExecutionTarget == ExecutionTargetContext.Sync)
            {
                ((UnityTask)task).EndTime = DateTime.Now;
                RemoveTask(task);
            }
        }

        public virtual void RemoveTask(ITask task)
        {
            m_Tasks.Remove(task);
        }
    }
}
