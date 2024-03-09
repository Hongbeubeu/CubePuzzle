using UnityEngine;

namespace CubePuzzle.Gameplay
{
    public class RaycastController : MonoBehaviour
    {
        [SerializeField] private Camera _mainCam;
        [SerializeField] private BoardController _boardController;

        private Transform _boardTrans;

        private void Start()
        {
            if (_mainCam == null)
            {
                _mainCam = Camera.main;
            }

            _boardTrans ??= _boardController.transform;
        }

        private void Update()
        {
            var plane = new Plane(_boardTrans.position, _boardTrans.position + _boardTrans.forward,
                _boardTrans.position + _boardTrans.right);
            var ray = _mainCam.ScreenPointToRay(Input.mousePosition);

            if (!plane.Raycast(ray, out var enter)) return;

            if (GameController.Instance != null) GameController.Instance.SetBlockPositionRotation(ray.GetPoint(enter));
            if (_boardController != null) _boardController.OnSelectTile(ray.GetPoint(enter));
        }
    }
}