using UnityEngine;

namespace RoadSystem
{
    public static class RoadSegmentHelper
    {
        /// <summary>
        /// Check is the point is on this road segment
        /// </summary>
        /// <param name="segment">The segment to check</param>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is on this road segment</returns>
        public static bool IsPointOnRoadSegment(this RoadSegment segment, Vector3 point)
        {
            var closestPointOnPath = segment.Path.GetClosestPointOnPath(point);
            return Vector3.Distance(closestPointOnPath, point) <= 0.01f;
        }

        /// <summary>
        /// Finds the closest distance from the target position to this segment
        /// </summary>
        /// <param name="segment">The segment to find</param>
        /// <param name="target">The position for which to find the distance</param>
        /// <returns>Closest distance from the target position to this segment</returns>
        public static float ClosestDistanceToSegment(this RoadSegment segment, Vector3 target)
        {
            return Vector3.Distance(segment.Path.GetClosestPointOnPath(target), target);
        }

        /// <summary>
        /// Finds the connected point from this segment to the target segment
        /// </summary>
        /// <param name="firstSegment">The segment to which connected points need to be found </param>
        /// <param name="secondSegment">The segment to which connected points need to be found</param>
        /// <returns>The point that connects two segments</returns>
        public static Vector3 GetCrossingPoint(RoadSegment firstSegment, RoadSegment secondSegment)
        {
            if (!firstSegment.IsConnectTo(secondSegment))
            {
                Debug.LogError($"{firstSegment} not connected to {secondSegment}");
                return Vector3.zero;
            }

            var srcStartPoint = firstSegment.Path.GetPointAtDistance(0);
            var srcEndPoint = firstSegment.Path.GetPointAtDistance(firstSegment.Path.length);

            var destStartPoint = secondSegment.Path.GetPointAtDistance(0);
            var destEndPoint = secondSegment.Path.GetPointAtDistance(secondSegment.Path.length);

            if (firstSegment.IsPointOnRoadSegment(destStartPoint))
                return destStartPoint;
            if (firstSegment.IsPointOnRoadSegment(destEndPoint))
                return destStartPoint;

            if (secondSegment.IsPointOnRoadSegment(srcStartPoint))
                return srcStartPoint;
            return srcEndPoint;
        }

        /// <summary>
        /// Compare the distance from the root to firstPoint with the distance from the root to secondPoint
        /// </summary>
        /// <param name="segment">RoadSegment to be check</param>
        /// <param name="firstPoint">The first point to compare</param>
        /// <param name="secondPoint">The second point to compare</param>
        /// <returns>True if the distance from the root to the firstPoint is less than or equal to the distance from the root to the secondPoint </returns>
        public static bool CompareDistance(this RoadSegment segment, Vector3 firstPoint, Vector3 secondPoint)
        {
            var distance0 = segment.Path.GetClosestDistanceAlongPath(firstPoint); // Distance from root to fromPoint
            var distance1 = segment.Path.GetClosestDistanceAlongPath(secondPoint); // Distance from root to toPoint
            return distance0 <= distance1;
        }
    }
}