using UnityEngine;

namespace CubePuzzle.Gameplay
{
    public class RotateBoardController : MonoBehaviour
    {
        [SerializeField] private Camera _mainCam;
        [SerializeField] private float _rotationSpeed = 10f;
        [SerializeField] private Transform _thisTransform;
        [SerializeField] private GameController _gameController;
        private Vector3 _previousMousePosition;
        private bool _isDragging;
        private bool _isZooming;

        private void Start()
        {
            if (_mainCam == null)
            {
                _mainCam = Camera.main;
            }

            if (_thisTransform == null)
            {
                _thisTransform = transform;
            }
        }

        private void Update()
        {
            if (_gameController.IsHoldBlock) return;

            HandleInput();

            if (!_isDragging) return;

            DoRotate();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                // Capture the mouse position when the user starts dragging
                _isDragging = true;
                _previousMousePosition = Input.mousePosition;
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                // Stop dragging when the mouse button is released
                _isDragging = false;
            }
        }

        private void DoRotate()
        {
            // Calculate the delta movement of the mouse
            var mouseDelta = Input.mousePosition - _previousMousePosition;

            // Rotate the object based on the mouse movement
            if (Mathf.Abs(mouseDelta.x) > Mathf.Abs(mouseDelta.y))
            {
                _thisTransform.Rotate(_thisTransform.up, -mouseDelta.x * _rotationSpeed * Time.deltaTime, Space.World);
            }
            else
            {
                _thisTransform.Rotate(_mainCam.transform.right, mouseDelta.y * _rotationSpeed * Time.deltaTime,
                    Space.World);
            }

            // Update the previous mouse position
            _previousMousePosition = Input.mousePosition;
        }
    }
}