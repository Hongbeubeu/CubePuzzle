using Sirenix.OdinInspector;
using UnityEngine;

namespace RoadSystem
{
    public class RoadManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Transform source;
        [SerializeField] private Transform destination;
        [SerializeField] private Vehicle vehicle;
        [SerializeField]private PathFinder pathFinder;

        public PathFinder PathFinder => pathFinder;

        #endregion


        [Button(ButtonSizes.Gigantic)]
        private void Test()
        {
            var path = pathFinder.FindPath(source.position, destination.position);
            if (path == null)
            {
                Debug.LogError("Not found path");
                return;
            }

            foreach (var p in path)
            {
                Debug.Log(p.name);
            }

            vehicle.Path = path;
            vehicle.Stop();
            vehicle.StartMove();
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (source == null) return;
            var position = source.position;
            var sourceSegment = pathFinder.FindNearestSegment(position);
            var p1 = sourceSegment.PathCreator.path.GetClosestPointOnPath(position);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, p1);

            if (destination == null) return;
            var position1 = destination.position;
            var targetSegment = pathFinder.FindNearestSegment(position1);
            var p2 = targetSegment.PathCreator.path.GetClosestPointOnPath(position1);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(position1, p2);
        }

#endif
    }
}