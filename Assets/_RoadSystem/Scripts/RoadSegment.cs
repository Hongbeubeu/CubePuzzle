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

        [SerializeField] private List<RoadSegment> roadIns = new();
        [SerializeField] private List<RoadSegment> roadOuts = new();

        public PathCreator PathCreator => pathCreator;
        public VertexPath Path => PathCreator.path;

        public List<RoadSegment> RoadIns => roadIns;
        public List<RoadSegment> RoadOuts => roadOuts;

        #endregion

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