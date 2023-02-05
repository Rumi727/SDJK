using UnityEngine;

namespace SCKRM.UI.Layout
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("SC KRM/UI/Layout/Horizontal Layout")]
    public sealed class HorizontalLayout : LayoutChildSetting<HorizontalLayoutSetting>
    {
        public float[] childXPoses { get; private set; } = new float[0];
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

            if (childXPoses.Length != childRectTransforms.Count)
                childXPoses = new float[childRectTransforms.Count];

            bool center = false;
            bool right = false;
            float x = 0;
            for (int i = 0; i < childRectTransforms.Count; i++)
            {
                RectTransform childRectTransform = childRectTransforms[i];
                if (childRectTransform == null)
                {
                    childXPoses[i] = x;
                    continue;
                }
                else if (disabledObjectIgnore && !childRectTransform.gameObject.activeInHierarchy)
                {
                    childXPoses[i] = x;
                    continue;
                }

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
                        x += (childRectTransform.rect.width + (padding.left - padding.right) + spacing) * 0.5f;
                        for (int j = i; j < childRectTransforms.Count; j++)
                        {
                            RectTransform rectTransform2 = childRectTransforms[j];
                            Vector2 size = rectTransform2.rect.size;
                            if (rectTransform2 == null)
                                continue;
                            else if (!rectTransform2.gameObject.activeInHierarchy)
                                continue;

                            HorizontalLayoutSetting taskBarLayoutSetting2 = childSettingComponents[j];
                            if (taskBarLayoutSetting2 != null && taskBarLayoutSetting2.mode == HorizontalLayoutSetting.Mode.right)
                                break;

                            x -= (size.x + spacing) * 0.5f;
                        }
                    }
                }

                Vector2 pos;
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

                    if (!onlyPos)
                        pos = new Vector2(x - padding.right, -(padding.top - padding.bottom) * 0.5f);
                    else
                        pos = new Vector2(x - padding.right, childRectTransform.anchoredPosition.y);

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

                    float width = childRectTransform.rect.width;
                    float offset = width;
                    for (int j = i - 1; j >= 0; j--)
                    {
                        RectTransform backChildRectTransform = childRectTransforms[j];
                        if (backChildRectTransform == null)
                            continue;
                        else if (!backChildRectTransform.gameObject.activeInHierarchy)
                            continue;

                        offset = backChildRectTransform.rect.width;
                        break;
                    }

                    x += (width - offset) * 0.5f;

                    if (!onlyPos)
                        pos = new Vector2(x, -(padding.top - padding.bottom) * 0.5f);
                    else
                        pos = new Vector2(x, childRectTransform.anchoredPosition.y);

                    x += width + spacing;
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

                    if (!onlyPos)
                        pos = new Vector2(x + padding.left, -(padding.top - padding.bottom) * 0.5f);
                    else
                        pos = new Vector2(x + padding.left, childRectTransform.anchoredPosition.y);

                    x += childRectTransform.rect.size.x + spacing;
                    lastXPos = x;
                }

                childXPoses[i] = x;

                if (!Kernel.isPlaying || !lerp || !useAni)
                    childRectTransform.anchoredPosition = pos;
                else
                    childRectTransform.anchoredPosition = childRectTransform.anchoredPosition.Lerp(pos, lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);

                if (!onlyPos)
                {
                    if (!Kernel.isPlaying || !lerp || !allLerp || !useAni)
                    {
                        childRectTransform.offsetMin = new Vector2(childRectTransform.offsetMin.x, padding.bottom);
                        childRectTransform.offsetMax = new Vector2(childRectTransform.offsetMax.x, -padding.top);
                    }
                    else
                    {
                        childRectTransform.offsetMin = childRectTransform.offsetMin.Lerp(new Vector2(childRectTransform.offsetMin.x, padding.bottom), lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);
                        childRectTransform.offsetMax = childRectTransform.offsetMax.Lerp(new Vector2(childRectTransform.offsetMax.x, -padding.top), lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);
                    }
                }
            }
        }
    }
}