using QuickEngine.Common;
using UnityEngine;

namespace CubePuzzle.Gameplay
{
    public class GameManager : Singleton<GameManager>
    {
        [Header(nameof(GameDatabase))] public GameDatabase Database;
    }
}