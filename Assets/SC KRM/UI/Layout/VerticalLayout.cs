using UnityEngine;

namespace SCKRM.UI.Layout
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("SC KRM/UI/Layout/Vertical Layout")]
    public sealed class VerticalLayout : LayoutChildSetting<VerticalLayoutSetting>
    {
        public float[] childYPoses { get; private set; } = new float[0];
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

            if (childYPoses.Length != childRectTransforms.Count)
                childYPoses = new float[childRectTransforms.Count];

            bool center = false;
            bool down = false;
            float y = 0;
            for (int i = 0; i < childRectTransforms.Count; i++)
            {
                RectTransform childRectTransform = childRectTransforms[i];
                if (childRectTransform == null)
                {
                    childYPoses[i] = y;
                    continue;
                }
                else if (disabledObjectIgnore && !childRectTransform.gameObject.activeInHierarchy)
                {
                    childYPoses[i] = y;
                    continue;
                }

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
                        y -= (childRectTransform.rect.height - (padding.top - padding.bottom) + spacing) * 0.5f;
                        for (int j = i; j < childRectTransforms.Count; j++)
                        {
                            RectTransform rectTransform2 = childRectTransforms[j];
                            Vector2 size = rectTransform2.rect.size;
                            if (rectTransform2 == null)
                                continue;
                            else if (!rectTransform2.gameObject.activeInHierarchy)
                                continue;

                            VerticalLayoutSetting taskBarLayoutSetting2 = childSettingComponents[j];
                            if (taskBarLayoutSetting2 != null && taskBarLayoutSetting2.mode == VerticalLayoutSetting.Mode.down)
                                break;

                            y += (size.y + spacing) * 0.5f;
                        }
                    }
                }

                Vector2 pos;
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

                    if (!onlyPos)
                        pos = new Vector2((padding.left - padding.right) * 0.5f, y + padding.bottom);
                    else
                        pos = new Vector2(childRectTransform.anchoredPosition.x, y + padding.bottom);

                    y += childRectTransform.rect.height + spacing;
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

                    float height = childRectTransform.rect.height;
                    float offset = height;
                    for (int j = i - 1; j >= 0; j--)
                    {
                        RectTransform backChildRectTransform = childRectTransforms[j];
                        if (backChildRectTransform == null)
                            continue;
                        else if (!backChildRectTransform.gameObject.activeInHierarchy)
                            continue;

                        offset = backChildRectTransform.rect.height;
                        break;
                    }

                    y -= (height - offset) * 0.5f;

                    if (!onlyPos)
                        pos = new Vector2((padding.left - padding.right) * 0.5f, y);
                    else
                        pos = new Vector2(childRectTransform.anchoredPosition.x, y);

                    y -= childRectTransform.rect.height + spacing;
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

                    if (!onlyPos)
                        pos = new Vector2((padding.left - padding.right) * 0.5f, y - padding.top);
                    else
                        pos = new Vector2(childRectTransform.anchoredPosition.x, y - padding.top);

                    y -= childRectTransform.rect.height + spacing;
                    lastYPos = y;
                }

                childYPoses[i] = y;

                if (!Kernel.isPlaying || !lerp || !useAni)
                    childRectTransform.anchoredPosition = pos;
                else
                    childRectTransform.anchoredPosition = childRectTransform.anchoredPosition.Lerp(pos, lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);

                if (!onlyPos)
                {
                    if (!Kernel.isPlaying || !lerp || !allLerp || !useAni)
                    {
                        childRectTransform.offsetMin = new Vector2(padding.left, childRectTransform.offsetMin.y);
                        childRectTransform.offsetMax = new Vector2(-padding.right, childRectTransform.offsetMax.y);
                    }
                    else
                    {
                        childRectTransform.offsetMin = childRectTransform.offsetMin.Lerp(new Vector2(padding.left, childRectTransform.offsetMin.y), lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);
                        childRectTransform.offsetMax = childRectTransform.offsetMax.Lerp(new Vector2(-padding.right, childRectTransform.offsetMax.y), lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);
                    }
                }
            }
        }
    }
}