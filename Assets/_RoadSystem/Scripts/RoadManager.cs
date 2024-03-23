using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoadSystem
{
    public class RoadManager : MonoBehaviour
    {
        #region Fields

        [SerializeField] private List<RoadSegment> roadSegments = new();
        [SerializeField] private Transform source;
        [SerializeField] private Transform destination;
        [SerializeField] private Vehicle vehicle;

        #endregion


        [Button(ButtonSizes.Gigantic)]
        private void Test()
        {
            var path = FindPath(source.position, destination.position);
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

        public List<RoadSegment> FindPath(Vector3 src, Vector3 dest)
        {
            var fromSegment = FindNearestSegment(src);
            var targetSegment = FindNearestSegment(dest);

            if (fromSegment == targetSegment)
            {
                return new List<RoadSegment> { fromSegment };
            }

            var path = FindPath(fromSegment, targetSegment);

            if (Vector3.Distance(path[1].PathCreator.path.GetClosestPointOnPath(src), src) < 0.01f
             && Vector3.Distance(path[0].PathCreator.path.GetClosestPointOnPath(src), src) < 0.01f)
            {
                path.RemoveAt(0);
            }

            if (path.Count <= 1)
                return path;

            if (Vector3.Distance(path[^1].PathCreator.path.GetClosestPointOnPath(dest), dest) < 0.01f
             && Vector3.Distance(path[^2].PathCreator.path.GetClosestPointOnPath(dest), dest) < 0.01f)
            {
                path.RemoveAt(path.Count - 1);
            }

            return path;
        }

        private List<RoadSegment> FindPath(RoadSegment src, RoadSegment dest)
        {
            if (src == dest)
            {
                return new List<RoadSegment> { src };
            }

            var queue = new Queue<RoadSegment>();
            var parentMap = new Dictionary<RoadSegment, RoadSegment>();
            var visited = new HashSet<RoadSegment>();

            queue.Enqueue(src);
            visited.Add(src);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == dest)
                    break;

                foreach (var neighbor in current.RoadOuts)
                {
                    if (visited.Contains(neighbor)) continue;

                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    parentMap[neighbor] = current;
                }
            }

            if (!parentMap.ContainsKey(dest)) return null;

            var path = new List<RoadSegment>();
            var backtrack = dest;
            while (backtrack != src)
            {
                path.Add(backtrack);
                backtrack = parentMap[backtrack];
            }

            path.Add(src);
            path.Reverse();

            return path;
        }

        private RoadSegment FindNearestSegment(Vector3 position)
        {
            var minDistance = Mathf.Infinity;
            RoadSegment result = null;

            foreach (var segment in roadSegments)
            {
                var distance = segment.FindClosestPoint(position);
                if (distance > minDistance) continue;
                minDistance = distance;
                result = segment;
            }

            return result;
        }

#if UNITY_EDITOR

        [Button(ButtonSizes.Large)]
        private void ValidateRoadSegments()
        {
            roadSegments.Clear();
            roadSegments = transform.GetComponentsInChildren<RoadSegment>().ToList();
        }

        private void OnDrawGizmos()
        {
            if (source == null) return;
            var position = source.position;
            var sourceSegment = FindNearestSegment(position);
            var p1 = sourceSegment.PathCreator.path.GetClosestPointOnPath(position);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, p1);

            if (destination == null) return;
            var position1 = destination.position;
            var targetSegment = FindNearestSegment(position1);
            var p2 = targetSegment.PathCreator.path.GetClosestPointOnPath(position1);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(position1, p2);
        }

#endif
    }
}