using System.Collections.Generic;
using PathCreation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace RoadSystem
{
    [RequireComponent(typeof(PathCreator))]
    public class RoadSegment : MonoBehaviour
    {
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

#if UNITY_EDITOR

        [Button]
        private void ValidatePathCreator()
        {
            pathCreator = GetComponent<PathCreator>();
        }

#endif
    }
}