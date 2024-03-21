using PathCreation.Examples;
using UnityEngine;

namespace RoadSystem
{
    public class Vehicle : PathFollower
    {
        [SerializeField] private Transform target;

        protected override void Update()
        {
            base.Update();
            // if (pathCreator != null)
            // {
            //     distanceTravelled += speed * Time.deltaTime;
            //     var targetPosition = pathCreator.path.GetClosestDistanceAlongPath(target.position);
            //     if (distanceTravelled >= targetPosition)
            //     {
            //         distanceTravelled = targetPosition;
            //     }
            //     transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            //     transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            // }
        }
    }
}