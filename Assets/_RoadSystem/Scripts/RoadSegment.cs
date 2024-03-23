using System.Collections.Generic;
using PathCreation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoadSystem
{
    [RequireComponent(typeof(PathCreator))]
    public class RoadSegment : MonoBehaviour
    {
        [SerializeField] private RoadType roadType;
        [SerializeField] private PathCreator pathCreator;

        [SerializeField] private List<RoadSegment> roadIns = new();
        [SerializeField] private List<RoadSegment> roadOuts = new();

        public PathCreator PathCreator => pathCreator;

        public List<RoadSegment> RoadIns => roadIns;
        public List<RoadSegment> RoadOuts => roadOuts;

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

            var point0 = PathCreator.path.GetPointAtTime(0, EndOfPathInstruction.Stop);
            var point1 = PathCreator.path.GetPointAtTime(1, EndOfPathInstruction.Stop);

            var point2 = toRoadSegment.PathCreator.path.GetClosestPointOnPath(point0);
            var point3 = toRoadSegment.PathCreator.path.GetClosestPointOnPath(point1);

            var distance0 = Vector3.Distance(point0, point2);
            var distance1 = Vector3.Distance(point1, point3);

            connectedAt = distance0 < distance1 ? 0 : PathCreator.path.length;
            return true;
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