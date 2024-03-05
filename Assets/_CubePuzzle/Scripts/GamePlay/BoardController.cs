using System;
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

        [DictionaryDrawerSettings] [ShowInInspector]
        private Dictionary<Vector2Int, Tile> _tiles = new();

        private void Start()
        {
            SpawnTiles();
        }

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

                    var x = Mathf.FloorToInt(currentPos.x / _cellUnitLength);
                    var y = Mathf.FloorToInt(currentPos.z / _cellUnitLength);
                    _tiles.Add(new Vector2Int(x, y), tile);
                    currentPos.x += _cellUnitLength;
                }

                currentPos.z += _cellUnitLength;
            }
        }

        private Tile _currentSelectedTile;

        public void OnSelectTile(Vector3 position)
        {
            if (_currentSelectedTile)
                _currentSelectedTile.OnDeSelected();

            position = transform.InverseTransformPoint(position);

            var currentGridPos = WorldPosition2GridPosition(position);

            if (!_tiles.TryGetValue(currentGridPos, out var tile)) return;

            _currentSelectedTile = tile;
            _currentSelectedTile.OnSelected();
        }

        public Vector2Int WorldPosition2GridPosition(Vector3 worldPosition)
        {
            var x = Mathf.FloorToInt(worldPosition.x / _cellUnitLength);
            var y = Mathf.FloorToInt(worldPosition.z / _cellUnitLength);
            return new Vector2Int(x, y);
        }

        public Vector3 GridPosition2WorldPosition(Vector2Int gridPosition)
        {
            var x = gridPosition.x + 0.5f;
            var z = gridPosition.y + 0.5f;
            return new Vector3(x, 0, z);
        }
    }
}