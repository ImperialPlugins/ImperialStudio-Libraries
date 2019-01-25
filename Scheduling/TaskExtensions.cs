namespace ImperialStudio.Core.Scheduling
{
    public static class TaskExtensions
    {
        public static bool Cancel(this ITask task)
        {
            return task.Scheduler.CancelTask(task);
        }
    }
}