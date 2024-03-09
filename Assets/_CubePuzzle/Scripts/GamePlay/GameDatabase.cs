using UnityEngine;

namespace CubePuzzle.Gameplay
{
    [CreateAssetMenu(menuName = "Cube Puzzle/Database", fileName = "Database")]
    public class GameDatabase : ScriptableObject
    {
        [Header(nameof(ObjectPooler))] public ObjectPooler ObjectPooler;
    }
}