using System.Collections.Generic;
using DG.Tweening;
using PathCreation.Examples;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoadSystem
{
    public class Vehicle : PathFollower
    {
        [SerializeField] private List<RoadTile> path;

        [SerializeField] private Transform target;

        private float length;
        private int currentPathIndex;
        private Tween currentTween;

        protected override void Update()
        {
            // Do nothing
        }

        [Button]
        private void DoMove()
        {
            if (path == null) return;
            if (pathCreator == null)
            {
                currentPathIndex = 0;
                distanceTravelled = 0;
                pathCreator = path[currentPathIndex];
            }
            length = pathCreator.path.length;
            var duration = (length - distanceTravelled) / speed;
            var fromValue = distanceTravelled;
            currentTween?.Kill();
            currentTween = DOVirtual.Float(fromValue, length, duration, value =>
            {
                distanceTravelled = value;
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            }).SetEase(Ease.Linear).OnComplete(NextPath);
        }
        
        private void NextPath()
        {
            currentPathIndex++;
            currentPathIndex %= path.Count;
            pathCreator = path[currentPathIndex];
            distanceTravelled = 0;
            length = pathCreator.path.length;
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
    }
}