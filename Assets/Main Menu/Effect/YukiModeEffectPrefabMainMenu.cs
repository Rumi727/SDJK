using SCKRM;
using SCKRM.Rhythm;
using SCKRM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.MainMenu.Effect
{
    public class YukiModeEffectPrefabMainMenu : YukiModeEffectPrefabParent, IUI
    {
        #region UI
        Canvas _canvas; public Canvas canvas => _canvas = this.GetComponentInParentFieldSave(_canvas, true);
        CanvasSetting _canvasSetting; public CanvasSetting canvasSetting => _canvasSetting = this.GetComponentInParentFieldSave(_canvasSetting, true);

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
                        _rectTransform = gameObject.AddComponent<RectTransform>();
                }

                return _rectTransform;
            }
        }
        [SerializeField] RectTransformTool _rectTransformTool; public RectTransformTool rectTransformTool => _rectTransformTool = this.GetComponentFieldSave(_rectTransformTool);

        [SerializeField] Graphic _graphic; public Graphic graphic => _graphic = this.GetComponentFieldSave(_graphic, ComponentUtility.GetComponentMode.none);
        #endregion

        protected override void Update()
        {
            base.Update();

            if (yukiModeEffect == null)
                return;

            if (yukiMode)
            {
                double div = offsetCurrentBeatReapeat / yukiModeEffect.count;

                if (isLeft)
                    rectTransform.anchoredPosition = new Vector2(-(float)(yukiModeEffect.width * (float)div), 0);
                else
                    rectTransform.anchoredPosition = new Vector2((float)(yukiModeEffect.width * (float)div), 0);

                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0);
                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, (float)(1 - div));
            }
            else
                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 0);
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            rectTransform.anchoredPosition = Vector2.zero;
            graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, 1);

            return true;
        }
    }
}
