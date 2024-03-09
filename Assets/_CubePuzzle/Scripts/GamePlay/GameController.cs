using QuickEngine.Common;
using UnityEngine;

namespace CubePuzzle.Gameplay
{
    public class GameController : Singleton<GameController>
    {
        public bool IsHoldBlock { get; private set; }

        private Transform block;
        [SerializeField] private Transform board;

        public void SetBlockPositionRotation(Vector3 position)
        {
            if (!IsHoldBlock || block == null) return;

            position += board.up * 2f;
            block.position = position;
            block.rotation = board.rotation;
        }

        private void Start()
        {
            RefreshBlock();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                IsHoldBlock = !IsHoldBlock;
                RefreshBlock();
            }
        }

        private void RefreshBlock()
        {
            if (IsHoldBlock)
            {
                block = GameManager.Instance.Database.ObjectPooler.InstantiateCube().transform;
            }
            else if (block != null)
            {
                GameManager.Instance.Database.ObjectPooler.ReturnPoolCube(block.gameObject);
                block = null;
            }
        }
    }
}