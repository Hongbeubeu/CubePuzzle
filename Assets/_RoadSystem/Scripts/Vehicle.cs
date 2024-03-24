using System;
using System.Collections.Generic;
using DG.Tweening;
using PathCreation;
using PathCreation.Examples;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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

        private int _currentPathIndex;
        private RoadSegment _currentRoadSegment;
        private Tween _currentTween;
        private Transform _transform;

        public Action completeAction;
        private bool IsLastSegment => _currentPathIndex == path.Count - 1;

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

        public void StartMove()
        {
            Stop();
            _currentRoadSegment = null;
            DoMove();
        }

        private void DoMove()
        {
            if (!VerifyPath()) return;

            // Get closet distance along path
            var fromDistance = _currentRoadSegment.Path.GetClosestDistanceAlongPath(_transform.position);

            // Find distance where vehicle run to
            float toDistance;
            if (MoveToPosition && IsLastSegment)
            {
                toDistance = _currentRoadSegment.Path.GetClosestDistanceAlongPath(DestinationPosition);
            }
            else
            {
                toDistance =
                    _currentRoadSegment.GetClosestDistanceConnectToOtherSegment(Path[_currentPathIndex + 1], out var length)
                        ? length
                        : _currentRoadSegment.Path.length;
            }

            var duration = Mathf.Abs(toDistance - fromDistance) / speed;

            _isRunInReverse = fromDistance > toDistance;

            _currentTween = DOVirtual.Float(fromDistance, toDistance, duration, OnVirtualUpdate)
                                     .SetEase(Ease.Linear)
                                     .OnComplete(NextPath);
        }

        private bool VerifyPath()
        {
            if (path.IsNullOrEmpty())
            {
                Debug.LogError("Path is null or empty");
                return false;
            }

            if (_currentRoadSegment != null) return true;

            _currentPathIndex = 0;
            _currentRoadSegment = path[_currentPathIndex];

            return true;
        }

        private bool _isRunInReverse;

        private void OnVirtualUpdate(float travelled)
        {
            _transform.position =
                _currentRoadSegment.Path.GetPointAtDistance(travelled, endOfPathInstruction);
            _transform.rotation =
                _currentRoadSegment.Path.GetRotationAtDistance(travelled, endOfPathInstruction);

            if (endOfPathInstruction is EndOfPathInstruction.BackLoop or EndOfPathInstruction.BackStop)
            {
                _transform.forward = -_transform.forward;
            }

            if (_isRunInReverse)
            {
                _transform.forward = -_transform.forward;
            }

            if (!isLookingForward)
            {
                _transform.forward = -_transform.forward;
            }
        }

        private void NextPath()
        {
            _currentTween?.Kill();
            _currentPathIndex++;
            _currentPathIndex %= path.Count;
            _currentRoadSegment = path[_currentPathIndex];
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
        public void DoMoveTask1()
        {
            AddTaskMoveForward(point1.position);
            taskHandler.Work();
        }

        [Button(ButtonSizes.Gigantic)]
        public void DoMoveTask2()
        {
            AddTaskMoveBackward(point2.position);
            taskHandler.Work();
        }

        #endregion

        #region TaskHandler

        public void AddTaskMoveForward(Vector3 target)
        {
            var task = new MoveTask(this, roadManager.PathFinder, target, true, true);
            taskHandler.AddTask(task, taskHandler);
        }

        public void AddTaskMoveBackward(Vector3 target)
        {
            var task = new MoveTask(this, roadManager.PathFinder, target, false, true);
            taskHandler.AddTask(task, taskHandler);
        }

        public void FreeAllTask()
        {
            taskHandler.FreeAllTask();
        }
        #endregion
    }
}