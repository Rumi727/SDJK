using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.UI
{
    public class FixRaycastImage : Image, ICanvasRaycastFilter
    {
        public override bool IsRaycastLocationValid(Vector2 screenPoint, UnityEngine.Camera eventCamera)
        {
            if (alphaHitTestMinimumThreshold <= 0)
                return true;

            if (alphaHitTestMinimumThreshold > 1)
                return false;

            if (sprite == null)
                return true;

            Vector2 local;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local))
                return false;

            Rect rect = GetPixelAdjustedRect();

            if (preserveAspect)
                PreserveSpriteAspectRatio(ref rect, new Vector2(sprite.texture.width, sprite.texture.height));

            // Convert to have lower left corner as reference point.
            local.x += rectTransform.pivot.x * rect.width;
            local.y += rectTransform.pivot.y * rect.height;

            local = MapCoordinate(local, rect);

            // Convert local coordinates to texture space.
            Rect spriteRect = sprite.textureRect;
            float x = (spriteRect.x + local.x) / sprite.texture.width;
            float y = (spriteRect.y + local.y) / sprite.texture.height;

            try
            {
                return sprite.texture.GetPixelBilinear(x, y).a >= alphaHitTestMinimumThreshold;
            }
            catch (UnityException e)
            {
                UnityEngine.Debug.LogError("Using alphaHitTestMinimumThreshold greater than 0 on Image whose sprite texture cannot be read. " + e.Message + " Also make sure to disable sprite packing for this sprite.", this);
                return true;
            }
        }

        void PreserveSpriteAspectRatio(ref Rect rect, Vector2 spriteSize)
        {
            var spriteRatio = spriteSize.x / spriteSize.y;
            var rectRatio = rect.width / rect.height;

            if (spriteRatio > rectRatio)
            {
                var oldHeight = rect.height;
                rect.height = rect.width * (1.0f / spriteRatio);
                rect.y += (oldHeight - rect.height) * rectTransform.pivot.y;
            }
            else
            {
                var oldWidth = rect.width;
                rect.width = rect.height * spriteRatio;
                rect.x += (oldWidth - rect.width) * rectTransform.pivot.x;
            }
        }

        Vector2 MapCoordinate(Vector2 local, Rect rect)
        {
            Rect spriteRect = sprite.rect;
            if (type == Type.Simple || type == Type.Filled)
                return new Vector2(local.x * spriteRect.width / rect.width, local.y * spriteRect.height / rect.height);

            Vector4 border = sprite.border;
            Vector4 adjustedBorder = GetAdjustedBorders(border / pixelsPerUnit, rect);

            for (int i = 0; i < 2; i++)
            {
                if (local[i] <= adjustedBorder[i])
                    continue;

                if (rect.size[i] - local[i] <= adjustedBorder[i + 2])
                {
                    local[i] -= (rect.size[i] - spriteRect.size[i]);
                    continue;
                }

                if (type == Type.Sliced)
                {
                    float lerp = Mathf.InverseLerp(adjustedBorder[i], rect.size[i] - adjustedBorder[i + 2], local[i]);
                    local[i] = Mathf.Lerp(border[i], spriteRect.size[i] - border[i + 2], lerp);
                }
                else
                {
                    local[i] -= adjustedBorder[i];
                    local[i] = Mathf.Repeat(local[i], spriteRect.size[i] - border[i] - border[i + 2]);
                    local[i] += border[i];
                }
            }

            return local;
        }

        Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
        {
            Rect originalRect = rectTransform.rect;

            for (int axis = 0; axis <= 1; axis++)
            {
                float borderScaleRatio;

                // The adjusted rect (adjusted for pixel correctness)
                // may be slightly larger than the original rect.
                // Adjust the border to match the adjustedRect to avoid
                // small gaps between borders (case 833201).
                if (originalRect.size[axis] != 0)
                {
                    borderScaleRatio = adjustedRect.size[axis] / originalRect.size[axis];
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }

                // If the rect is smaller than the combined borders, then there's not room for the borders at their normal size.
                // In order to avoid artefacts with overlapping borders, we scale the borders down to fit.
                float combinedBorders = border[axis] + border[axis + 2];
                if (adjustedRect.size[axis] < combinedBorders && combinedBorders != 0)
                {
                    borderScaleRatio = adjustedRect.size[axis] / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }
            return border;
        }
    }
}
