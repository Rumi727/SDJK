using SCKRM;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK
{
    public class VisualizerBar : MonoBehaviour
    {
        public RectTransform rectTransform;
        public Image image;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public float size 
        {
            get
            {
                while (Interlocked.CompareExchange(ref sizeLock, 1, 0) != 0)
                    Thread.Sleep(1);

                float size = _size.Clamp(0);

                Interlocked.Decrement(ref sizeLock);

                return size;
            }
            set
            {
                while (Interlocked.CompareExchange(ref sizeLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _size = value.Clamp(0);

                Interlocked.Decrement(ref sizeLock);
            }
        }
        float _size = 0;
        int sizeLock = 0;

        void Update()
        {
            if (size > 0)
                size -= 5 * Kernel.fpsDeltaTime;

            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(transform.localScale.x, size), 0.75f * Kernel.fpsDeltaTime);
        }
    }
}