using SCKRM;
using SCKRM.Json;
using SCKRM.Rhythm;
using SDJK.Map.Ruleset.SuperHexagon.Map;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Effect
{
    public sealed class FieldEffect : SuperHexagonEffect
    {
        [SerializeField, NotNull] Field _field; public Field field => _field;

        ThemeEffect<BeatValuePairAniListFloat, float> fieldXRotation = new();
        ThemeEffect<BeatValuePairAniListFloat, float> fieldYRotation = new();
        ThemeEffect<BeatValuePairAniListFloat, float> fieldZRotation = new();

        ThemeEffect<BeatValuePairAniListColor, JColor> backgroundColor = new();
        ThemeEffect<BeatValuePairAniListColor, JColor> backgroundColorAlt = new();

        ThemeEffect<BeatValuePairAniListColor, JColor> mainColor = new();
        ThemeEffect<BeatValuePairAniListColor, JColor> mainColorAlt = new();

        float autoZRotation = 0;
        protected override void RealUpdate()
        {
            double currentBeat = RhythmManager.currentBeatScreen;
            SuperHexagonThemeFile theme = map.effect.themes.GetValue(currentBeat, out double beat);

            #region Field Rotation
            float fieldXRotation = this.fieldXRotation.Update(theme, beat, theme.fieldXRotation);
            float fieldYRotation = this.fieldYRotation.Update(theme, beat, theme.fieldYRotation);
            float fieldZRotation = this.fieldZRotation.Update(theme, beat, theme.fieldZRotation);

            if (theme.autoZRotationDisable.GetValue(currentBeat))
            {
                if (0 < autoZRotation)
                    autoZRotation = autoZRotation.MoveTowards(0, (autoZRotation.Ceil() - 0) * (0.25f * Kernel.fpsSmoothDeltaTime));
                else
                    autoZRotation = autoZRotation.MoveTowards(0, (0 - autoZRotation.Floor()) * (0.25f * Kernel.fpsSmoothDeltaTime));
            }
            else
            {
                autoZRotation += theme.autoZRotationSpeed.GetValue(currentBeat) * Kernel.fpsSmoothDeltaTime;
                autoZRotation = autoZRotation.Repeat(360);
            }

            field.transform.localEulerAngles = new Vector3(fieldXRotation, fieldYRotation, fieldZRotation + autoZRotation);
            #endregion

            #region Background Color
            JColor backgroundColor = this.backgroundColor.Update(theme, beat, theme.backgroundColor);
            JColor backgroundColorAlt = this.backgroundColorAlt.Update(theme, beat, theme.backgroundColorAlt);

            if (theme.mainColorAltReversal.GetValue((currentBeat - beat).Repeat(theme.backgroundColorAltReversal.Last().beat)))
            {
                field.backgroundColor = backgroundColorAlt;
                field.backgroundColorAlt = backgroundColor;
            }
            else
            {
                field.backgroundColor = backgroundColor;
                field.backgroundColorAlt = backgroundColorAlt;
            }
            #endregion

            #region Main Color
            JColor mainColor = this.mainColor.Update(theme, beat, theme.mainColor);
            JColor mainColorAlt = this.mainColorAlt.Update(theme, beat, theme.mainColorAlt);

            if (theme.mainColorAltReversal.GetValue((currentBeat - beat).Repeat(theme.mainColorAltReversal.Last().beat)))
            {
                field.mainColor = mainColorAlt;
                field.mainColorAlt = mainColor;
            }
            else
            {
                field.mainColor = mainColor;
                field.mainColorAlt = mainColorAlt;
            }
            #endregion
        }

        class ThemeEffect<TList, TListType> where TList : BeatValuePairAniList<TListType> where TListType : struct
        {
            public SuperHexagonThemeFile lastTheme;
            public SuperHexagonThemeFile theme;
            public double startBeat;

            public TListType currentValue;

            public double? lastStartBeat;
            public TListType? lastValue;

            public TListType Update(SuperHexagonThemeFile theme, double startBeat, TList list)
            {
                if (lastTheme != theme)
                {
                    if (lastStartBeat == null)
                        lastStartBeat = startBeat;
                    else
                        lastStartBeat = this.startBeat;

                    if (lastValue == null)
                        lastValue = list.GetValue(double.MinValue);
                    else
                        lastValue = this.currentValue;

                    this.theme = theme;
                    this.startBeat = startBeat;

                    lastTheme = theme;
                }

                double currentBeat = (RhythmManager.currentBeatScreen - startBeat).Repeat(list.Last().beat);
                TListType currentValue = list.GetValue(currentBeat);

                double t = ((currentBeat - (double)lastStartBeat) / theme.transitionLength).Clamp01();
                if (!double.IsNormal(t))
                    t = 0;

                return this.currentValue = list.ValueCalculate(t, theme.transitionEasingFunction, (TListType)lastValue, currentValue);
            }
        }
    }
}
