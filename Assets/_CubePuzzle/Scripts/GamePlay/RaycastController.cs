using UnityEngine;

namespace CubePuzzle.Gameplay
{
    public class RaycastController : MonoBehaviour
    {
        [SerializeField] private Camera _mainCam;
        [SerializeField] private GameObject _point;

        private void Start()
        {
            if (_mainCam == null)
            {
                _mainCam = Camera.main;
            }
        }

        private void Update()
        {
            var ray = _mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, _mainCam.farClipPlane))
            {
                _point.transform.position = hit.point;
            }
        }
    }
}