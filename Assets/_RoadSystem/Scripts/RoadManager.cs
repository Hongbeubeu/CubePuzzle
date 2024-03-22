using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoadSystem
{
    public class RoadManager : MonoBehaviour
    {
        [SerializeField] private List<RoadSegment> _roadSegments = new();

        [SerializeField] private Transform _source;
        [SerializeField] private Transform _target;

        [SerializeField] private Vehicle _vehicle;


        [Button(ButtonSizes.Gigantic)]
        private void Test()
        {
            var path = FindPath(_source.position, _target.position);
            if (path == null)
            {
                Debug.LogError("Not found path");
                return;
            }

            foreach (var p in path)
            {
                Debug.Log(p.name);
            }

            _vehicle.Path = path;
            _vehicle.Stop();
            _vehicle.StartMove();
        }

        private List<RoadSegment> FindPath(Vector3 source, Vector3 target)
        {
            var fromSegment = FindNearestSegment(source);
            var targetSegment = FindNearestSegment(target);
            return FindPath(fromSegment, targetSegment);
        }

        private List<RoadSegment> FindPath(RoadSegment source, RoadSegment target)
        {
            if (source == target)
            {
                return new List<RoadSegment> { source };
            }

            var queue = new Queue<RoadSegment>();
            var parentMap = new Dictionary<RoadSegment, RoadSegment>();
            var visited = new HashSet<RoadSegment>();

            queue.Enqueue(source);
            visited.Add(source);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == target)
                    break;

                foreach (var neighbor in current.RoadOuts)
                {
                    if (visited.Contains(neighbor)) continue;

                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    parentMap[neighbor] = current;
                }
            }

            if (!parentMap.ContainsKey(target)) return null;

            var path = new List<RoadSegment>();
            var backtrack = target;
            while (backtrack != source)
            {
                path.Add(backtrack);
                backtrack = parentMap[backtrack];
            }

            path.Add(source);
            path.Reverse();

            return path;
        }


        private RoadSegment FindNearestSegment(Vector3 position)
        {
            var currentIndex = 0;
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

        private void OnDrawGizmos()
        {
            var sourceSegment = FindNearestSegment(_source.position);
            var point1 = sourceSegment.PathCreator.path.GetClosestPointOnPath(_source.position);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_source.position, point1);
            var targetSegment = FindNearestSegment(_target.position);
            var point2 = targetSegment.PathCreator.path.GetClosestPointOnPath(_target.position);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(_target.position, point2);
        }

#endif
    }
}