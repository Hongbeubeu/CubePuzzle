using UnityEngine;

namespace CubePuzzle.Gameplay
{
    public class ZoomBoardController : MonoBehaviour
    {
        [SerializeField] private Camera mainCam;
        public float zoomSpeed = 5f;
        public float minFOV = 20f;
        public float maxFOV = 60f;

        private void Start()
        {
            if (mainCam == null)
            {
                mainCam = Camera.main;
            }
        }

        private void Update()
        {
            HandleZoom();
        }

        private void HandleZoom()
        {
            // Get the scroll wheel input
            var scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollWheelInput == 0)
                return;

            // Calculate the new field of view based on the scroll input
            var newFOV = mainCam.fieldOfView - scrollWheelInput * zoomSpeed;

            // Clamp the field of view to the specified range
            newFOV = Mathf.Clamp(newFOV, minFOV, maxFOV);

            // Set the new field of view
            mainCam.fieldOfView = newFOV;
        }
    }
}