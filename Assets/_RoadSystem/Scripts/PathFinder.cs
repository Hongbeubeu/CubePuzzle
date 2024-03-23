using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoadSystem
{
    public class PathFinder : MonoBehaviour
    {
        [SerializeField] private List<RoadSegment> _roadSegments;

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

        public RoadSegment FindNearestSegment(Vector3 position)
        {
            var minDistance = Mathf.Infinity;
            RoadSegment result = null;

            foreach (var segment in _roadSegments)
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
            _roadSegments.Clear();
            _roadSegments = transform.GetComponentsInChildren<RoadSegment>().ToList();
        }
#endif
    }
}