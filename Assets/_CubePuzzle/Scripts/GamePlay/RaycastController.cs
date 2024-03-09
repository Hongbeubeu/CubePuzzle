﻿using UnityEngine;

namespace CubePuzzle.Gameplay
{
    public class RaycastController : MonoBehaviour
    {
        [SerializeField] private Camera _mainCam;
        [SerializeField] private BoardController _boardController;

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

            if (!Physics.Raycast(ray, out var hit, _mainCam.farClipPlane)) return;

            if (GameController.Instance != null) GameController.Instance.SetBlockPositionRotation(hit.point);
            if (_boardController != null) _boardController.OnSelectTile(hit.point);
        }
    }
}