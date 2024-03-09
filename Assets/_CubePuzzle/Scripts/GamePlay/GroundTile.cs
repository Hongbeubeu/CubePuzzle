using DG.Tweening;
using UnityEngine;

namespace CubePuzzle.Gameplay
{
    public class GroundTile : MonoBehaviour
    {
        private Transform _thisTrans;
        private Vector3 _rootPosition;

        [SerializeField] private Material tileMaterial;
        [SerializeField] private Material selectedMaterial;
        [SerializeField] private MeshRenderer mesh;


        private void Start()
        {
            if (_thisTrans == null)
            {
                _thisTrans = transform;
            }

            _rootPosition = _thisTrans.localPosition;
        }

        public void OnSelected()
        {
            _thisTrans.position += _thisTrans.up / 2f;
            mesh.material = selectedMaterial;
        }

        public void OnDeSelected()
        {
            _thisTrans.localPosition = _rootPosition;
            mesh.material = tileMaterial;
        }

        public void DoAnimation()
        {
            var target = _thisTrans.localPosition + _thisTrans.up * 1f;
            _thisTrans.DOLocalMove(target, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
    }
}