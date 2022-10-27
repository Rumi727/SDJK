using Newtonsoft.Json.Linq;
using SCKRM;
using SCKRM.Easing;
using SCKRM.Json;
using SDJK.Map;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SDJK.Ruleset.ADOFAI
{
    static class ADOFAIMapCompatibilitySystem
    {
        [Awaken]
        static void Awaken()
        {
            MapLoader.extensionToLoad.Add("adofai");
            MapLoader.mapLoaderFunc += (Type type, string mapFilePath, string extension) =>
            {
                if (extension == ".adofai" && (type == typeof(Map.Map) || type == typeof(ADOFAIMapFile)))
                {
                    try
                    {
                        ADOFAIMapFile sdjk = new ADOFAIMapFile();
                        List<double> allBeat = new List<double>();
                        ADOFAI adofai = JsonManager.JsonRead<ADOFAI>(mapFilePath, true);
                        if (adofai == null)
                            return null;

                        sdjk.info.mode = typeof(ADOFAIRuleset).FullName;

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
                                else if (difficulty >= 10)
                                    sdjk.info.difficultyLabel = "Insane (ADOFAI)";
                                else
                                    sdjk.info.difficultyLabel = "ADOFAI";
                            }

                            sdjk.globalEffect.background.Add(new BeatValuePair<BackgroundEffectPair>(double.MinValue, new BackgroundEffectPair(Path.GetFileNameWithoutExtension(adofai.settings.bgImage), ""), false));

                            if (ColorUtility.TryParseHtmlString("#" + adofai.settings.bgImageColor, out Color color))
                                sdjk.globalEffect.backgroundColor.Add(new BeatValuePairAni<JColor>(double.MinValue, color, 0, EasingFunction.Ease.Linear, false));
                            else
                                sdjk.globalEffect.backgroundColor.Add(new BeatValuePairAni<JColor>(double.MinValue, JColor.one, 0, EasingFunction.Ease.Linear, false));

                            sdjk.info.videoBackgroundFile = Path.GetFileNameWithoutExtension(adofai.settings.bgVideo);
                            sdjk.globalEffect.videoColor.Add(new BeatValuePairAni<JColor>(double.MinValue, JColor.one, 0, EasingFunction.Ease.Linear, false));
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

                            sdjk.globalEffect.backgroundFlash.Add(new BeatValuePairAni<JColor>(double.MinValue, JColor.zero, 0, EasingFunction.Ease.Linear, false));
                            sdjk.globalEffect.fieldFlash.Add(new BeatValuePairAni<JColor>(double.MinValue, JColor.zero, 0, EasingFunction.Ease.Linear, false));
                            sdjk.globalEffect.uiFlash.Add(new BeatValuePairAni<JColor>(double.MinValue, JColor.zero, 0, EasingFunction.Ease.Linear, false));
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

                                if (angle == 999)
                                {
                                    midspin = true;

                                    angle = (lastAngle + 180).Reapeat(360);
                                    if (angle >= 360)
                                        angle = 0;
                                }
                                else
                                    angle = angle.Reapeat(360);

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
                        ADOFAI.RepeatEvents repeatEvents = new ADOFAI.RepeatEvents();
                        int tempIndex = 0;
                        for (int i = 0; i < adofai.actions.Length; i++)
                        {
                            JObject action = adofai.actions[i];
                            int index = action["floor"].Value<int>() - 1;
                            string eventType = action["eventType"].Value<string>();
                            double beat = allBeat[index.Clamp(0, allBeat.Count - 1)];
                            float duration = 0;
                            float angleOffset = 0;
                            string eventTag = "";
                            EasingFunction.Ease ease = EasingFunction.Ease.Linear;

                            if (tempIndex != index)
                            {
                                repeatEvents.Invoke();
                                repeatEvents = new ADOFAI.RepeatEvents();

                                tempIndex = index;
                            }

                            #region Default Effect
                            if (action.ContainsKey("duration"))
                                duration = action["duration"].Value<float>();
                            if (action.ContainsKey("angleOffset"))
                            {
                                angleOffset = action["angleOffset"].Value<float>();
                                beat += angleOffset.Reapeat(360) / 180;
                            }
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
                            if (action.ContainsKey("eventTag"))
                                eventTag = action["eventTag"].Value<string>();
                            #endregion

                            ADOFAI.Effect effect = new ADOFAI.Effect(beat, eventTag, null, false);

                            #region Effect
                            if (eventType == "RepeatEvents")
                            {
                                repeatEvents.repetitions = action["repetitions"].Value<int>();
                                repeatEvents.interval = action["interval"].Value<float>();
                                repeatEvents.tag = action["tag"].Value<string>();
                            }
                            else if (eventType == "SetSpeed")
                            {
                                float bpm;
                                if (action["speedType"] != null)
                                {
                                    if (action["speedType"].Value<string>() == "Bpm")
                                    {
                                        bpm = action["beatsPerMinute"].Value<float>();
                                        effect.action += (double beat) => sdjk.globalEffect.bpm.Add(new SCKRM.Rhythm.BeatValuePair<double>(beat, bpm));
                                    }
                                    else
                                    {
                                        bpm = lastBpm * action["bpmMultiplier"].Value<float>();
                                        effect.action += (double beat) => sdjk.globalEffect.bpm.Add(new SCKRM.Rhythm.BeatValuePair<double>(beat, bpm));
                                    }
                                }
                                else
                                {
                                    bpm = action["beatsPerMinute"].Value<float>();
                                    effect.action += (double beat) => sdjk.globalEffect.bpm.Add(new SCKRM.Rhythm.BeatValuePair<double>(beat, bpm));
                                }

                                effect.isTileEffect = true;
                                lastBpm = bpm;
                            }
                            else if (eventType == "MoveCamera")
                            {
                                if (action.ContainsKey("position"))
                                {
                                    float[] pos = action["position"].Values<float>().ToArray();
                                    effect.action += (double beat) => sdjk.globalEffect.cameraPos.Add(new BeatValuePairAni<JVector3>(beat, new JVector3(pos[0], pos[1], -14), duration, ease, false));
                                }

                                if (action.ContainsKey("rotation"))
                                {
                                    float rotation = action["rotation"].Value<float>();
                                    effect.action += (double beat) => sdjk.globalEffect.cameraRotation.Add(new BeatValuePairAni<JVector3>(beat, new JVector3(0, 0, rotation), duration, ease, false));
                                }

                                if (action.ContainsKey("zoom"))
                                {
                                    double zoom = action["zoom"].Value<float>() * 0.01;
                                    effect.action += (double beat) => sdjk.globalEffect.cameraZoom.Add(new BeatValuePairAni<double>(beat, zoom, duration, ease, false));
                                }
                            }
                            else if (eventType == "CustomBackground")
                            {
                                if (action.ContainsKey("imageColor"))
                                {
                                    if (ColorUtility.TryParseHtmlString("#" + action["imageColor"], out Color color))
                                        effect.action += (double beat) => sdjk.globalEffect.backgroundColor.Add(new BeatValuePairAni<JColor>(beat, color, 0, EasingFunction.Ease.Linear, false));
                                    else
                                        effect.action += (double beat) => sdjk.globalEffect.backgroundColor.Add(new BeatValuePairAni<JColor>(beat, JColor.one, 0, EasingFunction.Ease.Linear, false));
                                }

                                if (action.ContainsKey("bgImage"))
                                {
                                    string background = Path.GetFileNameWithoutExtension(action["bgImage"].Value<string>());
                                    effect.action += (double beat) => sdjk.globalEffect.background.Add(new BeatValuePair<BackgroundEffectPair>(beat, new BackgroundEffectPair(background, ""), false));
                                }
                            }
                            else if (eventType == "Flash")
                            {
                                if (action.ContainsKey("startColor") && action.ContainsKey("endColor") && action.ContainsKey("startOpacity") && action.ContainsKey("endOpacity") && action.ContainsKey("plane"))
                                {
                                    if (ColorUtility.TryParseHtmlString("#" + action["startColor"], out Color startColor) && ColorUtility.TryParseHtmlString("#" + action["endColor"], out Color endColor))
                                    {
                                        string plane = action.Value<string>("plane");
                                        startColor.a *= action.Value<float>("startOpacity") * 0.01f;
                                        endColor.a *= action.Value<float>("endOpacity") * 0.01f;

                                        effect.action += (double beat) =>
                                        {
                                            BeatValuePairAni<JColor> start = new BeatValuePairAni<JColor>(beat, startColor, 0, EasingFunction.Ease.Linear, false);
                                            BeatValuePairAni<JColor> end = new BeatValuePairAni<JColor>(beat, endColor, duration, EasingFunction.Ease.Linear, false);

                                            if (plane == "Foreground")
                                            {
                                                sdjk.globalEffect.fieldFlash.Add(start);
                                                sdjk.globalEffect.fieldFlash.Add(end);
                                            }
                                            else
                                            {
                                                sdjk.globalEffect.backgroundFlash.Add(start);
                                                sdjk.globalEffect.backgroundFlash.Add(end);
                                            }
                                        };
                                    }
                                }
                            }
                            #endregion

                            repeatEvents.events.Add(effect);
                        }

                        repeatEvents.Invoke();
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
                else
                    return null;
            };
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

            public class RepeatEvents
            {
                public int repetitions = 0;
                public float interval = 0;
                public string tag = "";

                public readonly List<Effect> events = new List<Effect>();

                public void Invoke()
                {
                    for (int i = 0; i < events.Count; i++)
                    {
                        Effect adofaiEvent = events[i];
                        adofaiEvent.Invoke(0);
                    }

                    for (int i = 0; i < repetitions; i++)
                    {
                        for (int j = 0; j < events.Count; j++)
                        {
                            Effect adofaiEvent = events[j];

                            if (!adofaiEvent.isTileEffect && tag == adofaiEvent.eventTag)
                                adofaiEvent.Invoke(interval * (i + 1));
                        }
                    }
                }
            }

            public struct Effect
            {
                public double beat;
                public string eventTag;

                public EventsAction action;

                public bool isTileEffect;

                public Effect(double beat, string eventTag, EventsAction events, bool isTileEffect)
                {
                    this.beat = beat;
                    this.eventTag = eventTag;

                    this.action = events;

                    this.isTileEffect = isTileEffect;
                }

                public void Invoke(double offset) => action?.Invoke(beat + offset);

                public delegate void EventsAction(double beat);
            }
        }
    }
}