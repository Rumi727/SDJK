using UnityEngine;

namespace SCKRM.UI.Layout
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("SC KRM/UI/Layout/Horizontal Layout")]
    public sealed class HorizontalLayout : LayoutChildSetting<HorizontalLayoutSetting>
    {
        public float lastXPos { get; private set; } = 0;
        public float centerLastXPos { get; private set; } = 0;
        public float rightLastXPos { get; private set; } = 0;



        [SerializeField] bool _allLerp = false;
        public bool allLerp { get => _allLerp; set => _allLerp = value; }

        [SerializeField] bool _onlyPos = false;
        public bool onlyPos { get => _onlyPos; set => _onlyPos = value; }



        [SerializeField] RectOffset _padding = new RectOffset();
        public RectOffset padding { get => _padding; set => _padding = value; }



        DrivenRectTransformTracker tracker;

        protected override void OnDisable()
        {
            if (!Kernel.isPlaying)
                tracker.Clear();
        }

        public override void SizeUpdate(bool useAni = true)
        {
            if (childRectTransforms == null)
                return;

            if (!Kernel.isPlaying)
                tracker.Clear();

            bool center = false;
            bool right = false;
            float x = 0;
            for (int i = 0; i < childRectTransforms.Count; i++)
            {
                RectTransform childRectTransform = childRectTransforms[i];
                if (childRectTransform == null)
                    continue;
                else if (!childRectTransform.gameObject.activeSelf)
                    continue;

                if (!Kernel.isPlaying)
                {
                    if (!onlyPos)
                        tracker.Add(this, childRectTransform, DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.SizeDeltaY | DrivenTransformProperties.Anchors | DrivenTransformProperties.Pivot);
                    else
                        tracker.Add(this, childRectTransform, DrivenTransformProperties.AnchoredPositionX | DrivenTransformProperties.AnchorMinX | DrivenTransformProperties.AnchorMaxX | DrivenTransformProperties.PivotX);
                }

                HorizontalLayoutSetting taskBarLayoutSetting = childSettingComponents[i];
                if (taskBarLayoutSetting != null)
                {
                    if (!right && taskBarLayoutSetting.mode == HorizontalLayoutSetting.Mode.right)
                    {
                        right = true;
                        x = 0;
                    }
                    else if (!center && taskBarLayoutSetting.mode == HorizontalLayoutSetting.Mode.center)
                    {
                        center = true;

                        x = 0;
                        x += (childRectTransform.sizeDelta.x + (padding.left - padding.right) - spacing) * 0.5f;
                        for (int j = i; j < childRectTransforms.Count; j++)
                        {
                            RectTransform rectTransform2 = childRectTransforms[j];
                            Vector2 size = rectTransform2.rect.size;
                            if (rectTransform2 == null)
                                continue;
                            else if (!rectTransform2.gameObject.activeSelf)
                                continue;
                            else if (size.x == 0)
                                continue;

                            HorizontalLayoutSetting taskBarLayoutSetting2 = childSettingComponents[j];
                            if (taskBarLayoutSetting2 != null && taskBarLayoutSetting2.mode == HorizontalLayoutSetting.Mode.right)
                                break;

                            x -= size.x * 0.5f;
                        }
                    }
                }

                if (right)
                {
                    if (!onlyPos)
                    {
                        childRectTransform.anchorMin = new Vector2(1, 0);
                        childRectTransform.anchorMax = new Vector2(1, 1);
                        childRectTransform.pivot = new Vector2(1, 0.5f);
                    }
                    else
                    {
                        childRectTransform.anchorMin = new Vector2(1, childRectTransform.anchorMin.y);
                        childRectTransform.anchorMax = new Vector2(1, childRectTransform.anchorMin.y);
                        childRectTransform.pivot = new Vector2(1, childRectTransform.pivot.y);
                    }

                    if (!Kernel.isPlaying || !lerp || !useAni)
                    {
                        if (!onlyPos)
                            childRectTransform.anchoredPosition = new Vector2(x - padding.right, -(padding.top - padding.bottom) * 0.5f);
                        else
                            childRectTransform.anchoredPosition = new Vector2(x - padding.right, childRectTransform.anchoredPosition.y);
                    }
                    else
                    {
                        if (!onlyPos)
                            childRectTransform.anchoredPosition = childRectTransform.anchoredPosition.Lerp(new Vector2(x - padding.right, -(padding.top - padding.bottom) * 0.5f), lerpValue * Kernel.fpsUnscaledDeltaTime);
                        else
                            childRectTransform.anchoredPosition = childRectTransform.anchoredPosition.Lerp(new Vector2(x - padding.right, childRectTransform.anchoredPosition.y), lerpValue * Kernel.fpsUnscaledDeltaTime);
                    }

                    x -= childRectTransform.rect.size.x + spacing;
                    rightLastXPos = x;
                }
                else if (center)
                {
                    if (!onlyPos)
                    {
                        childRectTransform.anchorMin = new Vector2(0.5f, 0);
                        childRectTransform.anchorMax = new Vector2(0.5f, 1);
                        childRectTransform.pivot = new Vector2(0.5f, 0.5f);
                    }
                    else
                    {
                        childRectTransform.anchorMin = new Vector2(0.5f, childRectTransform.anchorMin.y);
                        childRectTransform.anchorMax = new Vector2(0.5f, childRectTransform.anchorMin.y);
                        childRectTransform.pivot = new Vector2(0.5f, childRectTransform.pivot.y);
                    }

                    if (!Kernel.isPlaying || !lerp || !useAni)
                    {
                        if (!onlyPos)
                            childRectTransform.anchoredPosition = new Vector2(x, -(padding.top - padding.bottom) * 0.5f);
                        else
                            childRectTransform.anchoredPosition = new Vector2(x, childRectTransform.anchoredPosition.y);
                    }
                    else
                    {
                        if (!onlyPos)
                            childRectTransform.anchoredPosition = childRectTransform.anchoredPosition.Lerp(new Vector2(x, -(padding.top - padding.bottom) * 0.5f), lerpValue * Kernel.fpsUnscaledDeltaTime);
                        else
                            childRectTransform.anchoredPosition = childRectTransform.anchoredPosition.Lerp(new Vector2(x, childRectTransform.anchoredPosition.y), lerpValue * Kernel.fpsUnscaledDeltaTime);
                    }

                    x += childRectTransform.rect.size.x + spacing;
                    centerLastXPos = x;
                }
                else
                {
                    if (!onlyPos)
                    {
                        childRectTransform.anchorMin = new Vector2(0, 0);
                        childRectTransform.anchorMax = new Vector2(0, 1);
                        childRectTransform.pivot = new Vector2(0, 0.5f);
                    }
                    else
                    {
                        childRectTransform.anchorMin = new Vector2(0, childRectTransform.anchorMin.y);
                        childRectTransform.anchorMax = new Vector2(0, childRectTransform.anchorMin.y);
                        childRectTransform.pivot = new Vector2(0, childRectTransform.pivot.y);
                    }

                    if (!Kernel.isPlaying || !lerp || !useAni)
                    {
                        if (!onlyPos)
                            childRectTransform.anchoredPosition = new Vector2(x + padding.left, -(padding.top - padding.bottom) * 0.5f);
                        else
                            childRectTransform.anchoredPosition = new Vector2(x + padding.left, childRectTransform.anchoredPosition.y);
                    }
                    else
                    {
                        if (!onlyPos)
                            childRectTransform.anchoredPosition = childRectTransform.anchoredPosition.Lerp(new Vector2(x + padding.left, -(padding.top - padding.bottom) * 0.5f), lerpValue * Kernel.fpsUnscaledDeltaTime);
                        else
                            childRectTransform.anchoredPosition = childRectTransform.anchoredPosition.Lerp(new Vector2(x + padding.left, childRectTransform.anchoredPosition.y), lerpValue * Kernel.fpsUnscaledDeltaTime);
                    }

                    x += childRectTransform.rect.size.x + spacing;
                    lastXPos = x;
                }

                if (!onlyPos)
                {
                    if (!Kernel.isPlaying || !lerp || !allLerp || !useAni)
                    {
                        childRectTransform.offsetMin = new Vector2(childRectTransform.offsetMin.x, padding.bottom);
                        childRectTransform.offsetMax = new Vector2(childRectTransform.offsetMax.x, -padding.top);
                    }
                    else
                    {
                        childRectTransform.offsetMin = childRectTransform.offsetMin.Lerp(new Vector2(childRectTransform.offsetMin.x, padding.bottom), lerpValue * Kernel.fpsUnscaledDeltaTime);
                        childRectTransform.offsetMax = childRectTransform.offsetMax.Lerp(new Vector2(childRectTransform.offsetMax.x, -padding.top), lerpValue * Kernel.fpsUnscaledDeltaTime);
                    }
                }
            }
        }
    }
}