using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace CubePuzzle.Gameplay
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private int _rowNumber;
        [SerializeField] private int _colNumber;
        [SerializeField] private float _cellUnitLength;
        [SerializeField] private Transform _parent;
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private List<Tile> _tiles = new();

        [Button(ButtonSizes.Gigantic)]
        private void SpawnTiles()
        {
            ClearOldTiles();
            DoSpawnTiles();
        }

        [Button(ButtonSizes.Gigantic)]
        private void DoAnimation()
        {
            StartCoroutine(AnimCoroutine());
        }

        private IEnumerator AnimCoroutine()
        {
            var count = 0;
            for (var row = 0; row < _rowNumber; row++)
            {
                for (var col = 0; col < _colNumber; col++)
                {
                    _tiles[count].DoAnimation();
                    count++;
                }

                yield return new WaitForSeconds(0.01f);
            }
        }

        private void ClearOldTiles()
        {
            _tiles.Clear();
            _parent.ClearAllChildren();
        }

        private void DoSpawnTiles()
        {
            var minX = -(_colNumber - _cellUnitLength) / 2f;
            var minY = -(_rowNumber - _cellUnitLength) / 2f;

            var currentPos = new Vector3(minX, 0, minY);

            for (var row = 0; row < _rowNumber; row++)
            {
                currentPos.x = minX;
                for (var col = 0; col < _colNumber; col++)
                {
                    var tile = PrefabUtility.InstantiatePrefab(_tilePrefab, _parent) as Tile;
                    tile.transform.localPosition = currentPos;

                    _tiles.Add(tile);
                    currentPos.x += _cellUnitLength;
                }

                currentPos.z += _cellUnitLength;
            }
        }
    }
}