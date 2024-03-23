using System.Collections.Generic;

namespace RoadSystem
{
    public abstract class BaseTask : ITask
    {
        private readonly List<ITaskListener> _listeners = new();
        public abstract void Start();

        public void Stop()
        {
            _listeners.Clear();
            OnStop();
        }

        protected virtual void OnStop()
        {
        }

        public void AttachListener(ITaskListener listener)
        {
            _listeners.Add(listener);
        }

        public virtual void OnCompleted()
        {
            
        }

        protected virtual void ConcludeTask()
        {
            foreach (var listener in _listeners)
            {
                listener.OnTaskCompleted(this);
            }
            
            _listeners.Clear();
        }
    }
}