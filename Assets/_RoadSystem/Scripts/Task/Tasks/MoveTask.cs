using UnityEngine;

namespace RoadSystem
{
    public class MoveTask : BaseTask
    {
        private readonly Vehicle _vehicle;
        private readonly RoadManager _roadManager;
        private readonly bool _lookingForward;
        private readonly Vector3 _target;

        public MoveTask(Vehicle vehicle, RoadManager roadManager, Vector3 target, bool lookingForward)
        {
            _vehicle = vehicle;
            _roadManager = roadManager;
            _target = target;
            _lookingForward = lookingForward;
        }

        public override void Start()
        {
            DoMove();
        }

        private void DoMove()
        {
            var path = _roadManager.FindPath(_vehicle.transform.position, _target);
            _vehicle.Path = path;
            _vehicle.LookingForward = _lookingForward;
            _vehicle.StartMove();
            _vehicle.CompleteAction += OnCompleted;
        }

        public override void OnCompleted()
        {
            base.OnCompleted();
            ConcludeTask();
        }
    }
}