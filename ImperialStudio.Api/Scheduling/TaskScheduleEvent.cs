using ImperialStudio.Api.Eventing;

namespace ImperialStudio.Api.Scheduling
{
    public class TaskScheduleEvent : IEvent, ICancellableEvent
    {
        public TaskScheduleEvent(ITask task)
        {
            Task = task;
        }

        public ITask Task { get; }

        public bool IsCancelled { get; set; }
    }
}