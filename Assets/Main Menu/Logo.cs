using SCKRM;
using SCKRM.Easing;
using SCKRM.Input;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SCKRM.UI;
using SDJK.Effect;
using SDJK.Map;
using UnityEngine;

namespace SDJK.MainMenu
{
    public sealed class Logo : UIBase
    {
        BeatValuePairAniListFloat beatScaleAni = new BeatValuePairAniListFloat(0) { new BeatValuePairAni<float>(0, 0.95f, 0, EasingFunction.Ease.Linear), new BeatValuePairAni<float>(0, 1, 0.9, EasingFunction.Ease.EaseOutSine), new BeatValuePairAni<float>(0.9, 0.95f, 0.1, EasingFunction.Ease.Linear) };
        
        float beatScale = 1;
        float pointerScaleStart = 1;
        float pointerScaleT = 0;
        float pointerScale = 1;
        float clickScale = 1;

        [SerializeField, FieldNotNull] LogoEffect logoEffect;
        [SerializeField] float pointerOffsetSize = 1;

        int lastCurrentBeat = 0;
        double lastBPMOffsetBeat = 0;
        double lastHitsoundBeat = -1;

        public bool pointer
        {
            get => _pointer;
            set
            {
                if (_pointer != value)
                {
                    _pointer = value;

                    pointerScaleStart = pointerScale;
                    pointerScaleT = 0;
                }
            }
        }
        bool _pointer = false;

        void Update()
        {
            {
                Rect rect = rectTransformTool.worldCorners.rect;
                pointer = Vector2.Distance(InputManager.mousePosition, rect.center) <= rect.width * 0.5f * pointerOffsetSize;
            }

            {
                if (pointerScaleT < 1)
                    pointerScaleT = (pointerScaleT + 0.03f * Kernel.fpsUnscaledSmoothDeltaTime).Clamp01();

                if (pointer)
                    pointerScale = (float)EasingFunction.EaseOutElastic(pointerScaleStart, 1.2, pointerScaleT);
                else
                    pointerScale = (float)EasingFunction.EaseOutElastic(pointerScaleStart, 1, pointerScaleT);
            }

            if (pointer)
            {
                if (InputManager.GetMouseButton(0, InputType.Alway))
                    clickScale = clickScale.Lerp(0.85f, 0.04f * Kernel.fpsUnscaledSmoothDeltaTime);
                else
                    clickScale = clickScale.Lerp(1, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);

                if (InputManager.GetMouseButton(0, InputType.Up))
                    MainMenu.NextScreen();
            }

            double oneBeat = (RhythmManager.currentBeatScreen - RhythmManager.bpmOffsetBeat).Repeat(1);
            beatScale = beatScaleAni.GetValue(oneBeat);

            if (!MainMenu.SaveData.logoMapHitsoundEnable)
            {
                int currentBeat = (int)(RhythmManager.currentBeatSound - RhythmManager.bpmOffsetBeat).Repeat(4);
                if (lastCurrentBeat != currentBeat || lastBPMOffsetBeat != RhythmManager.bpmOffsetBeat)
                {
                    if (pointer)
                    {
                        if (currentBeat == 0)
                            SoundManager.PlaySound("hitsound.normal", "sdjk", 0.5f, false, 1.35f);

                        SoundManager.PlaySound("hitsound.normal", "sdjk", 0.5f, false, 0.95f);
                    }

                    lastCurrentBeat = currentBeat;
                    lastBPMOffsetBeat = RhythmManager.bpmOffsetBeat;
                }
            }
            else
            {
                MapFile map = MapManager.selectedMap;
                int count = HitsoundEffect.GetHitsoundPlayCount(map, RhythmManager.currentBeatSound, ref lastHitsoundBeat);
                if (pointer)
                {
                    for (int i = 0; i < count; i++)
                        HitsoundEffect.DefaultHitsoundPlay();
                }

                {
                    HitsoundEffect.GetHitsoundPlayCount(map, RhythmManager.currentBeatScreen, ref lastHitsoundBeat, out int index);
                    if (index + 1 < map.allJudgmentBeat.Count)
                    {
                        double lastBeat = map.allJudgmentBeat[index];
                        double nextBeat = map.allJudgmentBeat[index + 1] - lastBeat;
                        double currentBeat = RhythmManager.currentBeatScreen - lastBeat;

                        beatScale = beatScaleAni.GetValue(currentBeat / nextBeat);
                    }
                }
            }

            if (!double.IsNormal(beatScale))
                beatScale = 1;

            transform.localScale = Vector3.one * beatScale * pointerScale * clickScale;
        }
    }
}
