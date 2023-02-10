using SCKRM.Renderer;
using SCKRM.UI.Layout;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SCKRM.UI.SideBar
{
    [AddComponentMenu("SC KRM/UI/Kerenl/Side Bar/Notice")]
    [RequireComponent(typeof(VerticalLayout), typeof(ChildSizeFitter))]
    public sealed class Notice : UIObjectPoolingBase, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, HideInInspector] VerticalLayout _verticalLayout; public VerticalLayout verticalLayout => _verticalLayout = this.GetComponentFieldSave(_verticalLayout);
        [SerializeField, HideInInspector] ChildSizeFitter _childSizeFitter; public ChildSizeFitter childSizeFitter => _childSizeFitter = this.GetComponentFieldSave(_childSizeFitter);



        [SerializeField] CanvasGroup _removeButtonCanvasGroup;
        public CanvasGroup removeButtonCanvasGroup => _removeButtonCanvasGroup;



        [SerializeField] CustomSpriteRendererBase _icon;
        public CustomSpriteRendererBase icon => _icon;

        [SerializeField] CustomTextRendererBase _nameText;
        public CustomTextRendererBase nameText => _nameText;

        [SerializeField] CustomTextRendererBase _infoText;
        public CustomTextRendererBase infoText => _infoText;

        bool pointer = false;
        void Update()
        {
            if (pointer || removeButtonCanvasGroup.gameObject == EventSystem.current.currentSelectedGameObject)
                removeButtonCanvasGroup.alpha = removeButtonCanvasGroup.alpha.MoveTowards(1, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
            else
                removeButtonCanvasGroup.alpha = removeButtonCanvasGroup.alpha.MoveTowards(0, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
        }

        public override bool Remove()
        {
            if (base.Remove())
                return false;

            if (icon.gameObject.activeSelf)
                icon.gameObject.SetActive(false);

            nameText.path = "";
            infoText.path = "";
            childSizeFitter.min = 40;
            verticalLayout.padding.left = 10;
            removeButtonCanvasGroup.alpha = 0;

            return true;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => pointer = true;

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => pointer = false;
    }
}