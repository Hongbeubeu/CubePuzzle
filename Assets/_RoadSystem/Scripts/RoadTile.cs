using System.Collections.Generic;
using PathCreation;
using UnityEngine;

namespace RoadSystem
{
    public class RoadTile : PathCreator
    {
        [SerializeField] private List<RoadTile> _roadIns;
        [SerializeField] private List<RoadTile> _roadOuts;
    }
}