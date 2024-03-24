using System.Collections.Generic;
using System.Linq;
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

        [SerializeField] private List<RoadSegment> roadIns = new();
        [SerializeField] private List<RoadSegment> roadOuts = new();

        public RoadType RoadType => roadType;
        public PathCreator PathCreator => pathCreator;
        public VertexPath Path => PathCreator.path;

        public List<RoadSegment> RoadIns => roadIns;
        public List<RoadSegment> RoadOuts => roadOuts;

        #endregion

        public bool IsConnectBy(RoadSegment src)
        {
            return RoadIns.Contains(src);
        }

        public bool IsConnectTo(RoadSegment target)
        {
            return RoadOuts.Contains(target);
        }

        public float FindClosestPoint(Vector3 target)
        {
            return Vector3.Distance(pathCreator.path.GetClosestPointOnPath(target), target);
        }

        public bool GetDistanceConnectToOtherSegment(RoadSegment toRoadSegment, out float connectedAt)
        {
            connectedAt = 0;
            if (!RoadOuts.Contains(toRoadSegment))
            {
                return false;
            }

            var point0 = Path.GetPointAtTime(0, EndOfPathInstruction.Stop);
            var point1 = Path.GetPointAtTime(1, EndOfPathInstruction.Stop);

            var point2 = toRoadSegment.Path.GetClosestPointOnPath(point0);
            var point3 = toRoadSegment.Path.GetClosestPointOnPath(point1);

            var distance0 = Vector3.Distance(point0, point2);
            var distance1 = Vector3.Distance(point1, point3);

            connectedAt = distance0 < distance1 ? 0 : Path.length;
            return true;
        }

        public bool GetClosestDistanceConnectToOtherSegment(RoadSegment toRoadSegment, out float connectedAt)
        {
            connectedAt = 0;
            if (!RoadOuts.Contains(toRoadSegment))
            {
                return false;
            }

            var point0 = toRoadSegment.Path.GetPointAtTime(0, EndOfPathInstruction.Stop);
            var point1 = toRoadSegment.Path.GetPointAtTime(1, EndOfPathInstruction.Stop);

            var point2 = Path.GetClosestPointOnPath(point0);
            var point3 = Path.GetClosestPointOnPath(point1);

            var distance0 = Vector3.Distance(point0, point2);
            var distance1 = Vector3.Distance(point1, point3);

            connectedAt = distance0 < distance1
                              ? Path.GetClosestDistanceAlongPath(point0)
                              : Path.GetClosestDistanceAlongPath(point1);
            return true;
        }

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

            if (IsOnRoadSegment(destStartPoint))
                return destStartPoint;
            if (IsOnRoadSegment(destEndPoint))
                return destStartPoint;

            if (targetSegment.IsOnRoadSegment(srcStartPoint))
                return srcStartPoint;
            return srcEndPoint;
        }

        public bool IsOnRoadSegment(Vector3 checkPoint)
        {
            var closestPointOnPath = Path.GetClosestPointOnPath(checkPoint);
            return Vector3.Distance(closestPointOnPath, checkPoint) <= 0.01f;
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