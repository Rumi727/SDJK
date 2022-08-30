using Newtonsoft.Json.Linq;
using SCKRM;
using SCKRM.Easing;
using SCKRM.Json;
using SDJK.Map;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;

using Version = SCKRM.Version;

namespace SDJK
{
    internal static class MapCompatibilitySystem
    {
        public static string[] compatibleMapExtensions { get; } = new string[] { "sdjk", "adofai" };

        public static T GlobalMapCompatibility<T>(string mapFilePath) where T : Map.Map, new()
        {
            string extension = Path.GetExtension(mapFilePath);
            if (extension == ".adofai")
            {
                try
                {
                    T sdjk = new T();
                    List<double> allBeat = new List<double>();
                    ADOFAI adofai = JsonManager.JsonRead<ADOFAI>(mapFilePath, true);
                    if (adofai == null)
                        return null;

                    #region Default Effect
                    {
                        sdjk.info.artist = adofai.settings.artist;
                        sdjk.info.songName = adofai.settings.song;
                        sdjk.info.author = adofai.settings.author;

                        sdjk.info.mainMenuStartTime = adofai.settings.previewSongStart;

                        {
                            int difficulty = adofai.settings.difficulty;
                            if (difficulty >= 1 && difficulty <= 3)
                                sdjk.info.difficultyLabel = "Easy (ADOFAI)";
                            else if (difficulty >= 4 && difficulty <= 6)
                                sdjk.info.difficultyLabel = "Normal (ADOFAI)";
                            else if (difficulty >= 7 && difficulty <= 9)
                                sdjk.info.difficultyLabel = "Hard (ADOFAI)";
                            if (difficulty >= 10)
                                sdjk.info.difficultyLabel = "Very Hard (ADOFAI)";
                            else
                                sdjk.info.difficultyLabel = "ADOFAI";
                        }

                        sdjk.globalEffect.background.Add(new BeatValuePair<BackgroundEffect>(double.MinValue, new BackgroundEffect(Path.GetFileNameWithoutExtension(adofai.settings.bgImage), ""), false));

                        if (ColorUtility.TryParseHtmlString("#" + adofai.settings.bgImageColor, out Color color))
                            sdjk.globalEffect.backgroundColor.Add(new BeatValuePairAni<JColor>(double.MinValue, color, 0, EasingFunction.Ease.Linear, false));
                        else
                            sdjk.globalEffect.backgroundColor.Add(new BeatValuePairAni<JColor>(double.MinValue, JColor.one, 0, EasingFunction.Ease.Linear, false));

                        sdjk.info.videoBackgroundFile = Path.GetFileNameWithoutExtension(adofai.settings.bgVideo);
                        sdjk.info.videoOffset = (adofai.settings.vidOffset - adofai.settings.offset) * 0.001f;

                        sdjk.info.songFile = Path.GetFileNameWithoutExtension(adofai.settings.songFilename);

                        sdjk.globalEffect.bpm.Add(new SCKRM.Rhythm.BeatValuePair<double>(double.MinValue, adofai.settings.bpm));
                        sdjk.globalEffect.volume.Add(new BeatValuePairAni<double>(double.MinValue, adofai.settings.volume * 0.02, 0, EasingFunction.Ease.Linear, false));
                        sdjk.info.songOffset = adofai.settings.offset * 0.001f;
                        sdjk.globalEffect.pitch.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(double.MinValue, adofai.settings.pitch * 0.01, 0, EasingFunction.Ease.Linear));
                        sdjk.globalEffect.tempo.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(double.MinValue, 1, 0, EasingFunction.Ease.Linear));

                        sdjk.globalEffect.cameraPos.Add(new BeatValuePairAni<JVector3>(double.MinValue, new Vector3(adofai.settings.position[0], adofai.settings.position[1], -14), 0, EasingFunction.Ease.Linear, false));
                        sdjk.globalEffect.cameraRotation.Add(new BeatValuePairAni<JVector3>(double.MinValue, new Vector3(0, 0, adofai.settings.rotation), 0, EasingFunction.Ease.Linear, false));
                        sdjk.globalEffect.cameraZoom.Add(new BeatValuePairAni<double>(double.MinValue, adofai.settings.zoom * 0.01, 0, EasingFunction.Ease.Linear, false));
                    }
                    #endregion

                    #region Tile Effect
                    Dictionary<int, bool> twirlList = new Dictionary<int, bool>();
                    Dictionary<int, int> holdList = new Dictionary<int, int>();
                    Dictionary<int, double> pauseList = new Dictionary<int, double>();
                    {
                        bool twirl = false;
                        for (int i = 0; i < adofai.actions.Length; i++)
                        {
                            JObject action = adofai.actions[i];
                            int index = action["floor"].Value<int>();
                            string eventType = action["eventType"].Value<string>();

                            if (eventType == "Twirl")
                            {
                                twirl = !twirl;
                                twirlList.Add(index, twirl);
                            }
                            else if (eventType == "Pause")
                                pauseList.Add(index, action["duration"].Value<int>());
                            else if (eventType == "Hold")
                                holdList.Add(index, action["duration"].Value<int>());
                        }
                    }
                    #endregion

                    #region Beat
                    {
                        if (adofai.pathData.Length > 0)
                        {
                            float previousAngle = 0;
                            List<float> angleData = new List<float>();
                            for (int i = 0; i < adofai.pathData.Length; i++)
                            {
                                char path = adofai.pathData[i];
                                if (path == 'R')
                                    angleData.Add(0);
                                else if (path == 'J')
                                    angleData.Add(30);
                                else if (path == 'E')
                                    angleData.Add(45);
                                else if (path == 'T')
                                    angleData.Add(60);
                                else if (path == 'U')
                                    angleData.Add(90);
                                else if (path == 'G')
                                    angleData.Add(120);
                                else if (path == 'Q')
                                    angleData.Add(135);
                                else if (path == 'H')
                                    angleData.Add(150);
                                else if (path == 'L')
                                    angleData.Add(180);
                                else if (path == 'N')
                                    angleData.Add(210);
                                else if (path == 'Z')
                                    angleData.Add(225);
                                else if (path == 'F')
                                    angleData.Add(240);
                                else if (path == 'D')
                                    angleData.Add(270);
                                else if (path == 'B')
                                    angleData.Add(300);
                                else if (path == 'C')
                                    angleData.Add(315);
                                else if (path == 'M')
                                    angleData.Add(330);
                                else if (path == 'p')
                                    angleData.Add(15);
                                else if (path == 'o')
                                    angleData.Add(75);
                                else if (path == 'q')
                                    angleData.Add(105);
                                else if (path == 'W')
                                    angleData.Add(165);
                                else if (path == 'x')
                                    angleData.Add(195);
                                else if (path == 'V')
                                    angleData.Add(255);
                                else if (path == 'Y')
                                    angleData.Add(285);
                                else if (path == 'A')
                                    angleData.Add(345);
                                else if (path == '5')
                                    angleData.Add(previousAngle + 72);
                                else if (path == '7')
                                    angleData.Add(previousAngle + 52);
                                else if (path == '!')
                                    angleData.Add(999);
                                else
                                    throw new NotSupportedException($"Unsupported patch data character '{path}'");

                                previousAngle = angleData[i];
                            }

                            adofai.angleData = angleData.ToArray();
                        }

                        bool twirl = false;
                        double lastBeat = -1;
                        double lastAngle = 0;
                        for (int i = 0; i < adofai.angleData.Length; i++)
                        {
                            if (twirlList.TryGetValue(i, out bool outTwirl))
                                twirl = outTwirl;

                            double pause = 0;
                            if (pauseList.TryGetValue(i, out double outpause))
                                pause = outpause;

                            int hold = 0;
                            if (holdList.TryGetValue(i, out int outHold))
                                hold = outHold;

                            double beat = lastBeat;
                            double angle = adofai.angleData[i];
                            bool midspin = false;

                            if (angle > 360)
                            {
                                midspin = true;

                                angle = (lastAngle + 180).Reapeat(360);
                                if (angle >= 360)
                                    angle = 0;
                            }

                            double offsetBeat;
                            if (!twirl)
                                offsetBeat = (1 + ((lastAngle - angle.Abs()) / 180)).Reapeat(2);
                            else
                                offsetBeat = (1 + ((angle.Abs() - lastAngle) / 180)).Reapeat(2);

                            beat += (offsetBeat + pause) + (hold * 2);
                            if (lastBeat == beat && !midspin)
                                beat += 2;

                            sdjk.allBeat.Add(beat);
                            allBeat.Add(beat);

                            lastAngle = angle;
                            lastBeat = beat;
                        }
                    }
                    #endregion

                    #region Actions
                    float lastBpm = adofai.settings.bpm;
                    for (int i = 0; i < adofai.actions.Length; i++)
                    {
                        JObject action = adofai.actions[i];
                        int index = action["floor"].Value<int>() - 1;
                        string eventType = action["eventType"].Value<string>();
                        double beat = allBeat[index.Clamp(0, allBeat.Count - 1)];
                        float duration = 0;
                        float angleOffset = 0;
                        EasingFunction.Ease ease = EasingFunction.Ease.Linear;

                        #region Effect
                        if (action.ContainsKey("duration"))
                            duration = action["duration"].Value<float>();
                        if (action.ContainsKey("angleOffset"))
                            angleOffset = action["angleOffset"].Value<float>();
                        if (action.ContainsKey("ease"))
                        {
                            string easeString = action["ease"].Value<string>();

                            if (easeString == "Linear")
                                ease = EasingFunction.Ease.Linear;
                            else if (easeString == "InBack")
                                ease = EasingFunction.Ease.EaseInBack;
                            else if (easeString == "InBounce")
                                ease = EasingFunction.Ease.EaseInBounce;
                            else if (easeString == "InCirc")
                                ease = EasingFunction.Ease.EaseInCirc;
                            else if (easeString == "InCubic")
                                ease = EasingFunction.Ease.EaseInCubic;
                            else if (easeString == "InElastic")
                                ease = EasingFunction.Ease.EaseInElastic;
                            else if (easeString == "InExpo")
                                ease = EasingFunction.Ease.EaseInExpo;
                            else if (easeString == "InOutBack")
                                ease = EasingFunction.Ease.EaseInOutBack;
                            else if (easeString == "InOutBounce")
                                ease = EasingFunction.Ease.EaseInOutBounce;
                            else if (easeString == "InOutCirc")
                                ease = EasingFunction.Ease.EaseInOutCirc;
                            else if (easeString == "InOutCubic")
                                ease = EasingFunction.Ease.EaseInOutCubic;
                            else if (easeString == "InOutElastic")
                                ease = EasingFunction.Ease.EaseInOutElastic;
                            else if (easeString == "InOutExpo")
                                ease = EasingFunction.Ease.EaseInOutExpo;
                            else if (easeString == "InOutQuad")
                                ease = EasingFunction.Ease.EaseInOutQuad;
                            else if (easeString == "InOutQuart")
                                ease = EasingFunction.Ease.EaseInOutQuart;
                            else if (easeString == "InOutQuint")
                                ease = EasingFunction.Ease.EaseInOutQuint;
                            else if (easeString == "InOutSine")
                                ease = EasingFunction.Ease.EaseInOutSine;
                            else if (easeString == "InQuad")
                                ease = EasingFunction.Ease.EaseInQuad;
                            else if (easeString == "InQuart")
                                ease = EasingFunction.Ease.EaseInQuart;
                            else if (easeString == "InQuart")
                                ease = EasingFunction.Ease.EaseInQuart;
                            else if (easeString == "InQuint")
                                ease = EasingFunction.Ease.EaseInQuint;
                            else if (easeString == "InSine")
                                ease = EasingFunction.Ease.EaseInSine;
                            else if (easeString == "OutBack")
                                ease = EasingFunction.Ease.EaseOutBack;
                            else if (easeString == "OutBounce")
                                ease = EasingFunction.Ease.EaseOutBounce;
                            else if (easeString == "OutCirc")
                                ease = EasingFunction.Ease.EaseOutCirc;
                            else if (easeString == "OutCubic")
                                ease = EasingFunction.Ease.EaseOutCubic;
                            else if (easeString == "OutElastic")
                                ease = EasingFunction.Ease.EaseOutElastic;
                            else if (easeString == "OutExpo")
                                ease = EasingFunction.Ease.EaseOutExpo;
                            else if (easeString == "OutQuad")
                                ease = EasingFunction.Ease.EaseOutQuad;
                            else if (easeString == "OutQuart")
                                ease = EasingFunction.Ease.EaseOutQuart;
                            else if (easeString == "OutQuint")
                                ease = EasingFunction.Ease.EaseOutQuint;
                            else if (easeString == "OutSine")
                                ease = EasingFunction.Ease.EaseOutSine;
                        }
                        #endregion

                        if (eventType == "SetSpeed")
                        {
                            float bpm;
                            if (action["speedType"] != null)
                            {
                                if (action["speedType"].Value<string>() == "Bpm")
                                {
                                    bpm = action["beatsPerMinute"].Value<float>();
                                    sdjk.globalEffect.bpm.Add(new SCKRM.Rhythm.BeatValuePair<double>(beat, bpm));
                                }
                                else
                                {
                                    bpm = lastBpm * action["bpmMultiplier"].Value<float>();
                                    sdjk.globalEffect.bpm.Add(new SCKRM.Rhythm.BeatValuePair<double>(beat, bpm));
                                }
                            }
                            else
                            {
                                bpm = action["beatsPerMinute"].Value<float>();
                                sdjk.globalEffect.bpm.Add(new SCKRM.Rhythm.BeatValuePair<double>(beat, bpm));
                            }

                            lastBpm = bpm;
                        }
                        else if (eventType == "MoveCamera")
                        {
                            if (action.ContainsKey("position"))
                            {
                                float[] pos = action["position"].Values<float>().ToArray();
                                sdjk.globalEffect.cameraPos.Add(new BeatValuePairAni<JVector3>(beat, new JVector3(pos[0], pos[1], -14), duration, ease, false));
                            }

                            if (action.ContainsKey("rotation"))
                            {
                                float rotation = action["rotation"].Value<float>();
                                sdjk.globalEffect.cameraRotation.Add(new BeatValuePairAni<JVector3>(beat, new JVector3(0, 0, rotation), duration, ease, false));
                            }

                            if (action.ContainsKey("zoom"))
                            {
                                double zoom = action["zoom"].Value<float>() * 0.01;
                                sdjk.globalEffect.cameraZoom.Add(new BeatValuePairAni<double>(beat, zoom, duration, ease, false));
                            }
                        }
                        else if (eventType == "CustomBackground")
                        {
                            if (action.ContainsKey("imageColor"))
                            {
                                if (ColorUtility.TryParseHtmlString("#" + action["imageColor"], out Color color))
                                    sdjk.globalEffect.backgroundColor.Add(new BeatValuePairAni<JColor>(beat, color, 0, EasingFunction.Ease.Linear, false));
                                else
                                    sdjk.globalEffect.backgroundColor.Add(new BeatValuePairAni<JColor>(beat, JColor.one, 0, EasingFunction.Ease.Linear, false));
                            }

                            if (action.ContainsKey("bgImage"))
                            {
                                string background = Path.GetFileNameWithoutExtension(action["bgImage"].Value<string>());
                                sdjk.globalEffect.background.Add(new BeatValuePair<BackgroundEffect>(beat, new BackgroundEffect(background, ""), false));
                            }
                        }
                            }
                        }
                    }
                    #endregion

