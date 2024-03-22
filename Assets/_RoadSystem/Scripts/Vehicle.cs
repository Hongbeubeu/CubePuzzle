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
        [SerializeField] private EndOfPathInstruction _endOfTotalPathInstruction;
        [SerializeField] private List<RoadSegment> path;

        private float length;
        private int currentPathIndex;
        private Tween currentTween;

        public List<RoadSegment> Path
        {
            get => path;
            set => path = value;
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
            if (path == null)
            {
                Debug.LogError("Path is null");
                return;
            }

            if (pathCreator == null)
            {
                currentPathIndex = 0;
                pathCreator = path[currentPathIndex].PathCreator;
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
            }).SetEase(Ease.Linear).OnComplete(NextPath);
        }

        private void NextPath()
        {
            currentTween?.Kill();
            currentPathIndex++;
            currentPathIndex %= path.Count;
            pathCreator = path[currentPathIndex].PathCreator;
            if (_endOfTotalPathInstruction == EndOfPathInstruction.Stop && currentPathIndex == 0) return;
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
    }
}