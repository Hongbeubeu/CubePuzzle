﻿using System;
using UnityEngine;

namespace com.curve
{
    public enum SplineWalkerMode
    {
        Once,
        Loop,
        PingPong
    }

    public class SplineWalker : MonoBehaviour
    {
        public SplineWalkerMode mode;
        public BezierSpline spline;
        public float duration;
        public bool lookForward;
        private float progress;
        private bool goingForward = true;

        private void Update()
        {
            if (goingForward)
            {
                progress += Time.deltaTime / duration;
                if (progress > 1f)
                {
                    if (mode == SplineWalkerMode.Once) progress = 1f;
                    else if (mode == SplineWalkerMode.Loop) progress -= 1f;
                    else
                    {
                        progress = 2f - progress;
                        goingForward = false;
                    }
                }
            }
            else
            {
                progress -= Time.deltaTime / duration;
                if (progress < 0f)
                {
                    progress = -progress;
                    goingForward = true;
                }
            }

            var position = spline.GetPoint(progress);
            transform.localPosition = position;
            if (lookForward)
            {
                transform.LookAt(position + spline.GetDirection(progress));
            }
        }
    }
}