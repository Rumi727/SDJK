using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PolyAndCode.UI
{
    public class RecyclableScrollRectScrollBar : Scrollbar, IEndDragHandler
    {
        [SerializeField] RecyclableScrollRect recyclableScrollRect;

        bool isDrag = false;
        protected override void Update()
        {
            base.Update();

            if (!isDrag)
                value = Mathf.Lerp(value, 0.5f, Time.unscaledDeltaTime * 10);

            recyclableScrollRect.content.anchoredPosition += (new Vector2(0, Mathf.Lerp(-200, 200, value)) * recyclableScrollRect.scrollSensitivity) * (Time.unscaledDeltaTime * 10);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            isDrag = true;
        }

        public void OnEndDrag(PointerEventData eventData) => isDrag = false;
    }
}