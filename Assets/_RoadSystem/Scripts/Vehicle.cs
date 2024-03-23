using System;
using System.Collections.Generic;
using DG.Tweening;
using PathCreation;
using PathCreation.Examples;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoadSystem
{
    public class Vehicle : PathFollower
    {
        [SerializeField] private bool _isLookingForward;
        [SerializeField] private EndOfPathInstruction _endOfTotalPathInstruction;
        [SerializeField] private List<RoadSegment> _path;
        [SerializeField] private TaskHandler _taskHandler;
        [SerializeField] private RoadManager _roadManager;

        private float length;
        private int currentPathIndex;
        private Tween currentTween;

        public Action CompleteAction;

        public bool LookingForward
        {
            get => _isLookingForward;
            set => _isLookingForward = value;
        }

        public List<RoadSegment> Path
        {
            get => _path;
            set => _path = value;
        }

        protected override void Update()
        {
            // Do nothing
        }

        [Button(ButtonSizes.Gigantic)]
        public void StartMove()
        {
            Stop();
            pathCreator = null;
            DoMove();
        }

        private void DoMove()
        {
            if (_path == null)
            {
                Debug.LogError("Path is null");
                return;
            }

            if (pathCreator == null)
            {
                currentPathIndex = 0;
                pathCreator = _path[currentPathIndex].PathCreator;
            }

            distanceTravelled = 0;
            length = pathCreator.path.length;
            var duration = (length - distanceTravelled) / speed;
            var fromValue = distanceTravelled;

            currentTween = DOVirtual.Float(fromValue, length, duration, value =>
            {
                distanceTravelled = value;
                if (distanceTravelled > length)
                {
                    distanceTravelled = length;
                }

                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                if (!_isLookingForward)
                    transform.forward = -transform.forward;
            }).SetEase(Ease.Linear).OnComplete(NextPath);
        }

        private void NextPath()
        {
            currentTween?.Kill();
            currentPathIndex++;
            currentPathIndex %= _path.Count;
            pathCreator = _path[currentPathIndex].PathCreator;
            if (_endOfTotalPathInstruction == EndOfPathInstruction.Stop && currentPathIndex == 0)
            {
                CompleteAction?.Invoke();
                return;
            }

            DoMove();
        }

        [Button]
        private void Pause()
        {
            currentTween?.Pause();
        }

        [Button]
        private void Resume()
        {
            currentTween?.Play();
        }

        [Button]
        public void Stop()
        {
            currentTween?.Kill();
        }


        [SerializeField] private Transform _point1;
        [SerializeField] private Transform _point2;

        [Button(ButtonSizes.Gigantic)]
        public void DoMoveTask()
        {
            AddTaskMoveForward(_point1.position);
            AddTaskMoveBackward(_point2.position);
            _taskHandler.Work();
        }

        #region TaskHandler

        public void AddTaskMoveForward(Vector3 target)
        {
            var task = new MoveTask(this, _roadManager, target, true);
            _taskHandler.AddTask(task, _taskHandler);
        }

        public void AddTaskMoveBackward(Vector3 target)
        {
            var task = new MoveTask(this, _roadManager, target, false);
            _taskHandler.AddTask(task, _taskHandler);
        }

        #endregion
    }
}