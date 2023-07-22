using Newtonsoft.Json.Linq;
using SCKRM;
using SCKRM.Easing;
using SCKRM.Json;
using SCKRM.Rhythm;
using SDJK.Mode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SDJK.Map.Ruleset.ADOFAI
{
    public static class ADOFAIMapLoader
    {
        [Awaken]
        static void Awaken()
        {
            MapLoader.extensionToLoad.Add("adofai");
            MapLoader.mapLoaderFunc += (Type type, string mapFilePath, string extension, bool liteLoader, IMode[] modes) =>
            {
                bool typeIsADOFAIMap = type == typeof(ADOFAIMapFile);
                if (typeIsADOFAIMap && !File.Exists(mapFilePath))
                    return new ADOFAIMapFile("");

                if (extension == ".adofai" && (type == typeof(MapFile) || typeIsADOFAIMap))
                    return MapLoad(mapFilePath);
                else
                    return null;
            };
        }

        public static ADOFAIMapFile MapLoad(string mapFilePath)
        {
            ADOFAIMapFile adofaiMap = new ADOFAIMapFile(mapFilePath);

            if (File.Exists(mapFilePath))
            {
                List<double> allBeat = new List<double>();
                ADOFAI adofai = JsonManager.JsonRead<ADOFAI>(mapFilePath, true);
                if (adofai == null)
                    return null;

                adofaiMap.info.ruleset = "adofai";

                #region Default Effect
                {
                    adofaiMap.info.artist = adofai.settings.artist;
                    adofaiMap.info.songName = adofai.settings.song;
                    adofaiMap.info.author = adofai.settings.author;

                    adofaiMap.info.mainMenuStartTime = adofai.settings.previewSongStart;

                    {
                        int difficulty = adofai.settings.difficulty;
                        if (difficulty >= 1 && difficulty <= 3)
                            adofaiMap.info.difficultyLabel = "Easy";
                        else if (difficulty >= 4 && difficulty <= 6)
                            adofaiMap.info.difficultyLabel = "Normal";
                        else if (difficulty >= 7 && difficulty <= 9)
                            adofaiMap.info.difficultyLabel = "Hard";
                        else if (difficulty >= 10)
                            adofaiMap.info.difficultyLabel = "Insane";
                        else
                            adofaiMap.info.difficultyLabel = "ADOFAI";
                    }

                    adofaiMap.globalEffect.backgroundEffect.background.Add(new BackgroundFileInfoPair(Path.GetFileNameWithoutExtension(adofai.settings.bgImage), ""));

                    if (("#" + adofai.settings.bgImageColor).TryHexToColor(out Color result))
                        adofaiMap.globalEffect.backgroundEffect.backgroundColor.Add(result);
                    else
                        adofaiMap.globalEffect.backgroundEffect.backgroundColor.Add(JColor.one);

                    adofaiMap.info.videoBackgroundFile = Path.GetFileNameWithoutExtension(adofai.settings.bgVideo);
                    adofaiMap.globalEffect.backgroundEffect.videoColor.Add(JColor.one);
                    adofaiMap.info.videoOffset = (adofai.settings.vidOffset - adofai.settings.offset) * 0.001f;

                    adofaiMap.info.songFile = Path.GetFileNameWithoutExtension(adofai.settings.songFilename);

                    adofaiMap.globalEffect.bpm.Add(double.MinValue, adofai.settings.bpm);
                    adofaiMap.globalEffect.volume.Add(double.MinValue, 0, adofai.settings.volume * 0.01);
                    adofaiMap.info.songOffset = adofai.settings.offset * 0.001f;
                    adofaiMap.globalEffect.pitch.Add(double.MinValue, 0, adofai.settings.pitch * 0.01);
                    adofaiMap.globalEffect.tempo.Add(double.MinValue, 0, 1);

                    adofaiMap.globalEffect.cameraPos.Add(new Vector3(adofai.settings.position[0], adofai.settings.position[1], -14));
                    adofaiMap.globalEffect.cameraRotation.Add(new Vector3(0, 0, adofai.settings.rotation));
                    adofaiMap.globalEffect.cameraZoom.Add(double.MinValue, 0, adofai.settings.zoom * 0.01);

                    adofaiMap.globalEffect.backgroundFlash.Add(JColor.zero);
                    adofaiMap.globalEffect.fieldFlash.Add(JColor.zero);
                    adofaiMap.globalEffect.uiFlash.Add(JColor.zero);
                }
                #endregion

                #region Tile Effect
                Dictionary<int, bool> twirlList = new Dictionary<int, bool>();
                Dictionary<int, int> holdList = new Dictionary<int, int>();
                Dictionary<int, double> pauseList = new Dictionary<int, double>();
                Dictionary<int, int> multiPlanetList = new Dictionary<int, int>();
                Dictionary<int, bool> autoTileList = new Dictionary<int, bool>();
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

                            if (twirlList.TryAdd(index, twirl))
                                adofaiMap.twirls.Add(new ADOFAITileEffectFile<bool>(index - 1, twirl));
                        }
                        else if (eventType == "Pause")
                        {
                            if (pauseList.ContainsKey(index))
                                pauseList[index] = pauseList[index] + action["duration"].Value<int>();
                            else
                                pauseList[index] = action["duration"].Value<int>();
                        }
                        else if (eventType == "FreeRoam")
                        {
                            if (pauseList.ContainsKey(index))
                                pauseList[index] = pauseList[index] + (action["duration"].Value<double>() - 1);
                            else
                                pauseList[index] = action["duration"].Value<double>() - 1;
                        }
                        else if (eventType == "Hold")
                        {
                            int duration = action["duration"].Value<int>();

                            if (holdList.TryAdd(index, duration))
                                adofaiMap.holds.Add(new ADOFAITileEffectFile<double>(index - 1, duration));
                        }
                        else if (eventType == "MultiPlanet")
                        {
                            string planets = action["planets"].Value<string>();

                            if (planets == "ThreePlanets")
                                multiPlanetList.TryAdd(index, 3);
                            else
                                multiPlanetList.TryAdd(index, 2);
                        }
                        else if (eventType == "AutoPlayTiles")
                        {
                            string enabled = action["enabled"].Value<string>();

                            if (enabled == "Enabled")
                            {
                                if (autoTileList.TryAdd(index, true))
                                    adofaiMap.autoTiles.Add(new ADOFAITileEffectFile<bool>(index - 1, true));
                            }
                            else
                            {
                                if (autoTileList.TryAdd(index, false))
                                    adofaiMap.autoTiles.Add(new ADOFAITileEffectFile<bool>(index - 1, false));
                            }
                        }
                    }
                }
                #endregion

                #region Beat
                {
                    #region Path Data
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
                                throw new NotSupportedException($"Unsupported pathData character '{path}'");

                            previousAngle = angleData[i];
                        }

                        adofai.angleData = angleData.ToArray();
                    }
                    #endregion

                    bool twirl = false;
                    double lastBeat = -1;
                    double lastAngle = 0;
                    double lastMultiPlanet = 1;
                    for (int i = 0; i < adofai.angleData.Length; i++)
                    {
                        if (twirlList.TryGetValue(i, out bool outTwirl))
                            twirl = outTwirl;

                        double pause = 0;
                        if (pauseList.TryGetValue(i, out double outPause))
                            pause = outPause;

                        int hold = 0;
                        if (holdList.TryGetValue(i, out int outHold))
                            hold = outHold;

                        double multiPlanet = lastMultiPlanet;
                        if (multiPlanetList.TryGetValue(i, out int outMultiPlanet))
                        {
                            multiPlanet = 2d / outMultiPlanet;
                            lastMultiPlanet = multiPlanet;
                        }

                        double beat = lastBeat;
                        double angle = adofai.angleData[i];
                        double offsetBeat;

                        if (angle >= 999)
                        {
                            //미드스핀 일경우 무조건 반대방향으로 회전하니 180도를 더하고 동타이니 오프셋 비트는 0이여야합니다
                            angle = (lastAngle + 180).Repeat(360);
                            offsetBeat = 0;
                        }
                        else
                        {
                            angle = angle.Repeat(360);

                            double offsetAngle = 0;
                            if (!twirl)
                                offsetAngle = lastAngle - angle;
                            else
                                offsetAngle = angle - lastAngle;

                            offsetBeat = (1 + (offsetAngle / (180d * multiPlanet)));
                            offsetBeat *= multiPlanet;
                            offsetBeat = offsetBeat.Repeat(2);

                            //미드스핀이 아닌데 오프셋 비트가 0 이하 일 경우 한 바퀴를 더 기다려야합니다
                            if (offsetBeat <= 0)
                                offsetBeat += 2;
                        }

                        beat += (offsetBeat + pause) + (hold * 2);

                        BeatAdd(beat, angle);

                        void BeatAdd(double beat, double angle)
                        {
                            adofaiMap.tiles.Add(beat);
                            allBeat.Add(beat);

                            lastAngle = angle;
                            lastBeat = beat;
                        }
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
                    double beat;
                    if (index >= 0)
                        beat = allBeat[index.Clamp(0, allBeat.Count - 1)];
                    else
                        beat = double.MinValue;

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
                        beat += angleOffset.Repeat(360) / 180;
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

                    void EffectAddMethod<T>(BeatValuePairList<T> list, double beat, T value)
                    {
                        if (list.Count > 0)
                        {
                            int startIndex = list.GetValueIndexBinarySearch(beat);

                            if (beat <= list[startIndex].beat)
                                list.RemoveRange(startIndex, list.Count - startIndex - 1);
                        }

                        list.Add(beat, value);
                    }

                    void EffectAddMethod2<T>(BeatValuePairAniList<T> list, double beat, double length, T value, EasingFunction.Ease easingFunction)
                    {
                        if (list.Count > 0)
                        {
                            int startIndex = list.GetValueIndexBinarySearch(beat);

                            if (beat <= list[startIndex].beat)
                                list.RemoveRange(startIndex, list.Count - startIndex - 1);
                        }

                        list.Add(beat, length, value, easingFunction, true);
                    }

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
                                effect.action += (double beat) => EffectAddMethod(adofaiMap.globalEffect.bpm, beat, bpm);
                            }
                            else
                            {
                                bpm = lastBpm * action["bpmMultiplier"].Value<float>();
                                effect.action += (double beat) => EffectAddMethod(adofaiMap.globalEffect.bpm, beat, bpm);
                            }
                        }
                        else
                        {
                            bpm = action["beatsPerMinute"].Value<float>();
                            effect.action += (double beat) => EffectAddMethod(adofaiMap.globalEffect.bpm, beat, bpm);
                        }

                        effect.isTileEffect = true;
                        lastBpm = bpm;
                    }
                    else if (eventType == "MoveCamera")
                    {
                        if (action.ContainsKey("position"))
                        {
                            float[] pos = action["position"].Values<float>().ToArray();
                            effect.action += (double beat) => EffectAddMethod2(adofaiMap.globalEffect.cameraPos, beat, duration, new JVector3(pos[0], pos[1], -14), ease);
                        }

                        if (action.ContainsKey("rotation"))
                        {
                            float rotation = action["rotation"].Value<float>();
                            effect.action += (double beat) => EffectAddMethod2(adofaiMap.globalEffect.cameraRotation, beat, duration, new JVector3(0, 0, rotation), ease);
                        }

                        if (action.ContainsKey("zoom"))
                        {
                            double zoom = action["zoom"].Value<float>() * 0.01;
                            effect.action += (double beat) => EffectAddMethod2(adofaiMap.globalEffect.cameraZoom, beat, duration, zoom, ease);
                        }
                    }
                    else if (eventType == "CustomBackground")
                    {
                        if (action.ContainsKey("imageColor"))
                        {
                            if (("#" + action["imageColor"]).TryHexToColor(out Color result))
                                effect.action += (double beat) => EffectAddMethod2(adofaiMap.globalEffect.backgroundEffect.backgroundColor, beat, 0, result, EasingFunction.Ease.Linear);
                            else
                                effect.action += (double beat) => EffectAddMethod2(adofaiMap.globalEffect.backgroundEffect.backgroundColor, beat, 0, JColor.one, EasingFunction.Ease.Linear);
                        }

                        if (action.ContainsKey("bgImage"))
                        {
                            string background = Path.GetFileNameWithoutExtension(action["bgImage"].Value<string>());
                            effect.action += (double beat) => EffectAddMethod(adofaiMap.globalEffect.backgroundEffect.background, beat, new BackgroundFileInfoPair(background, ""));
                        }
                    }
                    else if (eventType == "Flash")
                    {
                        if (action.ContainsKey("startColor") && action.ContainsKey("endColor") && action.ContainsKey("startOpacity") && action.ContainsKey("endOpacity") && action.ContainsKey("plane"))
                        {
                            if (("#" + action["startColor"]).TryHexToColor(out Color startColor) && ("#" + action["endColor"]).TryHexToColor(out Color endColor))
                            {
                                string plane = action.Value<string>("plane");
                                startColor.a *= action.Value<float>("startOpacity") * 0.01f;
                                endColor.a *= action.Value<float>("endOpacity") * 0.01f;

                                effect.action += (double beat) =>
                                {
                                    BeatValuePairAni<JColor> start = new BeatValuePairAni<JColor>(beat, startColor, 0, EasingFunction.Ease.Linear, true);
                                    BeatValuePairAni<JColor> end = new BeatValuePairAni<JColor>(beat, endColor, duration, EasingFunction.Ease.Linear, true);

                                    BeatValuePairAniListColor list;
                                    if (plane == "Foreground")
                                        list = adofaiMap.globalEffect.fieldFlash;
                                    else
                                        list = adofaiMap.globalEffect.backgroundFlash;

                                    if (list.Count > 0)
                                    {
                                        int startIndex = list.GetValueIndexBinarySearch(beat);

                                        if (beat <= list[startIndex].beat)
                                            list.RemoveRange(startIndex, list.Count - startIndex - 1);
                                    }

                                    list.Add(start);
                                    list.Add(end);
                                };
                            }
                        }
                    }
                    #endregion

                    repeatEvents.events.Add(effect);
                }

                repeatEvents.Invoke();
                #endregion

                if (allBeat.Count > 0)
                    adofaiMap.info.clearBeat = allBeat.Last() + 4;

                adofaiMap.Init(mapFilePath);
            }

            return adofaiMap;
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

                public Effect(double beat, string eventTag, EventsAction action, bool isTileEffect)
                {
                    this.beat = beat;
                    this.eventTag = eventTag;

                    this.action = action;

                    this.isTileEffect = isTileEffect;
                }

                public void Invoke(double offset) => action?.Invoke(beat + offset);

                public delegate void EventsAction(double beat);
            }
        }
    }
}
