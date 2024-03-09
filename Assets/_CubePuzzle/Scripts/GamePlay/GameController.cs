using QuickEngine.Common;
using UnityEngine;

namespace CubePuzzle.Gameplay
{
    public class GameController : Singleton<GameController>
    {
        public bool IsHoldBlock;

        [SerializeField] private Transform block;
        [SerializeField] private Transform board;

        public void SetBlockPositionRotation(Vector3 position)
        {
            position += board.up * 2f;
            block.position = position;
            block.rotation = board.rotation;
        }
    }
}