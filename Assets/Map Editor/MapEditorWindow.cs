using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Editor;
using SCKRM.Json;
using SCKRM.Rhythm;
using SDJK.Effect;
using SDJK.Map;
using SDJK.Ruleset;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SDJK.MapEditor
{
    public sealed class MapEditorWindow : EditorWindow
    {
        public MapFile mapFile;
        public MapEffectEditorWindow mapEffectEditor;

        public SCKRMWindowTabRhythm rhythmWindow;

        public void Init(MapFile mapFile)
        {
            this.mapFile = mapFile;
            mapEffectEditor = new MapEffectEditorWindow(this);
        }
        
        void OnEnable()
        {
            SceneManager.activeSceneChanged += ActiveSceneChanged;
            OnBecameVisible();
        }

        void OnDisable()
        {
            SceneManager.activeSceneChanged -= ActiveSceneChanged;
            OnBecameInvisible();
        }

        List<MonoBehaviour> judgementManagers = new List<MonoBehaviour>();
        void ActiveSceneChanged(Scene arg0, Scene arg1)
        {
            judgementManagers.Clear();
            MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>(true);

            for (int i = 0; i < monoBehaviours.Length; i++)
            {
                MonoBehaviour monoBehaviour = monoBehaviours[i];
                if (monoBehaviour is JudgementManagerBase)
                    judgementManagers.Add(monoBehaviour);
            }
        }

        bool up = false;
        bool down = false;
        bool space = false;
        bool shift = false;
        bool ctrl = false;
        bool alt = false;
        bool inspectorUpdate = true;
        MapFile lastMapFile;
        void OnGUI()
        {
            if (rhythmWindow == null)
                rhythmWindow = new SCKRMWindowTabRhythm();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("새로고침 딜레이", GUILayout.ExpandWidth(false));
            inspectorUpdate = EditorGUILayout.Toggle(inspectorUpdate, GUILayout.Width(15));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            CustomInspectorEditor.DrawLine(2);

            SCKRMWindowTabRhythm.Render(rhythmWindow, true);

            if (RhythmManager.soundPlayer != null && !RhythmManager.soundPlayer.isRemoved && !RhythmManager.soundPlayer.IsDestroyed())
            {
                if (Event.current.type == EventType.KeyDown)
                {
                    if (Event.current.keyCode == KeyCode.Keypad5)
                        space = !space;
                    else if (Event.current.keyCode == KeyCode.Keypad8)
                        up = true;
                    else if (Event.current.keyCode == KeyCode.Keypad2)
                        down = true;
                    else if (Event.current.keyCode == KeyCode.LeftControl)
                        ctrl = true;
                    else if (Event.current.keyCode == KeyCode.LeftShift)
                        shift = true;
                    else if (Event.current.keyCode == KeyCode.LeftAlt)
                        alt = true;
                }

                if (Event.current.type == EventType.KeyUp)
                {
                    if (Event.current.keyCode == KeyCode.Keypad8)
                        up = false;
                    else if (Event.current.keyCode == KeyCode.Keypad2)
                        down = false;
                    else if (Event.current.keyCode == KeyCode.LeftControl)
                        ctrl = false;
                    else if (Event.current.keyCode == KeyCode.LeftShift)
                        shift = false;
                    else if (Event.current.keyCode == KeyCode.LeftAlt)
                        alt = false;
                }
            }

            CustomInspectorEditor.DrawLine(2);

            if (!Kernel.isPlaying)
            {
                EditorGUILayout.HelpBox("Not playing", MessageType.Info);
                return;
            }

            EffectManager effectManager = FindObjectOfType<EffectManager>();
            if (effectManager != null)
            {
                mapFile = effectManager.selectedMap;

                if (mapFile == null)
                {
                    EditorGUILayout.HelpBox("No map selected in effect manager", MessageType.Info);
                    return;
                }

                if (lastMapFile != mapFile)
                {
                    Init(mapFile);
                    lastMapFile = mapFile;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Effect Manager script not found", MessageType.Info);
                return;
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("저장", GUILayout.ExpandWidth(false)))
            {
                if (File.Exists(mapFile.mapFilePath))
                    File.WriteAllText(mapFile.mapFilePath, JsonManager.ObjectToJson(mapFile));
            }

            if (GUILayout.Button("다른 이름으로 저장", GUILayout.ExpandWidth(false)))
            {
                string path = EditorUtility.SaveFilePanel("다른 이름으로 저장", Kernel.saveDataPath, "map.sdjk", "sdjk");
                if (!string.IsNullOrEmpty(path))
                    File.WriteAllText(path, JsonManager.ObjectToJson(mapFile));
            }

            GUILayout.Label("K8: " + up, GUILayout.ExpandWidth(false));
            GUILayout.Label("K2: " + down, GUILayout.ExpandWidth(false));
            GUILayout.Label("K5: " + space, GUILayout.ExpandWidth(false));
            GUILayout.Label("LCtrl: " + ctrl, GUILayout.ExpandWidth(false));
            GUILayout.Label("LShift: " + shift, GUILayout.ExpandWidth(false));
            GUILayout.Label("LAlt: " + alt, GUILayout.ExpandWidth(false));

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            CustomInspectorEditor.DrawLine(2, 0);

            mapEffectEditor.OnGUI();
        }

        //창이 활성화 될때 콜백
        void OnBecameVisible()
        {
            cancel.Cancel();
            cancel = new CancellationTokenSource();

            AsyncUpdate().Forget();
        }

        //창이 비활성화 될때 콜백
        void OnBecameInvisible() => cancel.Cancel();

        void OnInspectorUpdate()
        {
            if (inspectorUpdate && Kernel.isPlayingAndNotPaused)
                Repaint();
        }

        void Update()
        {
            if (!Kernel.isPlaying || !RhythmManager.isPlaying)
                return;

            double speed = 1;
            if (ctrl && shift)
                speed = 8;
            else if (shift)
                speed = 4;
            else if (ctrl)
                speed = 2;

            if (alt)
                speed = 1d / speed;

            if (mapFile != null)
                speed *= mapFile.globalEffect.tempo.GetValue(RhythmManager.currentBeatSound);

            if (down)
            {
                RhythmManager.isPaused = true;
                RhythmManager.Rewind(Kernel.deltaTimeDouble * speed);
            }
            else if (up || space)
                RhythmManager.isPaused = false;
            else
                RhythmManager.isPaused = true;

            for (int i = 0; i < judgementManagers.Count; i++)
                judgementManagers[i].gameObject.SetActive(false);

            if (!inspectorUpdate && Kernel.isPlayingAndNotPaused)
                Repaint();
        }

        double hitsoundBeat = 0;
        CancellationTokenSource cancel = new CancellationTokenSource();
        async UniTaskVoid AsyncUpdate()
        {
            while (true)
            {
                if (Kernel.isPlaying && RhythmManager.isPlaying)
                {
                    if (mapFile != null)
                    {
                        double speed = 1;
                        if (ctrl && shift)
                            speed = 8;
                        else if (shift)
                            speed = 4;
                        else if (ctrl)
                            speed = 2;

                        if (alt)
                            speed = 1d / speed;

                        RhythmManager.speed = mapFile.globalEffect.tempo.GetValue(RhythmManager.currentBeatSound) * speed;
                    }

                    if (HitsoundEffect.GetHitsoundPlayCount(mapFile, RhythmManager.currentBeatSound, ref hitsoundBeat) > 0)
                        HitsoundEffect.HitsoundPlay();
                }

                if (await UniTask.NextFrame(PlayerLoopTiming.LastUpdate, cancel.Token).SuppressCancellationThrow())
                    return;
            }
        }

        void OnLostFocus()
        {
            up = false;
            down = false;
        }

        [MenuItem("SDJK/Map Editor")]
        static void ShowWindow()
        {
            MapEditorWindow window = GetWindow<MapEditorWindow>("Map Editor");
            window.Show();
        }
    }
}
