using System;
using DG.Tweening;
using UnityEngine;

namespace CubePuzzle.Gameplay
{
    public class Tile : MonoBehaviour
    {
        private Transform thisTrans;

        private void Start()
        {
            if (thisTrans == null)
            {
                thisTrans = transform;
            }
        }

        public void DoAnimation()
        {
            var target = thisTrans.localPosition + thisTrans.up;
            thisTrans.DOLocalMove(target, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
    }
}