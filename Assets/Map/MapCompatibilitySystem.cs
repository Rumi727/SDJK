using Newtonsoft.Json.Linq;
using SCKRM;
using SCKRM.Easing;
using SCKRM.Json;
using SDJK.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SDJK
{
    internal static class MapCompatibilitySystem
    {
        public static string[] compatibleMapExtensions { get; } = new string[] { "sdjk", "adofai" };

        public static Map.Map GlobalMapCompatibility(string mapFilePath)
        {
            string extension = Path.GetExtension(mapFilePath);
            if (extension == ".adofai")
            {
                try
                {
                    Map.Map sdjk = new Map.Map();
                    ADOFAI adofai = JsonManager.JsonRead<ADOFAI>(mapFilePath, true);
                    List<double> allBeat = new List<double>();
                    
                    #region Default Effect
                    {
                        sdjk.info.artist = Path.GetFileNameWithoutExtension(adofai.settings.artist);
                        sdjk.info.songName = Path.GetFileNameWithoutExtension(adofai.settings.song);
                        sdjk.info.author = Path.GetFileNameWithoutExtension(adofai.settings.author);

                        sdjk.info.mainMenuStartTime = adofai.settings.previewSongStart;

                        sdjk.info.backgroundFile = Path.GetFileNameWithoutExtension(adofai.settings.bgImage);
                        sdjk.info.videoBackgroundFile = adofai.settings.bgVideo;

                        sdjk.info.videoOffset = adofai.settings.vidOffest * 0.001f;

                        sdjk.info.songFile = Path.GetFileNameWithoutExtension(adofai.settings.songFilename);

                        sdjk.globalEffect.bpm.Add(new SCKRM.Rhythm.BeatValuePair<double>(0, adofai.settings.bpm));
                        sdjk.globalEffect.volume.Add(new BeatValuePairAni<double>(double.MinValue, adofai.settings.volume * 0.01, 0, EasingFunction.Ease.Linear, false));
                        sdjk.info.songOffset = adofai.settings.offset * 0.001f;
                        sdjk.globalEffect.pitch.Add(new SCKRM.Rhythm.BeatValuePairAni<double>(double.MinValue, adofai.settings.pitch * 0.01, 0, EasingFunction.Ease.Linear));

                        sdjk.globalEffect.cameraPos.Add(new BeatValuePairAni<JVector3>(double.MinValue, new Vector3(adofai.settings.position[0], adofai.settings.position[1], -14), 0, EasingFunction.Ease.Linear, false));
                        sdjk.globalEffect.cameraRotation.Add(new BeatValuePairAni<JVector3>(double.MinValue, new Vector3(0, 0, adofai.settings.rotation), 0, EasingFunction.Ease.Linear, false));
                        sdjk.globalEffect.cameraZoom.Add(new BeatValuePairAni<double>(double.MinValue, adofai.settings.zoom * 0.01, 0, EasingFunction.Ease.Linear, false));
                        ;
                    }
                    #endregion

                    #region Twirl
                    Dictionary<int, bool> twirlList = new Dictionary<int, bool>();
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
                        }
                    }
                    #endregion

                    #region Beat
                    if (adofai.angleData.Length != 0)
                    {
                        bool twirl = false;
                        double lastBeat = -1;
                        double lastAngle = 0;
                        for (int i = 0; i < adofai.angleData.Length; i++)
                        {
                            if (twirlList.TryGetValue(i, out bool outTwirl))
                                twirl = outTwirl;

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

                            if (!twirl)
                            {
                                beat += (1 + ((lastAngle - angle.Abs()) / 180)).Reapeat(2);
                                if (lastBeat == beat && !midspin)
                                    beat += 2;

                                allBeat.Add(beat);
                            }
                            else
                            {
                                beat += (1 + ((angle.Abs() - lastAngle) / 180)).Reapeat(2);
                                if (lastBeat == beat && !midspin)
                                    beat += 2;

                                allBeat.Add(beat);
                            }

                            lastAngle = angle;
                            lastBeat = beat;
                        }
                    }
                    else
                        throw new NotSupportedException();
                    #endregion

                    #region Actions
                    float lastBpm = adofai.settings.bpm;
                    for (int i = 0; i < adofai.actions.Length; i++)
                    {
                        JObject action = adofai.actions[i];
                        int index = action["floor"].Value<int>() - 1;
                        string eventType = action["eventType"].Value<string>();
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
                            if (action["speedType"].Value<string>() == "Bpm")
                            {
                                bpm = action["beatsPerMinute"].Value<float>();
                                sdjk.globalEffect.bpm.Add(new SCKRM.Rhythm.BeatValuePair<double>(allBeat[index], bpm));
                            }
                            else
                            {
                                bpm = lastBpm * action["bpmMultiplier"].Value<float>();
                                sdjk.globalEffect.bpm.Add(new SCKRM.Rhythm.BeatValuePair<double>(allBeat[index], bpm));
                            }

                            lastBpm = bpm;
                        }
                        else if (eventType == "MoveCamera")
                        {
                            if (action.ContainsKey("position"))
                            {
                                float[] pos = action["position"].Values<float>().ToArray();
                                sdjk.globalEffect.cameraPos.Add(new BeatValuePairAni<JVector3>(allBeat[index], new JVector3(pos[0], pos[1], -14), duration, ease, false));
                            }

                            if (action.ContainsKey("rotation"))
                            {
                                float rotation = action["rotation"].Value<float>();
                                sdjk.globalEffect.cameraRotation.Add(new BeatValuePairAni<JVector3>(allBeat[index], new JVector3(0, 0, rotation), duration, ease, false));
                            }

                            if (action.ContainsKey("zoom"))
                            {
                                double zoom = action["zoom"].Value<float>() * 0.01;
                                sdjk.globalEffect.cameraZoom.Add(new BeatValuePairAni<double>(allBeat[index], zoom, duration, ease, false));
                            }
                        }
                    }
                    #endregion

                    return sdjk;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return null;
                }
            }
            else if (extension == ".sdjk")
                return JsonManager.JsonRead<Map.Map>(mapFilePath, true);
            else
                return null;
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

                public float bpm = 100;
                public int volume = 100;
                public int offset = 0;
                public int pitch = 100;

                public string bgImage = "";

                public string bgVideo = "";
                public int vidOffest = 0;

                public float[] position = new float[] { 0, 0 };
                public float rotation = 0;
                public float zoom = 100;
            }
        }
    }
}
