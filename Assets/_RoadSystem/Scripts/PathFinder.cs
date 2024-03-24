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

            if (fromSegment == targetSegment)
            {
                var distance0 = fromSegment.Path.GetClosestDistanceAlongPath(src);
                var distance1 = fromSegment.Path.GetClosestDistanceAlongPath(dest);

                return distance0 > distance1 && fromSegment.RoadType == RoadType.OneWay ? null : new List<RoadSegment> { fromSegment };
            }

            if (fromSegment.IsConnectTo(targetSegment))
            {
                if(Vector3.Distance(fromSegment.Path.GetClosestPointOnPath(dest), dest) < 0.01f
                && Vector3.Distance(targetSegment.Path.GetClosestPointOnPath(dest), dest) < 0.01f)
                {
                    var distance0 = fromSegment.Path.GetClosestDistanceAlongPath(src);
                    var distance1 = fromSegment.Path.GetClosestDistanceAlongPath(dest);
                    return fromSegment.RoadType == RoadType.OneWay && distance0 > distance1 ? null : new List<RoadSegment> { fromSegment };
                }
                
                if(Vector3.Distance(fromSegment.Path.GetClosestPointOnPath(src), src) < 0.01f
                && Vector3.Distance(targetSegment.Path.GetClosestPointOnPath(src), src) < 0.01f)
                {
                    var distance0 = targetSegment.Path.GetClosestDistanceAlongPath(src);
                    var distance1 = targetSegment.Path.GetClosestDistanceAlongPath(dest);
                    return targetSegment.RoadType == RoadType.OneWay && distance0 > distance1 ? null : new List<RoadSegment> { targetSegment };
                }
            }

            var path = FindPath(fromSegment, src, targetSegment, dest);

            if (path == null)
            {
                return null;
            }

            if (Vector3.Distance(path[1].Path.GetClosestPointOnPath(src), src) < 0.01f
             && Vector3.Distance(path[0].Path.GetClosestPointOnPath(src), src) < 0.01f)
            {
                path.RemoveAt(0);
            }

            if (path.Count <= 1)
            {
                var distance0 = fromSegment.Path.GetClosestDistanceAlongPath(src);
                var distance1 = fromSegment.Path.GetClosestDistanceAlongPath(dest);

                return distance0 > distance1 && fromSegment.RoadType == RoadType.OneWay ? null : path;
            }

            if (Vector3.Distance(path[^1].Path.GetClosestPointOnPath(dest), dest) < 0.01f
             && Vector3.Distance(path[^2].Path.GetClosestPointOnPath(dest), dest) < 0.01f)
            {
                path.RemoveAt(path.Count - 1);
            }

            if (path.Count <= 1)
            {
                var distance0 = fromSegment.Path.GetClosestDistanceAlongPath(src);
                var distance1 = fromSegment.Path.GetClosestDistanceAlongPath(dest);

                return distance0 > distance1 && fromSegment.RoadType == RoadType.OneWay ? null : path;
            }

            return path;
        }

        private List<RoadSegment> FindPath(RoadSegment src, Vector3 srcPoint, RoadSegment dest, Vector3 destPoint)
        {
            if (src == dest)
            {
                return new List<RoadSegment> { src };
            }

            var queue = new Queue<RoadSegment>();
            var parentMap = new Dictionary<RoadSegment, RoadSegment>();
            var visited = new HashSet<RoadSegment>();

            var currentDistance = src.Path.GetClosestDistanceAlongPath(srcPoint);
            var lastDistance = dest.Path.GetClosestDistanceAlongPath(destPoint);

            queue.Enqueue(src);
            visited.Add(src);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (parentMap.TryGetValue(current, out var parent))
                {
                    current.GetClosestDistanceConnectToOtherSegment(parent, connectedAt: out currentDistance);
                }
                
                if (current == dest)
                {
                    break;
                }

                foreach (var neighbor in current.RoadOuts)
                {
                    if (visited.Contains(neighbor)) continue;
                    current.GetClosestDistanceConnectToOtherSegment(neighbor, out var distance);
                    if (current.RoadType == RoadType.OneWay)
                    {
                        if (distance < currentDistance) continue;
                    }

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
#endif
    }
}