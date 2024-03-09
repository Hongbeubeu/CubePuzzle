using UnityEngine;

namespace CubePuzzle.Gameplay
{
    [CreateAssetMenu(menuName = "Cube Puzzle/Object Pooler", fileName = "ObjectPooler")]
    public class ObjectPooler : ScriptableObject
    {
        public GameObject cubePrefab;

        public GameObject InstantiateCube()
        {
            return FastPoolManager.GetPool(cubePrefab).FastInstantiate();
        }

        public void ReturnPoolCube(GameObject cube)
        {
            FastPoolManager.GetPool(cubePrefab).FastDestroy(cube);
        }
    }
}