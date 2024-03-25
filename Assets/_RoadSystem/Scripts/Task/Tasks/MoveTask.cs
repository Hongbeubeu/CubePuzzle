using UnityEngine;

namespace RoadSystem
{
    public class MoveTask : BaseTask
    {
        private readonly Vehicle _vehicle;
        private readonly PathFinder _pathFinder;
        private readonly bool _lookingForward;
        private readonly Vector3 _destination;
        private readonly bool _moveToPosition;

        public MoveTask(Vehicle vehicle, PathFinder pathFinder, Vector3 destination, bool lookingForward,
            bool moveToPosition = false)
        {
            _vehicle = vehicle;
            _pathFinder = pathFinder;
            _destination = destination;
            _lookingForward = lookingForward;
            _moveToPosition = moveToPosition;
        }

        public override void Start()
        {
            DoMove();
        }

        private void DoMove()
        {
            var path = _pathFinder.FindPath(_vehicle.transform.position, _destination);
            if (path == null)
            {
                Debug.LogError($"Not found path from {_vehicle.transform.position} to {_destination}");
                _vehicle.FreeAllTask();
                return;
            }

            _vehicle.Path = path;
            _vehicle.LookingForward = _lookingForward;
            _vehicle.MoveToPosition = _moveToPosition;
            _vehicle.DestinationPosition = _destination;
            _vehicle.CompleteAction += OnCompleted;
            _vehicle.StartMove();
        }

        public override void OnCompleted()
        {
            base.OnCompleted();
            _vehicle.CompleteAction -= OnCompleted;
            ConcludeTask();
        }
    }
}