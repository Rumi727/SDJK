using SCKRM;
using SCKRM.Object;
using SCKRM.Rhythm;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset.SDJK.UI
{
    public sealed class AccuracyBarUI : SDJKUI
    {
        [SerializeField] int sampleCount = 10;
        [SerializeField] string rodObject = "ruleset.sdjk.accuract_bar.rod";
        [SerializeField] float lerpT = 0.125f;
        [SerializeField, NotNull] SDJKManager manager;
        [SerializeField, NotNull] RectTransform arrow;
        [SerializeField, NotNull] RectTransform bar;
        [SerializeField, NotNull] RawImage barImage;

        protected override void OnEnable()
        {
            base.OnEnable();
            TextureRefresh();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (loadedTexture != null)
                DestroyImmediate(loadedTexture);
        }

        float lastWidth = -1;
        double anchorPos = 0;
        double accuracy = 0;
        List<double> accuracys = new List<double>();
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            if (rectTransform.rect.width != lastWidth)
            {
                TextureRefresh();
                lastWidth = rectTransform.rect.width;
            }

            anchorPos = anchorPos.Lerp(GetAnchorPos(accuracy), lerpT * RhythmManager.bpmFpsDeltaTime);

            arrow.anchorMin = new Vector2((float)anchorPos, arrow.anchorMin.y);
            arrow.anchorMax = new Vector2((float)anchorPos, arrow.anchorMax.y);
        }

        protected override void JudgementAction(double disSecond, bool isMiss, double accuracy, JudgementMetaData metaData)
        {
            float anchorPos = (float)GetAnchorPos(accuracy);
            AccuracyBarRod accuracyBarRod = (AccuracyBarRod)ObjectPoolingSystem.ObjectCreate(rodObject, bar).monoBehaviour;

            accuracyBarRod.rectTransform.anchorMin = new Vector2(anchorPos, accuracyBarRod.rectTransform.anchorMin.y);
            accuracyBarRod.rectTransform.anchorMax = new Vector2(anchorPos, accuracyBarRod.rectTransform.anchorMax.y);

            accuracys.Add(accuracy);
            while (accuracys.Count > sampleCount)
                accuracys.RemoveAt(0);

            this.accuracy = accuracys.Average();
        }

        [NonSerialized] Texture2D loadedTexture;
        public void TextureRefresh()
        {
            if (loadedTexture != null)
                DestroyImmediate(loadedTexture);

            Texture2D texture = new Texture2D(rectTransform.rect.width.RoundToInt(), 1);
            loadedTexture = texture;
            texture.filterMode = FilterMode.Point;
            barImage.texture = texture;

            Color[] colors = new Color[texture.width];
            JudgementMetaData[] metaDatas = new SDJKRuleset().judgementMetaDatas;
            double lastDisSecond = metaDatas[metaDatas.Length - 1].sizeSecond;
            SetColors(texture.width / 2, 0, false);
            SetColors(texture.width, texture.width / 2, true);

            void SetColors(int lerpX, int lerpY, bool loopReverse)
            {
                for (int i = 0; i < metaDatas.Length; i++)
                {
                    JudgementMetaData metaData = metaDatas[i];
                    int current = lerpX.Lerp(lerpY, metaData.sizeSecond / lastDisSecond);
                    int next;
                    if (i - 1 >= 0)
                    {
                        JudgementMetaData nextMetaData = metaDatas[i - 1];
                        next = lerpX.Lerp(lerpY, nextMetaData.sizeSecond / lastDisSecond);
                    }
                    else
                        next = lerpX;

                    if (!loopReverse)
                    {
                        for (int j = current; j < next; j++)
                            colors[j] = metaData.color;
                    }
                    else
                    {
                        for (int j = current; j < next; j++)
                            colors[(lerpX - j) + lerpY - 1] = metaData.color;
                    }
                }
            }

            texture.SetPixels(colors);
            texture.Apply();
        }

        double GetAnchorPos(double accuracy)
        {
            if (accuracy < 0)
                return 0.5.Lerp(0d, -accuracy);
            else
                return 0.5.Lerp(1d, accuracy);
        }
    }
}