                    return sdjk;
                }
                catch (Exception e)
                {
                    Debug.Log(mapFilePath);
                    Debug.LogException(e);
                    return null;
                }
            }
            else if (extension == ".sdjk")
            {
                {
                    T map = JsonManager.JsonRead<T>(mapFilePath, true);
                    if (map == null)
                        return null;
                    else if (map.info.sdjkVersion != new Version())
                        return map;
                }

                try
                {
                    if (typeof(T) == typeof(Map.Map) || typeof(T) == typeof(SDJKMapFile))
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
                        map.info.sdjkVersion = new Version(Application.version);



                        map.info.mode = "sdjk";



                        map.info.songFile = oldMap.BGM;



                        map.globalEffect.background.Add(new BeatValuePair<BackgroundEffect>(double.MinValue, new BackgroundEffect(oldMap.Background, oldMap.BackgroundNight), false));
                        map.globalEffect.backgroundColor.Add(new BeatValuePairAni<JColor>(double.MinValue, JColor.one, 0, EasingFunction.Ease.Linear, false));

                        map.info.videoBackgroundFile = oldMap.VideoBackground;
                        map.info.videoBackgroundNightFile = oldMap.VideoBackgroundNight;

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
                        {
                            List<Note> notes = new List<Note>();
                            for (int i = 0; i < oldMap.CapsLock.Count; i++)
                            {
                                if (oldMap.HoldCapsLock.Count != oldMap.CapsLock.Count)
                                    notes.Add(new Note(oldMap.CapsLock[i] - 1, 0));
                                else
                                    notes.Add(new Note(oldMap.CapsLock[i] - 1, oldMap.HoldCapsLock[i] - 1));
                            }

                            map.notes.Add(notes);
                        }

                        {
                            List<Note> notes = new List<Note>();
                            for (int i = 0; i < oldMap.A.Count; i++)
                            {
                                if (oldMap.HoldA.Count != oldMap.A.Count)
                                    notes.Add(new Note(oldMap.A[i] - 1, 0));
                                else
                                    notes.Add(new Note(oldMap.A[i] - 1, oldMap.HoldA[i] - 1));
                            }

                            map.notes.Add(notes);
                        }

                        {
                            List<Note> notes = new List<Note>();
                            for (int i = 0; i < oldMap.S.Count; i++)
                            {
                                if (oldMap.HoldS.Count != oldMap.S.Count)
                                    notes.Add(new Note(oldMap.S[i] - 1, 0));
                                else
                                    notes.Add(new Note(oldMap.S[i] - 1, oldMap.HoldS[i] - 1));
                            }

                            map.notes.Add(notes);
                        }

                        {
                            List<Note> notes = new List<Note>();
                            for (int i = 0; i < oldMap.D.Count; i++)
                            {
                                if (oldMap.HoldD.Count != oldMap.D.Count)
                                    notes.Add(new Note(oldMap.D[i] - 1, 0));
                                else
                                    notes.Add(new Note(oldMap.D[i] - 1, oldMap.HoldD[i] - 1));
                            }

                            map.notes.Add(notes);
                        }

                        {
                            List<Note> notes = new List<Note>();
                            for (int i = 0; i < oldMap.J.Count; i++)
                            {
                                if (oldMap.HoldJ.Count != oldMap.J.Count)
                                    notes.Add(new Note(oldMap.J[i] - 1, 0));
                                else
                                    notes.Add(new Note(oldMap.J[i] - 1, oldMap.HoldJ[i] - 1));
                            }

                            map.notes.Add(notes);
                        }

                        {
                            List<Note> notes = new List<Note>();
                            for (int i = 0; i < oldMap.K.Count; i++)
                            {
                                if (oldMap.HoldK.Count != oldMap.K.Count)
                                    notes.Add(new Note(oldMap.K[i] - 1, 0));
                                else
                                    notes.Add(new Note(oldMap.K[i] - 1, oldMap.HoldK[i] - 1));
                            }

                            map.notes.Add(notes);
                        }

                        {
                            List<Note> notes = new List<Note>();
                            for (int i = 0; i < oldMap.L.Count; i++)
                            {
                                if (oldMap.HoldL.Count != oldMap.L.Count)
                                    notes.Add(new Note(oldMap.L[i] - 1, 0));
                                else
                                    notes.Add(new Note(oldMap.L[i] - 1, oldMap.HoldL[i] - 1));
                            }

                            map.notes.Add(notes);
                        }

                        {
                            List<Note> notes = new List<Note>();
                            for (int i = 0; i < oldMap.Semicolon.Count; i++)
                            {
                                if (oldMap.HoldSemicolon.Count != oldMap.Semicolon.Count)
                                    notes.Add(new Note(oldMap.Semicolon[i] - 1, 0));
                                else
                                    notes.Add(new Note(oldMap.Semicolon[i] - 1, oldMap.HoldSemicolon[i] - 1));
                            }

                            map.notes.Add(notes);
                        }
                        #endregion

                        #region Effect
                        map.globalEffect.bpm.Add(new SCKRM.Rhythm.BeatValuePair<double>(double.MinValue, oldMap.Effect.BPM));
                        for (int i = 0; i < oldMap.Effect.BPMEffect.Count; i++)
                        {
                            var effect = oldMap.Effect.BPMEffect[i];
                            map.globalEffect.bpm.Add(new SCKRM.Rhythm.BeatValuePair<double>(effect.Beat - 1, effect.Value));
                        }

                        map.globalEffect.dropPart.Add(new SCKRM.Rhythm.BeatValuePair<bool>(double.MinValue, oldMap.Effect.DropPart));
                        for (int i = 0; i < oldMap.Effect.DropPartEffect.Count; i++)
                        {
                            var effect = oldMap.Effect.DropPartEffect[i];
                            map.globalEffect.dropPart.Add(new SCKRM.Rhythm.BeatValuePair<bool>(effect.Beat - 1, effect.Value));
                        }

                        map.globalEffect.cameraZoom.Add(new BeatValuePairAni<double>(double.MinValue, oldMap.Effect.Camera.CameraZoom, 0, EasingFunction.Ease.Linear, false));
                        map.globalEffect.cameraPos.Add(new BeatValuePairAni<JVector3>(double.MinValue, oldMap.Effect.Camera.CameraPos, 0, EasingFunction.Ease.Linear, false));
                        map.globalEffect.cameraRotation.Add(new BeatValuePairAni<JVector3>(double.MinValue, oldMap.Effect.Camera.CameraRotation, 0, EasingFunction.Ease.Linear, false));

                        map.globalEffect.pitch.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(double.MinValue, oldMap.Effect.Pitch, 0, EasingFunction.Ease.Linear));
                        map.globalEffect.tempo.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(double.MinValue, 1, 0, EasingFunction.Ease.Linear));

                        map.globalEffect.volume.Add(new BeatValuePairAni<double>(double.MinValue, oldMap.Effect.Volume * 2, 0, EasingFunction.Ease.Linear, false));

                        map.globalEffect.hpAddValue.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(double.MinValue, oldMap.Effect.HPAddValue, 0, EasingFunction.Ease.Linear));
                        for (int i = 0; i < oldMap.Effect.HPAddValueEffect.Count; i++)
                        {
                            var effect = oldMap.Effect.HPAddValueEffect[i];
                            map.globalEffect.hpAddValue.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(effect.Beat - 1, effect.Value, 0, EasingFunction.Ease.Linear));
                        }

                        map.globalEffect.hpMissValue.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(double.MinValue, oldMap.Effect.HPRemoveValue, 0, EasingFunction.Ease.Linear));
                        for (int i = 0; i < oldMap.Effect.HPRemoveValueEffect.Count; i++)
                        {
                            var effect = oldMap.Effect.HPRemoveValueEffect[i];
                            map.globalEffect.hpMissValue.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(effect.Beat - 1, effect.Value, 0, EasingFunction.Ease.Linear));
                        }

                        {
                            var effect = oldMap.Effect.HPRemove;
                            if (effect)
                                map.globalEffect.hpRemoveValue.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(double.MinValue, oldMap.Effect.HPRemoveValue, 0, EasingFunction.Ease.Linear));
                            else
                                map.globalEffect.hpRemoveValue.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(double.MinValue, 0, 0, EasingFunction.Ease.Linear));
                        }

                        for (int i = 0; i < oldMap.Effect.HPRemoveEffect.Count; i++)
                        {
                            var effect = oldMap.Effect.HPRemoveEffect[i];
                            if (effect.Value)
                                map.globalEffect.hpRemoveValue.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(effect.Beat - 1, oldMap.Effect.HPRemoveValue, 0, EasingFunction.Ease.Linear));
                            else
                                map.globalEffect.hpRemoveValue.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(effect.Beat - 1, 0, 0, EasingFunction.Ease.Linear));
                        }

                        map.globalEffect.judgmentSize.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(double.MinValue, oldMap.Effect.JudgmentSize, 0, EasingFunction.Ease.Linear));
                        for (int i = 0; i < oldMap.Effect.JudgmentSizeEffect.Count; i++)
                        {
                            var effect = oldMap.Effect.JudgmentSizeEffect[i];
                            map.globalEffect.judgmentSize.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(effect.Beat - 1, effect.Value, 0, EasingFunction.Ease.Linear));
                        }
                        #endregion

                        return map as T;
                    }

                    return null;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return null;
                }
            }
            else
                return null;
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

        class ADOFAI
        {
            public string pathData = "";
            public float[] angleData = new float[0];

            public Settings settings = new Settings();

            public JObject[] actions = new JObject[0];

            public class Settings
            {
                public string artist = "";
                public string song = "";

                public string author = "";

                public string songFilename = "";

                public int previewSongStart = 0;

                public int difficulty = 0;

                public float bpm = 100;
                public int volume = 100;
                public int offset = 0;
                public int pitch = 100;

                public string bgImage = "";
                public string bgImageColor = "";

                public string bgVideo = "";
                public int vidOffset = 0;

                public float[] position = new float[] { 0, 0 };
                public float rotation = 0;
                public float zoom = 100;
            }
        }
    }
}
