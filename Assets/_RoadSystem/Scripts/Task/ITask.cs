namespace RoadSystem
{
    public interface ITask
    {
        void Start();
        void Stop();
        void AttachListener(ITaskListener listener);
    }
}