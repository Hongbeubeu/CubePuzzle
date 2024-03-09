using UnityEngine;
using UnityEngine.EventSystems;

namespace CubePuzzle.Gameplay
{
    public class RotateBoardController : MonoBehaviour
    {
        private Camera _mainCam;
        private Transform _thisTransform;
        private Vector3 _previousMousePosition;
        private bool _isDragging;
        private bool _isZooming;

        [SerializeField] private float rotationSpeed = 10f;

        private void Start()
        {
            _mainCam = GameController.Instance.MainCamera;
            _thisTransform = transform;
        }

        private void Update()
        {
            if (GameController.Instance.IsHoldBlock || !EventSystem.current.IsPointerOverGameObject()) return;

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
                _thisTransform.Rotate(_thisTransform.up, -mouseDelta.x * rotationSpeed * Time.deltaTime, Space.World);
            }
            else
            {
                _thisTransform.Rotate(_mainCam.transform.right, mouseDelta.y * rotationSpeed * Time.deltaTime,
                    Space.World);
            }

            // Update the previous mouse position
            _previousMousePosition = Input.mousePosition;
        }
    }
}