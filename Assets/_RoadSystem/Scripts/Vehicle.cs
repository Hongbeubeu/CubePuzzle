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

        // private Tween _currentTween;
        private Transform _transform;
        private bool _isRunInReverse;
        private Sequence _tweenSequence;
        public event Action CompleteAction;
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
            // Stop();
            _currentRoadSegment = null;
            DoMove();
        }

        private void DoMove()
        {
            if (!VerifyPath()) return;

            // Get closet distance along path
            var fromDistance = _currentRoadSegment.Path.GetClosestDistanceAlongPath(_transform.position);

            // Find distance where vehicle run to
            var toDistance = CalculateTargetDistance();

            var duration = Mathf.Abs(toDistance - fromDistance) / speed;
            if (duration <= 0.01f)
            {
                NextPath();
                return;
            }

            DoTweenMove(fromDistance, toDistance, duration);
        }

        private void DoTweenMove(float fromDistance, float toDistance, float duration)
        {
            _tweenSequence = DOTween.Sequence();
            if (!_currentRoadSegment.PathCreator.bezierPath.IsClosed || fromDistance < toDistance)
            {
                _isRunInReverse = fromDistance > toDistance;

                var tween = DOVirtual.Float(fromDistance, toDistance, duration, OnVirtualUpdate).SetEase(Ease.Linear);
                _tweenSequence.Append(tween);
                _tweenSequence.AppendCallback(NextPath);
                _tweenSequence.Play();
            }
            else // if current segment is oneway and closed then run into 2 times
            {
                var toTempDistance = _currentRoadSegment.Path.length;
                _isRunInReverse = false;
                duration = Mathf.Abs(toTempDistance - fromDistance) / speed;
                var tween0 = DOVirtual.Float(fromDistance, toTempDistance, duration, OnVirtualUpdate)
                                      .SetEase(Ease.Linear);
                _tweenSequence.Append(tween0);

                duration = Mathf.Abs(toDistance) / speed;
                var tween1 = DOVirtual
                            .Float(0, toDistance, duration, OnVirtualUpdate)
                            .SetEase(Ease.Linear);

                _tweenSequence.Append(tween1);
                _tweenSequence.AppendCallback(NextPath);
                _tweenSequence.Play();
            }
        }

        private float CalculateTargetDistance()
        {
            float toDistance;
            if (MoveToPosition && IsLastSegment)
            {
                // If current segment is last segment on path
                // then move to closest distance by DestinationPosition
                toDistance = _currentRoadSegment.Path.GetClosestDistanceAlongPath(DestinationPosition);
            }
            else
            {
                // Find connected point to next segment then run to it
                var connectedPoint =
                    RoadSegmentHelper.GetCrossingPoint(_currentRoadSegment, Path[_currentPathIndex + 1]);
                toDistance = _currentRoadSegment.Path.GetClosestDistanceAlongPath(connectedPoint);
            }

            return toDistance;
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
            // Stop();
            _currentPathIndex++;
            _currentPathIndex %= path.Count;
            _currentRoadSegment = path[_currentPathIndex];
            if (endOfTotalPathInstruction == EndOfPathInstruction.Stop && _currentPathIndex == 0)
            {
                CompleteAction?.Invoke();
                return;
            }

            DoMove();
        }

        [Button(ButtonSizes.Large)]
        public void Pause()
        {
            _tweenSequence?.Pause();
        }

        [Button(ButtonSizes.Large)]
        public void Resume()
        {
            _tweenSequence?.Play();
        }

        [Button(ButtonSizes.Large)]
        public void Stop()
        {
            _tweenSequence?.Kill();
            FreeAllTask();
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
            if (_transform.position == target) return;
            var task = new MoveTask(this, roadManager.PathFinder, target, true, true);
            taskHandler.AddTask(task, taskHandler);
        }

        public void AddTaskMoveBackward(Vector3 target)
        {
            if (_transform.position == target) return;
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