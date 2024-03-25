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
        public bool NeedToCheckWayType => RoadType == RoadType.OneWay && !pathCreator.bezierPath.IsClosed;

        #endregion

        public bool IsConnectTo(RoadSegment target)
        {
            return ConnectToRoads.Contains(target);
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