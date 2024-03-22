using System.Collections.Generic;
using PathCreation;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RoadSystem
{
    [RequireComponent(typeof(PathCreator))]
    public class RoadSegment : MonoBehaviour
    {
        [SerializeField] private PathCreator _pathCreator;

        [SerializeField] private List<RoadSegment> _roadIns = new();
        [SerializeField] private List<RoadSegment> _roadOuts = new();

        public PathCreator PathCreator => _pathCreator;

        public List<RoadSegment> RoadIns => _roadIns;
        public List<RoadSegment> RoadOuts => _roadOuts;

        public float FindClosestPoint(Vector3 target)
        {
            return Vector3.Distance(_pathCreator.path.GetClosestPointOnPath(target), target);
        }

#if UNITY_EDITOR

        [Button]
        private void ValidatePathCreator()
        {
            _pathCreator = GetComponent<PathCreator>();
        }

#endif
    }
}