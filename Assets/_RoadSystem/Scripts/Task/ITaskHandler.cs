using System.Collections.Generic;

namespace RoadSystem
{
    public interface ITaskHandler
    {
        List<ITask> Tasks { get; }
        bool Working { get; }
        void AddTask(ITask task, ITaskListener listener);
        void Work();
        void FreeAllTask();
    }
}