using DG.Tweening;
using UnityEngine;

namespace CubePuzzle.Gameplay
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Material _tileMaterial;
        [SerializeField] private Material _selectedMaterial;
        [SerializeField] private MeshRenderer _mesh;

        private Transform thisTrans;
        private Vector3 rootPosition;

        private void Start()
        {
            if (thisTrans == null)
            {
                thisTrans = transform;
            }

            rootPosition = thisTrans.localPosition;
        }

        public void OnSelected()
        {
            thisTrans.position += thisTrans.up;
            _mesh.material = _selectedMaterial;
        }

        public void OnDeSelected()
        {
            thisTrans.localPosition = rootPosition;
            _mesh.material = _tileMaterial;
        }

        public void DoAnimation()
        {
            var target = thisTrans.localPosition + thisTrans.up * 2f;
            thisTrans.DOLocalMove(target, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
    }
}