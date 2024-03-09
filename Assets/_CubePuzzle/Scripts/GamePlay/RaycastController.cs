using UnityEngine;

namespace CubePuzzle.Gameplay
{
    public class RaycastController : MonoBehaviour
    {
        private Camera _mainCam;
        private Transform _boardTrans;

        [SerializeField] private BoardController boardController;

        private void Start()
        {
            _mainCam = GameController.Instance.MainCamera;
            _boardTrans = boardController.transform;
        }

        private void Update()
        {
            if (!GameController.Instance.IsHoldBlock) return;

            var plane = new Plane(_boardTrans.position, _boardTrans.position + _boardTrans.forward,
                _boardTrans.position + _boardTrans.right);
            var ray = _mainCam.ScreenPointToRay(Input.mousePosition);

            if (!plane.Raycast(ray, out var enter)) return;

            if (GameController.Instance != null) GameController.Instance.SetBlockPositionRotation(ray.GetPoint(enter));
            if (boardController != null) boardController.OnSelectTile(ray.GetPoint(enter));
        }
    }
}