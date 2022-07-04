using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SCKRM.UI
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class RectTransformTool : UIBehaviour, IUI
    {
        [SerializeField] RectTransform _parentRectTransform; public RectTransform parentRectTransform
        {
            get
            {
                if (_parentRectTransform == null || _parentRectTransform.gameObject != transform.parent.gameObject)
                    _parentRectTransform = transform.parent as RectTransform;

                return _parentRectTransform;
            }
        }
        [SerializeField] RectTransform _rectTransform; public RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null || _rectTransform.gameObject != gameObject)
                {
                    _rectTransform = transform as RectTransform;
                    if (_rectTransform == null)
                        DestroyImmediate(this);
                }

                return _rectTransform;
            }
        }

        public RectTransformTool rectTransformTool => this;

        [SerializeField] Graphic _graphic; public Graphic graphic => _graphic = this.GetComponentFieldSave(_graphic, ComponentTool.GetComponentMode.none);



        Vector3[] worldCornersArray = new Vector3[4];
        public RectCorner localCorners => rectTransform.rect;
        public RectCorner worldCorners
        {
            get
            {
                rectTransform.GetWorldCorners(worldCornersArray);
                return new RectCorner(worldCornersArray[0], worldCornersArray[1], worldCornersArray[2], worldCornersArray[3]);
            }
        }

        public delegate void RectTransformEvent();

        /// <summary>
        /// 상위 트랜스폼이 변경되기 전에 호출됩니다
        /// </summary>
        public event RectTransformEvent onBeforeTransformParentChanged;
        //[SerializeField] UnityEvent onBeforeTransformParentChangedUnityEvent = new UnityEvent();
        /// <summary>
        /// 상위 트랜스폼이 변경되면 호출됩니다
        /// </summary>
        public event RectTransformEvent onTransformParentChanged;
        //[SerializeField] UnityEvent onTransformParentChangedUnityEvent = new UnityEvent();

        /// <summary>
        /// RectTransform의 월드 값이 변경되면 호출됩니다
        /// </summary>
        public event RectTransformEvent onRectTransformDimensionsChange;
        //[SerializeField] UnityEvent onRectTransformDimensionsChangeUnityEvent = new UnityEvent();
        /// <summary>
        /// 애니메이션으로 인해 속성이 변경된 경우를 위한 콜백입니다
        /// </summary>
        public event RectTransformEvent onDidApplyAnimationProperties;
        //[SerializeField] UnityEvent onDidApplyAnimationPropertiesUnityEvent = new UnityEvent();

        /// <summary>
        /// 부모 캔버스의 상태가 변경되면 호출됩니다
        /// </summary>
        public event RectTransformEvent onCanvasHierarchyChanged;
        //[SerializeField] UnityEvent onCanvasHierarchyChangedUnityEvent = new UnityEvent();
        /// <summary>
        /// 캔버스 그룹의 상태가 변경되면 호출됩니다
        /// </summary>
        public event RectTransformEvent onCanvasGroupChanged;
        //[SerializeField] UnityEvent onCanvasGroupChangedUnityEvent = new UnityEvent();

        protected override void OnBeforeTransformParentChanged()
        {
            onBeforeTransformParentChanged?.Invoke();
            /*if (onBeforeTransformParentChangedUnityEvent.GetPersistentEventCount() > 0)
                onBeforeTransformParentChangedUnityEvent.Invoke();*/
        }
        protected override void OnTransformParentChanged()
        {
            onTransformParentChanged?.Invoke();
            /*if (onTransformParentChangedUnityEvent.GetPersistentEventCount() > 0)
                onTransformParentChangedUnityEvent.Invoke();*/
        }

        protected override void OnRectTransformDimensionsChange()
        {
            onRectTransformDimensionsChange?.Invoke();
            /*if (onRectTransformDimensionsChangeUnityEvent.GetPersistentEventCount() > 0)
                onRectTransformDimensionsChangeUnityEvent.Invoke();*/
        }
        protected override void OnDidApplyAnimationProperties()
        {
            onDidApplyAnimationProperties?.Invoke();
            /*if (onDidApplyAnimationPropertiesUnityEvent.GetPersistentEventCount() > 0)
                onDidApplyAnimationPropertiesUnityEvent.Invoke();*/
        }

        protected override void OnCanvasHierarchyChanged()
        {
            onCanvasHierarchyChanged?.Invoke();
            /*if (onCanvasHierarchyChangedUnityEvent.GetPersistentEventCount() > 0)
                onCanvasHierarchyChangedUnityEvent.Invoke();*/
        }
        protected override void OnCanvasGroupChanged()
        {
            onCanvasGroupChanged?.Invoke();
            /*if (onCanvasGroupChangedUnityEvent.GetPersistentEventCount() > 0)
                onCanvasGroupChangedUnityEvent.Invoke();*/
        }
    }

    public struct RectCorner
    {
        public Rect rect
        {
            get => this;
            set
            {
                bottomLeft = new Vector2(rect.xMin, rect.yMin);
                topLeft = new Vector2(rect.xMin, rect.yMax);
                topRight = new Vector2(rect.xMax, rect.yMax);
                bottomRight = new Vector2(rect.xMax, rect.yMin);
            }
        }

        public Vector2 bottomLeft { get; set; }
        public Vector2 topLeft { get; set; }
        public Vector2 topRight { get; set; }
        public Vector2 bottomRight { get; set; }



        public static implicit operator Rect(RectCorner value) => new Rect(value.bottomLeft, value.topRight - value.bottomLeft);
        public static implicit operator RectCorner(Rect value) => new RectCorner(value);



        public RectCorner(Vector2 bottomLeft, Vector2 topLeft, Vector2 topRight, Vector2 bottomRight)
        {
            this.bottomLeft = bottomLeft;
            this.topLeft = topLeft;
            this.topRight = topRight;
            this.bottomRight = bottomRight;
        }

        public RectCorner(Rect rect)
        {
            bottomLeft = new Vector2(rect.xMin, rect.yMin);
            topLeft = new Vector2(rect.xMin, rect.yMax);
            topRight = new Vector2(rect.xMax, rect.yMax);
            bottomRight = new Vector2(rect.xMax, rect.yMin);
        }

        public RectCorner(Vector2 min, Vector2 max)
        {
            bottomLeft = new Vector2(min.x, min.y);
            topLeft = new Vector2(min.x, max.y);
            topRight = new Vector2(max.x, max.y);
            bottomRight = new Vector2(max.x, min.y);
        }
    }
}
