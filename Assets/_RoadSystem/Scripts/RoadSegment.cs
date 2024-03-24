using System.Collections.Generic;
using PathCreation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoadSystem
{
    [RequireComponent(typeof(PathCreator))]
    public class RoadSegment : MonoBehaviour
    {
        #region Fields

        [SerializeField] private RoadType roadType;
        [SerializeField] private PathCreator pathCreator;
        [SerializeField] private List<RoadSegment> connectToRoads = new();

        public RoadType RoadType => roadType;
        public PathCreator PathCreator => pathCreator;
        public VertexPath Path => PathCreator.path;
        public List<RoadSegment> ConnectToRoads => connectToRoads;

        #endregion
        
        public bool IsConnectTo(RoadSegment target)
        {
            return ConnectToRoads.Contains(target);
        }

        /// <summary>
        /// Find the closest distance from the target position to this segment
        /// </summary>
        /// <param name="target">The position for which to find the distance</param>
        /// <returns>Closest distance from the target position to this segment</returns>
        public float ClosestDistanceToSegment(Vector3 target)
        {
            return Vector3.Distance(pathCreator.path.GetClosestPointOnPath(target), target);
        }
        
        /// <summary>
        /// Finds the connected point from this segment to the target segment
        /// </summary>
        /// <param name="targetSegment">The segment to which connected points need to be found</param>
        /// <returns>The point that connects two segments</returns>
        public Vector3 GetConnectPoint(RoadSegment targetSegment)
        {
            if (!IsConnectTo(targetSegment))
            {
                Debug.LogError($"{this} not connected to {targetSegment}");
                return Vector3.zero;
            }

            var srcStartPoint = Path.GetPointAtDistance(0);
            var srcEndPoint = Path.GetPointAtDistance(Path.length);

            var destStartPoint = targetSegment.Path.GetPointAtDistance(0);
            var destEndPoint = targetSegment.Path.GetPointAtDistance(targetSegment.Path.length);

            if (IsPointOnRoadSegment(destStartPoint))
                return destStartPoint;
            if (IsPointOnRoadSegment(destEndPoint))
                return destStartPoint;

            if (targetSegment.IsPointOnRoadSegment(srcStartPoint))
                return srcStartPoint;
            return srcEndPoint;
        }

        /// <summary>
        /// Check is the point is on this road segment
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is on this road segment</returns>
        public bool IsPointOnRoadSegment(Vector3 point)
        {
            var closestPointOnPath = Path.GetClosestPointOnPath(point);
            return Vector3.Distance(closestPointOnPath, point) <= 0.01f;
        }

        /// <summary>
        /// Compare the distance from the root to firstPoint with the distance from the root to secondPoint
        /// </summary>
        /// <param name="firstPoint">The first point to compare</param>
        /// <param name="secondPoint">The second point to compare</param>
        /// <returns>True if the distance from the root to the firstPoint is less than or equal to the distance from the root to the secondPoint </returns>
        public bool CompareDistance(Vector3 firstPoint, Vector3 secondPoint)
        {
            var distance0 = Path.GetClosestDistanceAlongPath(firstPoint); // Distance from root to fromPoint
            var distance1 = Path.GetClosestDistanceAlongPath(secondPoint); // Distance from root to toPoint
            return distance0 <= distance1;
        }
#if UNITY_EDITOR

        [Button]
        private void ValidatePathCreator()
        {
            pathCreator = GetComponent<PathCreator>();
        }

#endif
    }

    public enum RoadType : byte
    {
        OneWay,
        TwoWay
    }
}