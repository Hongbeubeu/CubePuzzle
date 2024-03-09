using QuickEngine.Common;
using UnityEngine;

namespace CubePuzzle.Gameplay
{
    public class GameController : Singleton<GameController>
    {
        private Transform _block;
        private Transform _boardTrans;

        [SerializeField] private BoardController boardController;
        [SerializeField] private Camera mainCamera;

        public Camera MainCamera => mainCamera;

        public bool IsHoldBlock { get; private set; }

        private void Start()
        {
            RefreshBlock();

            _boardTrans = boardController.transform;
        }

        public void SetBlockPositionRotation(Vector3 position)
        {
            if (!IsHoldBlock || _block == null) return;

            position += _boardTrans.up * 2f;
            _block.position = position;
            _block.rotation = _boardTrans.rotation;
        }

        public void SetHoldBlock()
        {
            if (IsHoldBlock) return;
            IsHoldBlock = true;
            RefreshBlock();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                IsHoldBlock = false;
                RefreshBlock();
                boardController.OnDeselectTile();
            }
        }

        private void RefreshBlock()
        {
            if (IsHoldBlock)
            {
                _block = GameManager.Instance.Database.ObjectPooler.InstantiateCube().transform;
            }
            else if (_block != null)
            {
                GameManager.Instance.Database.ObjectPooler.ReturnPoolCube(_block.gameObject);
                _block = null;
            }
        }
    }
}