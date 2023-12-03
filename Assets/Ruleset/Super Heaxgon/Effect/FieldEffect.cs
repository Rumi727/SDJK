using SCKRM;
using SCKRM.Json;
using SCKRM.Rhythm;
using SDJK.Map.Ruleset.SuperHexagon.Map;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon.Effect
{
    public sealed class FieldEffect : SuperHexagonEffect
    {
        [SerializeField, FieldNotNull] Field _field; public Field field => _field;

        ThemeEffect<BeatValuePairAniListFloat, float> fieldXPosition = new();
        ThemeEffect<BeatValuePairAniListFloat, float> fieldYPosition = new();
        ThemeEffect<BeatValuePairAniListFloat, float> fieldZPosition = new();

        ThemeEffect<BeatValuePairAniListFloat, float> fieldXRotation = new();
        ThemeEffect<BeatValuePairAniListFloat, float> fieldYRotation = new();
        ThemeEffect<BeatValuePairAniListFloat, float> fieldZRotation = new();

        ThemeEffect<BeatValuePairAniListFloat, float> fieldAutoZRotationSpeed = new();

        ThemeEffect<BeatValuePairAniListColor, JColor> backgroundColor = new();
        ThemeEffect<BeatValuePairAniListColor, JColor> backgroundColorAlt = new();

        ThemeEffect<BeatValuePairAniListColor, JColor> mainColor = new();
        ThemeEffect<BeatValuePairAniListColor, JColor> mainColorAlt = new();

        float fieldAutoZRotation = 0;
        protected override void RealUpdate()
        {
            double currentBeat = RhythmManager.currentBeatScreen;
            SuperHexagonThemeFile theme = map.effect.themes.GetValue(currentBeat, out double beat);

            #region Field Rotation
            float fieldXPosition = this.fieldXPosition.Update(theme, beat, theme.fieldXPosition);
            float fieldYPosition = this.fieldYPosition.Update(theme, beat, theme.fieldYPosition);
            float fieldZPosition = this.fieldZPosition.Update(theme, beat, theme.fieldZPosition);

            float fieldXRotation = this.fieldXRotation.Update(theme, beat, theme.fieldXRotation);
            float fieldYRotation = this.fieldYRotation.Update(theme, beat, theme.fieldYRotation);
            float fieldZRotation = this.fieldZRotation.Update(theme, beat, theme.fieldZRotation);

            if (theme.fieldAutoZRotationDisable.GetValue(currentBeat))
            {
                if (0 < fieldAutoZRotation)
                    fieldAutoZRotation = fieldAutoZRotation.MoveTowards(0, (fieldAutoZRotation.Ceil() - 0) * (0.0625f * Kernel.fpsSmoothDeltaTime));
                else
                    fieldAutoZRotation = fieldAutoZRotation.MoveTowards(0, (0 - fieldAutoZRotation.Floor()) * (0.0625f * Kernel.fpsSmoothDeltaTime));
            }
            else
            {
                fieldAutoZRotation += fieldAutoZRotationSpeed.Update(theme, beat, theme.fieldAutoZRotationSpeed) * Kernel.fpsSmoothDeltaTime * (float)RhythmManager.speed;
                fieldAutoZRotation = fieldAutoZRotation.Repeat(360);
            }

            field.transform.localPosition = map.effect.fieldPosition.GetValue(currentBeat) + new Vector3(fieldXPosition, fieldYPosition, fieldZPosition);
            field.transform.localEulerAngles = map.effect.fieldRotation.GetValue(currentBeat) + new Vector3(fieldXRotation, fieldYRotation, fieldZRotation + fieldAutoZRotation);
            #endregion

            #region Background Color
            JColor backgroundColor = this.backgroundColor.Update(theme, beat, theme.backgroundColor);
            JColor backgroundColorAlt = this.backgroundColorAlt.Update(theme, beat, theme.backgroundColorAlt);

            field.isBackgroundAltReversal = theme.backgroundColorAltReversal.GetValue((currentBeat - beat).Repeat(theme.backgroundColorAltReversal.Last().beat));

            field.backgroundColor = backgroundColor;
            field.backgroundColorAlt = backgroundColorAlt;
            #endregion

            #region Main Color
            JColor mainColor = this.mainColor.Update(theme, beat, theme.mainColor);
            JColor mainColorAlt = this.mainColorAlt.Update(theme, beat, theme.mainColorAlt);

            field.isMainColorAltReversal = theme.mainColorAltReversal.GetValue((currentBeat - beat).Repeat(theme.mainColorAltReversal.Last().beat));

            field.mainColor = mainColor;
            field.mainColorAlt = mainColorAlt;
            #endregion
        }

        class ThemeEffect<TList, TListType> where TList : BeatValuePairAniList<TListType> where TListType : struct
        {
            public SuperHexagonThemeFile theme;
            public SuperHexagonThemeFile lastTheme;

            public double startBeat;

            public TListType currentValue;
            public TListType? lastValue;

            public TListType Update(SuperHexagonThemeFile theme, double startBeat, TList list)
            {
                if (lastTheme != theme)
                {
                    if (lastValue == null)
                        lastValue = list.GetValue(double.MinValue);
                    else
                        lastValue = this.currentValue;

                    this.theme = theme;
                    lastTheme = theme;
                }

                double currentBeat = RhythmManager.currentBeatScreen - startBeat;
                double currentBeatRepeat = currentBeat.Repeat(list.Last().beat);
                TListType currentValue = list.GetValue(currentBeatRepeat);

                double t = (currentBeat / theme.transitionLength).Clamp01();
                if (!double.IsNormal(t))
                    t = 0;

                return this.currentValue = list.ValueCalculate(t, theme.transitionEasingFunction, (TListType)lastValue, currentValue);
            }
        }
    }
}
