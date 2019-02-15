using ImperialStudio.Api.Scheduling;

namespace ImperialStudio.Extensions.Scheduling
{
    public static class TaskExtensions
    {
        public static bool Cancel(this ITask task)
        {
            return task.Scheduler.CancelTask(task);
        }
    }
}