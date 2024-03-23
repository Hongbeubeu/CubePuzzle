using System.Collections.Generic;
using UnityEngine;

namespace RoadSystem
{
    public class TaskHandler : MonoBehaviour, ITaskHandler , ITaskListener
    {
        private ITask _currentTask;

        public List<ITask> Tasks { get; } = new();
        public bool Working => _currentTask != null;
        public bool IsAlive => gameObject.activeInHierarchy;

        public void AddTask(ITask task, ITaskListener listener)
        {
            if (!IsAlive)
            {
                Debug.LogError("Trying to add task to inactive GameObject");
                return;
            }

            task.AttachListener(listener);
            Tasks.Add(task);
        }

        public void Work()
        {
            if (_currentTask == null && Tasks.Count > 0)
            {
                var newTask = Tasks[0];
                _currentTask = newTask;
                _currentTask.Start();
            }
        }

        public void FreeAllTask()
        {
            foreach (var task in Tasks)
            {
                task.Stop();
            }

            _currentTask = null;
            Tasks.Clear();
        }

        public void OnTaskCompleted(ITask task)
        {
            Tasks.Remove(task);
            if (Tasks.Count == 0)
            {
                _currentTask = null;
            }
            else
            {
                var newTask = Tasks[0];
                _currentTask = newTask;
                _currentTask.Start();
            }
        }
    }
}