using UnityEngine;

namespace SCKRM.UI.Layout
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("SC KRM/UI/Layout/Vertical Layout")]
    public sealed class VerticalLayout : LayoutChildSetting<VerticalLayoutSetting>
    {
        public float lastYPos { get; private set; } = 0;
        public float centerLastYPos { get; private set; } = 0;
        public float downLastYPos { get; private set; } = 0;



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
            bool down = false;
            float y = 0;
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
                        tracker.Add(this, childRectTransform, DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.SizeDeltaX | DrivenTransformProperties.Anchors | DrivenTransformProperties.Pivot);
                    else
                        tracker.Add(this, childRectTransform, DrivenTransformProperties.AnchoredPositionY | DrivenTransformProperties.AnchorMinY | DrivenTransformProperties.AnchorMaxY | DrivenTransformProperties.PivotY);
                }

                VerticalLayoutSetting taskBarLayoutSetting = childSettingComponents[i];
                if (taskBarLayoutSetting != null)
                {
                    if (!down && taskBarLayoutSetting.mode == VerticalLayoutSetting.Mode.down)
                    {
                        down = true;
                        y = 0;
                    }
                    else if (!center && taskBarLayoutSetting.mode == VerticalLayoutSetting.Mode.center)
                    {
                        center = true;

                        y = 0;
                        y += (childRectTransform.sizeDelta.y - (padding.top - padding.bottom) - spacing) * 0.5f;
                        for (int j = i; j < childRectTransforms.Count; j++)
                        {
                            RectTransform rectTransform2 = childRectTransforms[j];
                            Vector2 size = rectTransform2.rect.size;
                            if (rectTransform2 == null)
                                continue;
                            else if (!rectTransform2.gameObject.activeSelf)
                                continue;
                            else if (size.y == 0)
                                continue;

                            VerticalLayoutSetting taskBarLayoutSetting2 = childSettingComponents[j];
                            if (taskBarLayoutSetting2 != null && taskBarLayoutSetting2.mode == VerticalLayoutSetting.Mode.down)
                                break;

                            y -= size.y * 0.5f;
                        }
                    }
                }

                if (down)
                {
                    if (!onlyPos)
                    {
                        childRectTransform.anchorMin = new Vector2(0, 0);
                        childRectTransform.anchorMax = new Vector2(1, 0);
                        childRectTransform.pivot = new Vector2(0.5f, 0);
                    }
                    else
                    {
                        childRectTransform.anchorMin = new Vector2(childRectTransform.anchorMin.x, 0);
                        childRectTransform.anchorMax = new Vector2(childRectTransform.anchorMax.x, 0);
                        childRectTransform.pivot = new Vector2(childRectTransform.pivot.x, 0);
                    }

                    if (!Kernel.isPlaying || !lerp || !useAni)
                    {
                        if (!onlyPos)
                            childRectTransform.anchoredPosition = new Vector2((padding.left - padding.right) * 0.5f, y + padding.bottom);
                        else
                            childRectTransform.anchoredPosition = new Vector2(childRectTransform.anchoredPosition.x, y + padding.bottom);
                    }
                    else
                    {
                        if (!onlyPos)
                            childRectTransform.anchoredPosition = childRectTransform.anchoredPosition.Lerp(new Vector2((padding.left - padding.right) * 0.5f, y + padding.bottom), lerpValue * Kernel.fpsUnscaledDeltaTime);
                        else
                            childRectTransform.anchoredPosition = childRectTransform.anchoredPosition.Lerp(new Vector2(childRectTransform.anchoredPosition.x, y + padding.bottom), lerpValue * Kernel.fpsUnscaledDeltaTime);
                    }

                    y += childRectTransform.rect.size.y + spacing;
                    downLastYPos = y;
                }
                else if (center)
                {
                    if (!onlyPos)
                    {
                        childRectTransform.anchorMin = new Vector2(0, 0.5f);
                        childRectTransform.anchorMax = new Vector2(1, 0.5f);
                        childRectTransform.pivot = new Vector2(0.5f, 0.5f);
                    }
                    else
                    {
                        childRectTransform.anchorMin = new Vector2(childRectTransform.anchorMin.x, 0.5f);
                        childRectTransform.anchorMax = new Vector2(childRectTransform.anchorMax.x, 0.5f);
                        childRectTransform.pivot = new Vector2(childRectTransform.pivot.x, 0.5f);
                    }

                    if (!Kernel.isPlaying || !lerp || !useAni)
                    {
                        if (!onlyPos)
                            childRectTransform.anchoredPosition = new Vector2((padding.left - padding.right) * 0.5f, y);
                        else
                            childRectTransform.anchoredPosition = new Vector2(childRectTransform.anchoredPosition.x, y);
                    }
                    else
                    {
                        if (!onlyPos)
                            childRectTransform.anchoredPosition = childRectTransform.anchoredPosition.Lerp(new Vector2((padding.left - padding.right) * 0.5f, y), lerpValue * Kernel.fpsUnscaledDeltaTime);
                        else
                            childRectTransform.anchoredPosition = childRectTransform.anchoredPosition.Lerp(new Vector2(childRectTransform.anchoredPosition.x, y), lerpValue * Kernel.fpsUnscaledDeltaTime);
                    }

                    y += childRectTransform.rect.size.y + spacing;
                    centerLastYPos = y;
                }
                else
                {
                    if (!onlyPos)
                    {
                        childRectTransform.anchorMin = new Vector2(0, 1);
                        childRectTransform.anchorMax = new Vector2(1, 1);
                        childRectTransform.pivot = new Vector2(0.5f, 1);
                    }
                    else
                    {
                        childRectTransform.anchorMin = new Vector2(childRectTransform.anchorMin.x, 1);
                        childRectTransform.anchorMax = new Vector2(childRectTransform.anchorMax.x, 1);
                        childRectTransform.pivot = new Vector2(childRectTransform.pivot.x, 1);
                    }

                    if (!Kernel.isPlaying || !lerp || !useAni)
                    {
                        if (!onlyPos)
                            childRectTransform.anchoredPosition = new Vector2((padding.left - padding.right) * 0.5f, y - padding.top);
                        else
                            childRectTransform.anchoredPosition = new Vector2(childRectTransform.anchoredPosition.x, y - padding.top);
                    }
                    else
                    {
                        if (!onlyPos)
                            childRectTransform.anchoredPosition = childRectTransform.anchoredPosition.Lerp(new Vector2((padding.left - padding.right) * 0.5f, y - padding.top), lerpValue * Kernel.fpsUnscaledDeltaTime);
                        else
                            childRectTransform.anchoredPosition = childRectTransform.anchoredPosition.Lerp(new Vector2(childRectTransform.anchoredPosition.x, y - padding.top), lerpValue * Kernel.fpsUnscaledDeltaTime);
                    }

                    y -= childRectTransform.rect.size.y + spacing;
                    lastYPos = y;
                }

                if (!onlyPos)
                {
                    if (!Kernel.isPlaying || !lerp || !allLerp || !useAni)
                    {
                        childRectTransform.offsetMin = new Vector2(padding.left, childRectTransform.offsetMin.y);
                        childRectTransform.offsetMax = new Vector2(-padding.right, childRectTransform.offsetMax.y);
                    }
                    else
                    {
                        childRectTransform.offsetMin = childRectTransform.offsetMin.Lerp(new Vector2(padding.left, childRectTransform.offsetMin.y), lerpValue * Kernel.fpsUnscaledDeltaTime);
                        childRectTransform.offsetMax = childRectTransform.offsetMax.Lerp(new Vector2(-padding.right, childRectTransform.offsetMax.y), lerpValue * Kernel.fpsUnscaledDeltaTime);
                    }
                }
            }
        }
    }
}