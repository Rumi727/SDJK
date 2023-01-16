using SCKRM;
using SCKRM.Easing;
using SCKRM.Rhythm;
using SCKRM.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SDJK.MainMenu
{
    public class BarButton : SCKRM.UI.UI, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        public float sizeOffset => _sizeOffset; [SerializeField] float _sizeOffset;
        public UnityEvent onClick => _onClick; [SerializeField] UnityEvent _onClick;



        [SerializeField] RectTransform icon;



        bool pointer = false;
        float pointerSizeStart = 0;
        float pointerSizeT = 200;
        float pointerSize = 200;
        Vector3 rotation = Vector3.zero;
        double lastCurrentBeat1Beat = 0;
        bool pointerAniLock = true;
        void Update()
        {
            if (pointerSizeT < 1)
                pointerSizeT = (pointerSizeT + 0.03f * Kernel.fpsUnscaledDeltaTime).Clamp01();

            if (pointer)
                pointerSize = (float)EasingFunction.EaseOutElastic(pointerSizeStart, 330 + sizeOffset, pointerSizeT);
            else
                pointerSize = (float)EasingFunction.EaseOutElastic(pointerSizeStart, 230 + sizeOffset, pointerSizeT);

            rectTransform.sizeDelta = new Vector2(pointerSize, 0);

            if (pointerAniLock && lastCurrentBeat1Beat > RhythmManager.currentBeatScreen1Beat)
                pointerAniLock = false;

            if (pointer && !pointerAniLock)
            {
                float sin = (float)Math.Sin(RhythmManager.currentBeatScreen1Beat * Mathf.PI).Abs();
                icon.anchoredPosition = new Vector2(icon.anchoredPosition.x, sin * 10);

                int currentBeat;
                if (RhythmManager.currentBeatScreen < 0)
                    currentBeat = (int)RhythmManager.currentBeatScreen + 1;
                else
                    currentBeat = (int)RhythmManager.currentBeatScreen;

                if (currentBeat % 2 == 0)
                    rotation = rotation.MoveTowards(new Vector3(0, 0, 10), RhythmManager.bpmFpsDeltaTime);
                else
                    rotation = rotation.MoveTowards(new Vector3(0, 0, -10), RhythmManager.bpmFpsDeltaTime);

                icon.localEulerAngles = rotation;
            }
            else
            {
                icon.anchoredPosition = icon.anchoredPosition.Lerp(new Vector2(icon.anchoredPosition.x, 0), 0.2f * Kernel.fpsUnscaledDeltaTime);

                rotation = rotation.Lerp(Vector3.zero, 0.2f * Kernel.fpsUnscaledDeltaTime);
                icon.localEulerAngles = rotation;

                pointerAniLock = true;
            }

            lastCurrentBeat1Beat = RhythmManager.currentBeatScreen1Beat;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            pointer = true;

            pointerSizeStart = rectTransform.sizeDelta.x;
            pointerSizeT = 0;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            pointer = false;

            pointerSizeStart = rectTransform.sizeDelta.x;
            pointerSizeT = 0;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (pointer)
                onClick.Invoke();
        }
    }
}
