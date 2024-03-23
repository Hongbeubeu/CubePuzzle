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
        #region Fields

        [SerializeField] private bool isLookingForward;
        [SerializeField] private EndOfPathInstruction endOfTotalPathInstruction;
        [SerializeField] private List<RoadSegment> path;
        [SerializeField] private TaskHandler taskHandler;
        [SerializeField] private RoadManager roadManager;

        private float _length;
        private int _currentPathIndex;
        private Tween _currentTween;
        private Transform _transform;
        private bool IsLastSegment => _currentPathIndex == path.Count - 1;

        public Action completeAction;

        public bool LookingForward
        {
            get => isLookingForward;
            set => isLookingForward = value;
        }

        public List<RoadSegment> Path
        {
            get => path;
            set => path = value;
        }

        public bool MoveToPosition { get; set; }
        public Vector3 DestinationPosition { get; set; }

        #endregion

        #region UnityEvents

        protected override void Start()
        {
            base.Start();
            _transform ??= transform;
        }


        protected override void Update()
        {
        }

        #endregion

        [Button(ButtonSizes.Gigantic)]
        public void StartMove()
        {
            Stop();
            pathCreator = null;
            DoMove();
        }

        private void DoMove()
        {
            if (path == null)
            {
                Debug.LogError("Path is null");
                return;
            }

            if (pathCreator == null)
            {
                _currentPathIndex = 0;
                pathCreator = path[_currentPathIndex].PathCreator;
            }

            distanceTravelled = 0;

            if (MoveToPosition && IsLastSegment)
            {
                _length = pathCreator.path.GetClosestDistanceAlongPath(DestinationPosition);
            }
            else
            {
                _length = pathCreator.path.length;
            }

            var duration = (_length - distanceTravelled) / speed;
            var fromValue = distanceTravelled;

            _currentTween = DOVirtual.Float(fromValue, _length, duration, value =>
            {
                distanceTravelled = value;

                _transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                _transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);

                if (!isLookingForward)
                {
                    _transform.forward = -_transform.forward;
                }
            }).SetEase(Ease.Linear).OnComplete(NextPath);
        }

        private void NextPath()
        {
            _currentTween?.Kill();
            _currentPathIndex++;
            _currentPathIndex %= path.Count;
            pathCreator = path[_currentPathIndex].PathCreator;
            if (endOfTotalPathInstruction == EndOfPathInstruction.Stop && _currentPathIndex == 0)
            {
                completeAction?.Invoke();
                return;
            }

            DoMove();
        }

        [Button]
        public void Pause()
        {
            _currentTween?.Pause();
        }

        [Button]
        public void Resume()
        {
            _currentTween?.Play();
        }

        [Button]
        public void Stop()
        {
            _currentTween?.Kill();
        }

        #region Test

        [SerializeField] private Transform point1;
        [SerializeField] private Transform point2;

        [Button(ButtonSizes.Gigantic)]
        public void DoMoveTask()
        {
            AddTaskMoveForward(point1.position);
            AddTaskMoveBackward(point2.position);
            taskHandler.Work();
        }

        #endregion

        #region TaskHandler

        public void AddTaskMoveForward(Vector3 target)
        {
            var task = new MoveTask(this, roadManager, target, true, true);
            taskHandler.AddTask(task, taskHandler);
        }

        public void AddTaskMoveBackward(Vector3 target)
        {
            var task = new MoveTask(this, roadManager, target, false, true);
            taskHandler.AddTask(task, taskHandler);
        }

        #endregion
    }
}