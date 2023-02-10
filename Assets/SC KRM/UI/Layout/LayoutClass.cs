using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCKRM.UI.Layout
{
    public abstract class LayoutChildBase : UIAniLayoutBase
    {
        [SerializeField] float _spacing;
        public float spacing { get => _spacing; set => _spacing = value; }


        [SerializeField] bool _disabledObjectIgnore = true;
        public bool disabledObjectIgnore { get => _disabledObjectIgnore; set => _disabledObjectIgnore = value; }


        [SerializeField] RectTransform[] _ignore = new RectTransform[0];
        public RectTransform[] ignore { get => _ignore; set => _ignore = value; }


        public List<RectTransform> childRectTransforms { get; } = new List<RectTransform>();

        /// <summary>
        /// Please put base.LayoutRefresh() when overriding
        /// </summary>
        public override void LayoutRefresh()
        {
            if ((transform.childCount - ignore.Length) != childRectTransforms.Count || !Kernel.isPlaying)
                SetChild();

            int childCount = transform.childCount;
            for (int i = 0; i < (childCount - ignore.Length); i++)
            {
                if (transform.GetChild(i) != childRectTransforms[i])
                {
                    SetChild();
                    break;
                }
            }
        }

        protected virtual void SetChild()
        {
            childRectTransforms.Clear();

            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform childTransform = transform.GetChild(i);
                if (childTransform != ignore.Contains(childTransform))
                    childRectTransforms.Add(childTransform as RectTransform);
            }
        }
    }

    public abstract class LayoutChildSettingBase<ChildSettingComponent> : LayoutChildBase where ChildSettingComponent : Component
    {
        public List<ChildSettingComponent> childSettingComponents { get; } = new List<ChildSettingComponent>();

        protected override void SetChild()
        {
            childRectTransforms.Clear();
            childSettingComponents.Clear();

            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform childTransform = transform.GetChild(i);
                if (childTransform != ignore.Contains(childTransform))
                {
                    childRectTransforms.Add(childTransform.GetComponent<RectTransform>());
                    childSettingComponents.Add(childTransform.GetComponent<ChildSettingComponent>());
                }
            }
        }
    }
}