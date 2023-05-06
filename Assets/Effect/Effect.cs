using SCKRM;
using SCKRM.Rhythm;
using SCKRM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Effect
{
    public abstract class Effect : MonoBehaviour
    {
        [SerializeField] EffectManager _effectManager; public virtual EffectManager effectManager => _effectManager;

        public virtual Map.MapPack mapPack => effectManager.selectedMapPack;
        public virtual Map.MapFile map => effectManager.selectedMap;

        public virtual void Refresh(bool force = false) { }

        protected virtual void Update()
        {
            if (!RhythmManager.isPlaying || effectManager == null || effectManager.selectedMap == null)
                return;

            RealUpdate();
        }

        protected virtual void RealUpdate() { }
    }

    public abstract class UIEffect : Effect, IUI
    {
        Canvas _canvas; public Canvas canvas => _canvas = this.GetComponentInParentFieldSave(_canvas, true);
        CanvasSetting _canvasSetting; public CanvasSetting canvasSetting => _canvasSetting = this.GetComponentInParentFieldSave(_canvasSetting, true);

        RectTransform _parentRectTransform; public RectTransform parentRectTransform
        {
            get
            {
                if (_parentRectTransform == null || _parentRectTransform.gameObject != transform.parent.gameObject)
                    _parentRectTransform = transform.parent as RectTransform;

                return _parentRectTransform;
            }
        }
        RectTransform _rectTransform; public RectTransform rectTransform
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
        RectTransformTool _rectTransformTool; public RectTransformTool rectTransformTool => _rectTransformTool = this.GetComponentFieldSave(_rectTransformTool);

        Graphic _graphic; public Graphic graphic => _graphic = this.GetComponentFieldSave(_graphic, ComponentUtility.GetComponentMode.none);
    }
}
