using SCKRM;
using SCKRM.Easing;
using SCKRM.Json;
using SDJK.Map;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.Map
{
    static class SDJKLoader
    {
        [Awaken]
        static void Awaken()
        {
            MapLoader.extensionToLoad.Add("sdjk");
            MapLoader.mapLoaderFunc += (Type type, string mapFilePath, string extension) =>
            {
                if (extension == ".sdjk" && (type == typeof(MapFile) || type == typeof(SDJKMapFile)))
                {
                    {
                        SDJKMapFile map = JsonManager.JsonRead<SDJKMapFile>(mapFilePath, true);
                        if (map == null)
                            return null;
                        else if (map.info.sdjkVersion != default)
                        {
                            if (map.info.mode == typeof(SDJKRuleset).FullName)
                                return map;

                            return null;
                        }
                    }

                    try
                    {
                        SDJKMapFile map = new SDJKMapFile();
                        OldSDJK oldMap = JsonManager.JsonRead<OldSDJK>(mapFilePath, true);

                        #region Default Effect
                        for (int i = 0; i < oldMap.AllBeat.Count; i++)
                        {
                            if (i < oldMap.AllBeat.Count - 1)
                                map.allBeat.Add(oldMap.AllBeat[i] - 1);
                        }



                        map.info.sckrmVersion = Kernel.sckrmVersion;
                        map.info.sdjkVersion = new SCKRM.Version(Kernel.version);



                        map.info.mode = typeof(SDJKRuleset).FullName;



                        map.info.songFile = oldMap.BGM;



                        map.globalEffect.background.Add(new BackgroundEffectPair(oldMap.Background, oldMap.BackgroundNight));
                        map.globalEffect.backgroundColor.Add(JColor.one);

                        map.info.videoBackgroundFile = oldMap.VideoBackground;
                        map.info.videoBackgroundNightFile = oldMap.VideoBackgroundNight;
                        map.globalEffect.videoColor.Add(JColor.one);

                        map.info.videoOffset = oldMap.VideoOffset;



                        map.info.artist = oldMap.Artist;
                        map.info.songName = oldMap.BGMName;

                        {
                            string difficulty = oldMap.Difficulty;
                            if (difficulty == "very_easy")
                                map.info.difficultyLabel = "Very Easy (SDJK 1.0)";
                            else if (difficulty == "easy")
                                map.info.difficultyLabel = "Easy (SDJK 1.0)";
                            else if (difficulty == "normal")
                                map.info.difficultyLabel = "Normal (SDJK 1.0)";
                            else if (difficulty == "hard")
                                map.info.difficultyLabel = "Hard (SDJK 1.0)";
                            else if (difficulty == "very_hard")
                                map.info.difficultyLabel = "Very Hard (SDJK 1.0)";
                            else
                                map.info.difficultyLabel = difficulty + " (SDJK 1.0)";
                        }



                        map.info.songOffset = oldMap.Offset;
                        map.info.mainMenuStartTime = oldMap.MainMenuStartTime;
                        #endregion

                        #region Beat
                        NoteAdd(oldMap.CapsLock, oldMap.HoldCapsLock);
                        NoteAdd(oldMap.A, oldMap.HoldA);
                        NoteAdd(oldMap.S, oldMap.HoldS);
                        NoteAdd(oldMap.D, oldMap.HoldD);
                        NoteAdd(oldMap.J, oldMap.HoldJ);
                        NoteAdd(oldMap.K, oldMap.HoldK);
                        NoteAdd(oldMap.L, oldMap.HoldL);
                        NoteAdd(oldMap.Semicolon, oldMap.HoldSemicolon);

                        void NoteAdd(List<double> list, List<double> holdList)
                        {
                            if (list.Count <= 0)
                                return;

                            List<NoteFile> notes = new List<NoteFile>();
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (list.Count != holdList.Count)
                                    notes.Add(new NoteFile(list[i] - 1, 0, NoteTypeFile.normal));
                                else
                                {
                                    if (holdList[i] >= -1 && holdList[i] < 0)
                                        notes.Add(new NoteFile(list[i] - 1, 0, NoteTypeFile.instantDeath));
                                    else
                                        notes.Add(new NoteFile(list[i] - 1, holdList[i], NoteTypeFile.normal));
                                }
                            }

                            map.notes.Add(notes);
                        }
                        #endregion

                        #region Field, Bar Effect
                        {
                            FieldEffectFile fieldEffect = new FieldEffectFile();
                            for (int i = 0; i < map.notes.Count; i++)
                                fieldEffect.barEffect.Add(new BarEffectFile());

                            map.effect.fieldEffect.Add(fieldEffect);
                        }
                        #endregion

                        #region Effect Method
                        void EffectAdd<T>(T defaultValue, List<OldSDJK.EffectValue<T>> oldList, SCKRM.Rhythm.BeatValuePairList<T> list)
                        {
                            list.Add(double.MinValue, defaultValue);
                            for (int i = 0; i < oldList.Count; i++)
                            {
                                var effect = oldList[i];
                                list.Add(effect.Beat - 1, effect.Value);
                            }
                        }

                        void EffectAdd2<T>(T defaultValue, List<OldSDJK.EffectValue<T>> oldList, BeatValuePairList<T> list)
                        {
                            list.Add(double.MinValue, defaultValue);
                            for (int i = 0; i < oldList.Count; i++)
                            {
                                var effect = oldList[i];
                                list.Add(effect.Beat - 1, effect.Value);
                            }
                        }

                        void EffectAdd3<T>(T defaultValue, List<OldSDJK.EffectValueLerp<T>> oldList, BeatValuePairAniList<T> list)
                        {
                            list.Add(double.MinValue, defaultValue);
                            for (int i = 0; i < oldList.Count; i++)
                            {
                                var effect = oldList[i];
                                list.Add(effect.Beat - 1, LerpToBeat(effect.Lerp, effect.Beat - 1), effect.Value, EasingFunction.Ease.EaseOutExpo);
                            }
                        }

                        void EffectAdd4<T>(T defaultValue, List<OldSDJK.EffectValueLerp<T>> oldList, SCKRM.Rhythm.BeatValuePairAniList<T> list)
                        {
                            list.Add(double.MinValue, defaultValue);
                            for (int i = 0; i < oldList.Count; i++)
                            {
                                var effect = oldList[i];
                                list.Add(effect.Beat - 1, LerpToBeat(effect.Lerp, effect.Beat - 1), effect.Value, EasingFunction.Ease.EaseOutExpo);
                            }
                        }

                        void EffectAdd5(JVector3 defaultValue, List<OldSDJK.EffectValueLerp<OldSDJK.JVector3>> oldList, BeatValuePairAniList<JVector3> list)
                        {
                            list.Add(double.MinValue, defaultValue);
                            for (int i = 0; i < oldList.Count; i++)
                            {
                                var effect = oldList[i];
                                list.Add(effect.Beat - 1, LerpToBeat(effect.Lerp, effect.Beat - 1), effect.Value, EasingFunction.Ease.EaseOutExpo);
                            }
                        }

                        void EffectAdd6(JColor defaultValue, List<OldSDJK.EffectValueLerp<OldSDJK.JColor>> oldList, BeatValuePairAniList<JColor> list)
                        {
                            list.Add(double.MinValue, defaultValue);
                            for (int i = 0; i < oldList.Count; i++)
                            {
                                var effect = oldList[i];
                                list.Add(effect.Beat - 1, LerpToBeat(effect.Lerp, effect.Beat - 1), effect.Value, EasingFunction.Ease.EaseOutExpo);
                            }
                        }

                        double LerpToBeat(double lerp, double beat)
                        {
                            if (lerp >= 1)
                                return 0;
                            else
                                return (0.1 / lerp) * (map.globalEffect.bpm.GetValue(beat) / 60);
                        }
                        #endregion

                        #region Effect
                        EffectAdd(oldMap.Effect.BPM, oldMap.Effect.BPMEffect, map.globalEffect.bpm);
                        EffectAdd(oldMap.Effect.DropPart, oldMap.Effect.DropPartEffect, map.globalEffect.dropPart);

                        EffectAdd3(oldMap.Effect.Camera.CameraZoom, oldMap.Effect.Camera.CameraZoomEffect, map.globalEffect.cameraZoom);
                        EffectAdd5(oldMap.Effect.Camera.CameraPos, oldMap.Effect.Camera.CameraPosEffect, map.globalEffect.cameraPos);
                        EffectAdd5(oldMap.Effect.Camera.CameraRotation, oldMap.Effect.Camera.CameraRotationEffect, map.globalEffect.cameraRotation);

                        EffectAdd4(oldMap.Effect.Pitch, oldMap.Effect.PitchEffect, map.globalEffect.pitch);
                        map.globalEffect.tempo.Add(double.MinValue, 0, 1);

                        EffectAdd3(oldMap.Effect.Volume, oldMap.Effect.VolumeEffect, map.globalEffect.volume);

                        EffectAdd4(oldMap.Effect.HPAddValue, oldMap.Effect.HPAddValueEffect, map.globalEffect.hpAddValue);
                        EffectAdd4(oldMap.Effect.HPRemoveValue, oldMap.Effect.HPRemoveValueEffect, map.globalEffect.hpMissValue);

                        {
                            var effect = oldMap.Effect.HPRemove;
                            if (effect)
                                map.globalEffect.hpRemoveValue.Add(double.MinValue, 0, oldMap.Effect.HPRemoveValue);
                            else
                                map.globalEffect.hpRemoveValue.Add(double.MinValue, 0, 0);
                        }

                        for (int i = 0; i < oldMap.Effect.HPRemoveEffect.Count; i++)
                        {
                            var effect = oldMap.Effect.HPRemoveEffect[i];
                            if (effect.Value)
                                map.globalEffect.hpRemoveValue.Add(effect.Beat - 1, 0, oldMap.Effect.HPRemoveValue);
                            else
                                map.globalEffect.hpRemoveValue.Add(effect.Beat - 1, 0, 0);
                        }

                        EffectAdd4(oldMap.Effect.JudgmentSize, oldMap.Effect.JudgmentSizeEffect, map.globalEffect.judgmentSize);

                        {
                            FieldEffectFile fieldEffect = map.effect.fieldEffect[0];
                            EffectAdd5(oldMap.Effect.Camera.UiPos, oldMap.Effect.Camera.UiPosEffect, fieldEffect.pos);
                            EffectAdd5(oldMap.Effect.Camera.UiRotation, oldMap.Effect.Camera.UiRotationEffect, fieldEffect.rotation);

                            fieldEffect.height.Add(double.MinValue, 0, oldMap.Effect.Camera.UiZoom * 16);
                            for (int i = 0; i < oldMap.Effect.Camera.UiZoomEffect.Count; i++)
                            {
                                var effect = oldMap.Effect.Camera.UiZoomEffect[i];
                                fieldEffect.height.Add(effect.Beat - 1, LerpToBeat(effect.Lerp, effect.Beat - 1), effect.Value * 16, EasingFunction.Ease.EaseOutExpo);
                            }

                            {
                                List<BarEffectFile> barEffect = fieldEffect.barEffect;
                                bool capsLock = true;
                                bool a = true;
                                bool s = true;
                                bool d = true;
                                bool j = true;
                                bool k = true;
                                bool l = true;
                                bool semiccolon = true;
                                for (int i = 0; i < map.notes.Count; i++)
                                {
                                    if (oldMap.CapsLock.Count > 0 && capsLock)
                                    {
                                        EffectAdd5(oldMap.Effect.CapsLockBarPos, oldMap.Effect.CapsLockBarPosEffect, barEffect[i].pos);
                                        capsLock = false;
                                    }
                                    else if (oldMap.A.Count > 0 && a)
                                    {
                                        EffectAdd5(oldMap.Effect.ABarPos, oldMap.Effect.ABarPosEffect, barEffect[i].pos);
                                        a = false;
                                    }
                                    else if (oldMap.S.Count > 0 && s)
                                    {
                                        EffectAdd5(oldMap.Effect.SBarPos, oldMap.Effect.SBarPosEffect, barEffect[i].pos);
                                        s = false;
                                    }
                                    else if (oldMap.J.Count > 0 && d)
                                    {
                                        EffectAdd5(oldMap.Effect.DBarPos, oldMap.Effect.DBarPosEffect, barEffect[i].pos);
                                        d = false;
                                    }
                                    else if (oldMap.J.Count > 0 && j)
                                    {
                                        EffectAdd5(oldMap.Effect.JBarPos, oldMap.Effect.JBarPosEffect, barEffect[i].pos);
                                        j = false;
                                    }
                                    else if (oldMap.K.Count > 0 && k)
                                    {
                                        EffectAdd5(oldMap.Effect.KBarPos, oldMap.Effect.KBarPosEffect, barEffect[i].pos);
                                        k = false;
                                    }
                                    else if (oldMap.L.Count > 0 && l)
                                    {
                                        EffectAdd5(oldMap.Effect.LBarPos, oldMap.Effect.LBarPosEffect, barEffect[i].pos);
                                        l = false;
                                    }
                                    else if (oldMap.Semicolon.Count > 0 && semiccolon)
                                    {
                                        EffectAdd5(oldMap.Effect.SemicolonBarPos, oldMap.Effect.SemicolonBarPosEffect, barEffect[i].pos);
                                        semiccolon = false;
                                    }
                                }
                            }

                            for (int i = 0; i < map.notes.Count; i++)
                            {
                                BarEffectFile barEffect = fieldEffect.barEffect[i];

                                EffectAdd6(oldMap.Effect.BarColor, oldMap.Effect.BarColorEffect, barEffect.color);
                                EffectAdd6(oldMap.Effect.NoteColor, oldMap.Effect.NoteColorEffect, barEffect.noteColor);

                                barEffect.noteOffset.Add(double.MinValue, 0, 0);
                                double previousValue = 0;
                                for (int j = 0; j < oldMap.Effect.NoteOffsetEffect.Count; j++)
                                {
                                    var effect = oldMap.Effect.NoteOffsetEffect[j];
                                    if (effect.Add)
                                    {
                                        barEffect.noteOffset.Add(effect.Beat - 1, LerpToBeat(effect.Lerp, effect.Beat - 1), previousValue + effect.Value.y, EasingFunction.Ease.EaseOutExpo);
                                        previousValue = previousValue + effect.Value.y;
                                    }
                                    else
                                    {
                                        barEffect.noteOffset.Add(effect.Beat - 1, LerpToBeat(effect.Lerp, effect.Beat - 1), effect.Value.y, EasingFunction.Ease.EaseOutExpo);
                                        previousValue = effect.Value.y;
                                    }
                                }

                                EffectAdd2(oldMap.Effect.NoteStop, oldMap.Effect.NoteStopEffect, fieldEffect.barEffect[i].noteStop);
                            }
                        }

                        EffectAdd3(oldMap.Effect.BeatYPos, oldMap.Effect.BeatYPosEffect, map.effect.globalNoteDistance);
                        #endregion

                        #region Effect Stacking Trick Method
                        void EffectStackingTrick<T>(BeatValuePairAniList<T> list)
                        {
                            for (int i = 1; i < list.Count - 1; i++)
                            {
                                var previousEffect = list[i - 1];
                                var effect = list[i];
                                var nextEffect = list[i + 1];

                                if (effect.beat + effect.length > nextEffect.beat && (nextEffect.beat - effect.beat) / effect.length < 0.25)
                                {
                                    double t = ((nextEffect.beat - effect.beat) / effect.length).Clamp01();
                                    T calculatedValue = list.ValueCalculate(t, EasingFunction.GetEasingFunction(effect.easingFunction), previousEffect, effect);

                                    var modifyedEffect = list[i];
                                    modifyedEffect.length = nextEffect.beat - effect.beat;
                                    modifyedEffect.value = calculatedValue;
                                    modifyedEffect.easingFunction = EasingFunction.Ease.Linear;

                                    list[i] = modifyedEffect;
                                }
                            }
                        }

                        void EffectStackingTrick2<T>(SCKRM.Rhythm.BeatValuePairAniList<T> list)
                        {
                            for (int i = 1; i < list.Count - 1; i++)
                            {
                                var previousEffect = list[i - 1];
                                var effect = list[i];
                                var nextEffect = list[i + 1];

                                if (effect.beat + effect.length > nextEffect.beat && (nextEffect.beat - effect.beat) / effect.length < 0.25)
                                {
                                    double t = ((nextEffect.beat - effect.beat) / effect.length).Clamp01();
                                    T calculatedValue = list.ValueCalculate(t, EasingFunction.GetEasingFunction(effect.easingFunction), previousEffect, effect);

                                    var modifyedEffect = list[i];
                                    modifyedEffect.length = nextEffect.beat - effect.beat;
                                    modifyedEffect.value = calculatedValue;
                                    modifyedEffect.easingFunction = EasingFunction.Ease.Linear;

                                    list[i] = modifyedEffect;
                                }
                            }
                        }
                        #endregion

                        #region Effect Stacking Trick
                        EffectStackingTrick(map.globalEffect.cameraZoom);
                        EffectStackingTrick(map.globalEffect.cameraPos);
                        EffectStackingTrick(map.globalEffect.cameraRotation);

                        EffectStackingTrick2(map.globalEffect.pitch);
                        EffectStackingTrick(map.globalEffect.volume);

                        EffectStackingTrick2(map.globalEffect.hpAddValue);
                        EffectStackingTrick2(map.globalEffect.hpRemoveValue);
                        EffectStackingTrick2(map.globalEffect.hpMissValue);

                        EffectStackingTrick2(map.globalEffect.judgmentSize);

                        EffectStackingTrick(map.effect.fieldEffect[0].pos);
                        EffectStackingTrick(map.effect.fieldEffect[0].rotation);
                        EffectStackingTrick(map.effect.fieldEffect[0].height);

                        EffectStackingTrick(map.effect.globalNoteDistance);
                        #endregion

                        return map;
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        return null;
                    }
                }
                else
                    return null;
            };
        }

        class OldSDJK
        {
            public List<double> CapsLock = new List<double>();
            public List<double> A = new List<double>();
            public List<double> S = new List<double>();
            public List<double> D = new List<double>();
            public List<double> J = new List<double>();
            public List<double> K = new List<double>();
            public List<double> L = new List<double>();
            public List<double> Semicolon = new List<double>();

            public List<double> HoldCapsLock = new List<double>();
            public List<double> HoldA = new List<double>();
            public List<double> HoldS = new List<double>();
            public List<double> HoldD = new List<double>();
            public List<double> HoldJ = new List<double>();
            public List<double> HoldK = new List<double>();
            public List<double> HoldL = new List<double>();
            public List<double> HoldSemicolon = new List<double>();

            public List<double> AllBeat = new List<double>();

            public EffectList Effect = new EffectList();

            public string BGM = "";
            public string Background = "";
            public string BackgroundNight = "";
            public string VideoBackground = "";
            public string VideoBackgroundNight = "";
            public double VideoOffset = 0;

            public string Artist = "none";
            public string BGMName = "none";
            public double Offset;
            public string Difficulty = "";

            public bool HitSoundSimultaneousPlayAllow = false;

            public string Cover = "";
            public string CoverNight = "";

            public double MainMenuStartTime = 0;

            public class EffectList
            {
                public CameraEffect Camera = new CameraEffect();

                public double BPM = 100;
                public List<EffectValue<double>> BPMEffect = new List<EffectValue<double>>();

                public double BeatYPos = 3;
                public List<EffectValueLerp<double>> BeatYPosEffect = new List<EffectValueLerp<double>>();

                public double Pitch = 1;
                public List<EffectValueLerp<double>> PitchEffect = new List<EffectValueLerp<double>>();

                public double Volume = 1;
                public List<EffectValueLerp<double>> VolumeEffect = new List<EffectValueLerp<double>>();

                public bool HPRemove = true;
                public List<EffectValue<bool>> HPRemoveEffect = new List<EffectValue<bool>>();

                public double HPAddValue = 0.5f;
                public List<EffectValueLerp<double>> HPAddValueEffect = new List<EffectValueLerp<double>>();

                public double HPRemoveValue = 0.5f;
                public List<EffectValueLerp<double>> HPRemoveValueEffect = new List<EffectValueLerp<double>>();

                public double MaxHPValue = 100;
                public List<EffectValueLerp<double>> MaxHPValueEffect = new List<EffectValueLerp<double>>();

                public double JudgmentSize = 1;
                public List<EffectValueLerp<double>> JudgmentSizeEffect = new List<EffectValueLerp<double>>();

                public bool AudioSpectrumUse = false;
                public JColor32 AudioSpectrumColor = new JColor32(255);

                public double WindowSize = 1;
                public List<EffectValueLerp<double>> WindowSizeEffect = new List<EffectValueLerp<double>>();

                public JVector2 WindowPos = new JVector2();
                public DatumPoint WindowDatumPoint = DatumPoint.Center;
                public DatumPoint ScreenDatumPoint = DatumPoint.Center;
                public List<WindowPosEffect> WindowPosEffect = new List<WindowPosEffect>();

                public JVector3 CapsLockBarPos = new JVector3();
                public List<EffectValueLerp<JVector3>> CapsLockBarPosEffect = new List<EffectValueLerp<JVector3>>();
                public JVector3 ABarPos = new JVector3();
                public List<EffectValueLerp<JVector3>> ABarPosEffect = new List<EffectValueLerp<JVector3>>();
                public JVector3 SBarPos = new JVector3();
                public List<EffectValueLerp<JVector3>> SBarPosEffect = new List<EffectValueLerp<JVector3>>();
                public JVector3 DBarPos = new JVector3();
                public List<EffectValueLerp<JVector3>> DBarPosEffect = new List<EffectValueLerp<JVector3>>();
                public JVector3 JBarPos = new JVector3();
                public List<EffectValueLerp<JVector3>> JBarPosEffect = new List<EffectValueLerp<JVector3>>();
                public JVector3 KBarPos = new JVector3();
                public List<EffectValueLerp<JVector3>> KBarPosEffect = new List<EffectValueLerp<JVector3>>();
                public JVector3 LBarPos = new JVector3();
                public List<EffectValueLerp<JVector3>> LBarPosEffect = new List<EffectValueLerp<JVector3>>();
                public JVector3 SemicolonBarPos = new JVector3();
                public List<EffectValueLerp<JVector3>> SemicolonBarPosEffect = new List<EffectValueLerp<JVector3>>();

                public bool DropPart = false;
                public List<EffectValue<bool>> DropPartEffect = new List<EffectValue<bool>>();

                public bool NoteStop = false;
                public List<EffectValue<bool>> NoteStopEffect = new List<EffectValue<bool>>();
                public JVector3 NoteOffset = new JVector3();
                public List<NoteOffsetEffectValue> NoteOffsetEffect = new List<NoteOffsetEffectValue>();

                public JColor NoteColor = new JColor(0, 1, 0, 1);
                public List<EffectValueLerp<JColor>> NoteColorEffect = new List<EffectValueLerp<JColor>>();

                public JColor BarColor = new JColor(1, 1, 1, 1);
                public List<EffectValueLerp<JColor>> BarColorEffect = new List<EffectValueLerp<JColor>>();

                public List<double> AllBeat = new List<double>();
            }

            public class CameraEffect
            {
                public double CameraZoom = 1;
                public List<EffectValueLerp<double>> CameraZoomEffect = new List<EffectValueLerp<double>>();
                public JVector3 CameraPos = new JVector3(0, 0, -14);
                public List<EffectValueLerp<JVector3>> CameraPosEffect = new List<EffectValueLerp<JVector3>>();
                public JVector3 CameraRotation = new JVector3();
                public List<EffectValueLerp<JVector3>> CameraRotationEffect = new List<EffectValueLerp<JVector3>>();

                public double UiZoom = 1;
                public List<EffectValueLerp<double>> UiZoomEffect = new List<EffectValueLerp<double>>();
                public JVector3 UiPos = new JVector3();
                public List<EffectValueLerp<JVector3>> UiPosEffect = new List<EffectValueLerp<JVector3>>();
                public JVector3 UiRotation = new JVector3();
                public List<EffectValueLerp<JVector3>> UiRotationEffect = new List<EffectValueLerp<JVector3>>();
            }

            public struct WindowPosEffect
            {
                public double Beat;
                public JVector2 Pos;
                public DatumPoint WindowDatumPoint;
                public DatumPoint ScreenDatumPoint;
                public double Lerp;
                public bool Disturbance;

                public static WindowPosEffect Default = new WindowPosEffect(0, new JVector2(), DatumPoint.Center, DatumPoint.Center, 1, false);

                public WindowPosEffect(double Beat, JVector2 Pos, DatumPoint WindowDatumPoint, DatumPoint ScreenDatumPoint, double Lerp, bool Disturbance)
                {
                    this.Beat = Beat;
                    this.Pos = Pos;
                    this.WindowDatumPoint = WindowDatumPoint;
                    this.ScreenDatumPoint = ScreenDatumPoint;
                    this.Lerp = Lerp;
                    this.Lerp = Lerp;
                    this.Disturbance = Disturbance;
                }
            }

            public struct EffectValueLerp<T>
            {
                public double Beat;
                public T Value;
                public double Lerp;
                public bool Disturbance;

                public static EffectValueLerp<T> Default = new EffectValueLerp<T>(0, default(T), 1, false);

                public EffectValueLerp(double Beat, T Value, double Lerp, bool Disturbance)
                {
                    this.Beat = Beat;
                    this.Value = Value;
                    this.Lerp = Lerp;
                    this.Disturbance = Disturbance;
                }
            }

            public struct EffectValue<T>
            {
                public double Beat;
                public T Value;
                public bool Disturbance;

                public static EffectValue<T> Default = new EffectValue<T>(0, default(T), false);

                public EffectValue(double Beat, T Value, bool Disturbance)
                {
                    this.Beat = Beat;
                    this.Value = Value;
                    this.Disturbance = Disturbance;
                }
            }

            public struct NoteOffsetEffectValue
            {
                public double Beat;
                public bool Add;
                public JVector3 Value;
                public float Lerp;
                public bool Disturbance;

                public static NoteOffsetEffectValue Default = new NoteOffsetEffectValue(0, true, new JVector3(), 1, false);

                public NoteOffsetEffectValue(double Beat, bool Add, JVector3 Value, float Lerp, bool Disturbance)
                {
                    this.Beat = Beat;
                    this.Add = Add;
                    this.Value = Value;
                    this.Lerp = Lerp;
                    this.Disturbance = Disturbance;
                }
            }

            public struct JVector2
            {
                public float x;
                public float y;

                public static implicit operator SCKRM.Json.JVector2(JVector2 value) => new SCKRM.Json.JVector2(value.x, value.y);
            }

            public struct JVector3
            {
                public float x;
                public float y;
                public float z;

                public JVector3(float x, float y, float z)
                {
                    this.x = x;
                    this.y = y;
                    this.z = z;
                }

                public static implicit operator SCKRM.Json.JVector3(JVector3 value) => new SCKRM.Json.JVector3(value.x, value.y, value.z);
            }

            public struct JColor
            {
                public float r;
                public float g;
                public float b;
                public float a;

                public JColor(float r, float g, float b, float a)
                {
                    this.r = r;
                    this.g = g;
                    this.b = b;
                    this.a = a;
                }

                public static implicit operator SCKRM.Json.JColor(JColor value) => new SCKRM.Json.JColor(value.r, value.g, value.b, value.a);
            }

            public struct JColor32
            {
                public float r;
                public float g;
                public float b;
                public float a;

                public JColor32(float f) => r = g = b = a = f;

                public static implicit operator SCKRM.Json.JColor32(JColor32 value) => new SCKRM.Json.JColor32((byte)value.r, (byte)value.g, (byte)value.b, (byte)value.a);
            }

            public enum DatumPoint
            {
                LeftTop,
                RightTop,
                LeftCenter,
                RightCenter,
                LeftBottom,
                RightBottom,
                CenterTop,
                CenterBottom,
                Center
            }
        }
    }
}
