namespace RoadSystem
{
    public interface ITaskListener
    {
        void OnTaskCompleted(ITask task);
    }
}