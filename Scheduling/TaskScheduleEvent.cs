using ImperialStudio.Core.Api.Eventing;
using ImperialStudio.Core.Api.Scheduling;
using ImperialStudio.Core.Eventing;

namespace ImperialStudio.Core.Scheduling
{
    public class TaskScheduleEvent : Event, ICancellableEvent
    {
        public TaskScheduleEvent(ITask task)
        {
            Task = task;
        }

        public ITask Task { get; }

        public bool IsCancelled { get; set; }
    }
}