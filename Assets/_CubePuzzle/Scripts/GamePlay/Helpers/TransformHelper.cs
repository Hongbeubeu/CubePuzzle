using UnityEngine;

namespace CubePuzzle.Gameplay
{
    public static class TransformHelper
    {
        public static void ClearAllChildren(this Transform parent)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(parent.GetChild(i).gameObject);
            }
        }
    }
}