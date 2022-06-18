using SCKRM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK
{
    public class VisualizerBar : MonoBehaviour
    {
        public RectTransform rectTransform;
        public Image image;
        public float size = 0;

        void Update()
        {
            if (size > 0)
                size -= 5 * Kernel.fpsDeltaTime;

            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(transform.localScale.x, size), 0.75f * Kernel.fpsDeltaTime);
        }
    }
}