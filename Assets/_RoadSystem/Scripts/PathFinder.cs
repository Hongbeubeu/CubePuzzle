using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoadSystem
{
    public class PathFinder : MonoBehaviour
    {
        [SerializeField] private List<RoadSegment> roadSegments;

        public List<RoadSegment> FindPath(Vector3 src, Vector3 dest)
        {
            var fromSegment = FindNearestSegment(src);
            var targetSegment = FindNearestSegment(dest);

            // Actually find path from fromSegment to toSegment
            var path = FindPath(fromSegment, src, targetSegment, dest);

            if (path == null)
            {
                return null;
            }

            if (path.Count == 1)
            {
                return path[0].NeedToCheckWayType && !path[0].CompareDistance(src, dest)
                           ? null
                           : path;
            }

            return path;
        }

        private List<RoadSegment> FindPath(RoadSegment src, Vector3 srcPoint, RoadSegment dest, Vector3 destPoint)
        {
            if (src == dest)
            {
                return src.NeedToCheckWayType && !src.CompareDistance(srcPoint, destPoint)
                           ? null
                           : new List<RoadSegment> { src };
            }

            if (src.IsConnectTo(dest))
            {
                return new List<RoadSegment> { src, dest };
            }

            var queue = new Queue<RoadSegment>();
            var parentMap = new Dictionary<RoadSegment, RoadSegment>();
            var visited = new HashSet<RoadSegment>();

            var currentDistance = src.Path.GetClosestDistanceAlongPath(srcPoint);

            queue.Enqueue(src);
            visited.Add(src);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (parentMap.TryGetValue(current, out var parent))
                {
                    var connectedPoint = RoadSegmentHelper.GetCrossingPoint(current, parent);
                    currentDistance = current.Path.GetClosestDistanceAlongPath(connectedPoint);
                }

                if (current == dest)
                {
                    break;
                }

                foreach (var neighbor in current.ConnectToRoads)
                {
                    if (visited.Contains(neighbor)) continue;

                    var connectedPoint = RoadSegmentHelper.GetCrossingPoint(current, neighbor);
                    var distance = current.Path.GetClosestDistanceAlongPath(connectedPoint);
                    if (current.NeedToCheckWayType)
                    {
                        if (distance < currentDistance) continue;
                    }

                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    parentMap[neighbor] = current;
                }
            }

            if (!parentMap.ContainsKey(dest)) return null;

            var path = BackTrackingPath(src, dest, parentMap);

            return path;
        }

        private List<RoadSegment> BackTrackingPath(RoadSegment src, RoadSegment dest,
            Dictionary<RoadSegment, RoadSegment> parentMap)
        {
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

            foreach (var segment in roadSegments)
            {
                var distance = segment.ClosestDistanceToSegment(position);
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
#endif
    }
}