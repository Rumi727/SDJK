using SCKRM.Resource;
using SCKRM.Sound;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    public sealed class SCKRMWindowTabSound : ISCKRMWindowTab
    {
        [InitializeOnLoadMethod] public static void Init() => SCKRMWindowEditor.TabAdd(new SCKRMWindowTabSound());

        public string name => "오디오";
        public int sortIndex => 100;


        public void OnGUI() => render();
        public static void Render(SCKRMWindowTabSound window, int scrollYSize = 0) => window.render(scrollYSize);

        string audioNameSpace = "";
        string audioKey = "";
        AudioClip audioClip;

        float audioVolume = 1;
        bool audioLoop = false;

        float audioPitch = 1;
        float audioTempo = 1;

        float audioPanStereo = 0;
        bool audioSpatial = false;

        float audioMinDistance = 0;
        float audioMaxDistance = 16;
        Vector3 audioLocalPosition = Vector3.zero;

        Vector2 audioScrollPos = Vector2.zero;
        void render(int scrollYSize = 0)
        {
            EditorGUILayout.LabelField("제어판", EditorStyles.boldLabel);

            {
                {
                    {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label("네임스페이스", GUILayout.ExpandWidth(false));
                        audioNameSpace = CustomInspectorEditor.DrawNameSpace(audioNameSpace);
                        GUILayout.Label("오디오 키", GUILayout.ExpandWidth(false));
                        audioKey = CustomInspectorEditor.DrawStringArray(audioKey, ResourceManager.GetSoundDataKeys(audioNameSpace));
                        GUILayout.Label("오디오 클립", GUILayout.ExpandWidth(false));
                        audioClip = (AudioClip)EditorGUILayout.ObjectField(audioClip, typeof(AudioClip), true);

                        if (!Kernel.isPlaying)
                            GUI.enabled = false;

                        GUI.enabled = true;

                        EditorGUILayout.EndHorizontal();
                    }
                    {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label("볼륨", GUILayout.ExpandWidth(false));
                        audioVolume = EditorGUILayout.Slider(audioVolume, 0, 1);
                        GUILayout.Label("반복", GUILayout.ExpandWidth(false));
                        audioLoop = EditorGUILayout.Toggle(audioLoop, GUILayout.Width(15));

                        GUILayout.Label("피치", GUILayout.ExpandWidth(false));
                        audioPitch = EditorGUILayout.Slider(audioPitch, -3, 3);

                        GUILayout.Label("템포", GUILayout.ExpandWidth(false));
                        audioTempo = EditorGUILayout.Slider(audioTempo, -3, 3);

                        EditorGUILayout.EndHorizontal();
                    }
                    {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label("3D", GUILayout.ExpandWidth(false));
                        audioSpatial = EditorGUILayout.Toggle(audioSpatial, GUILayout.Width(15));

                        if (audioSpatial)
                        {
                            GUILayout.Label("최소 거리", GUILayout.ExpandWidth(false));
                            audioMinDistance = EditorGUILayout.Slider(audioMinDistance, 0, 64);
                            GUILayout.Label("최대 거리", GUILayout.ExpandWidth(false));
                            audioMaxDistance = EditorGUILayout.Slider(audioMaxDistance, 0, 64);

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();

                            GUILayout.Label("위치", GUILayout.ExpandWidth(false));
                            audioLocalPosition = EditorGUILayout.Vector3Field("", audioLocalPosition);

                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.Label("스테레오", GUILayout.ExpandWidth(false));
                            audioPanStereo = EditorGUILayout.Slider(audioPanStereo, -1, 1);

                            EditorGUILayout.EndHorizontal();
                        }
                    }
                }

                {
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label($"{SoundManager.soundList.Count} / {SoundManager.maxSoundCount}", GUILayout.ExpandWidth(false));

                    if (!Kernel.isPlaying)
                        GUI.enabled = false;

                    if (GUILayout.Button("모든 음악 정지", GUILayout.ExpandWidth(false)))
                        SoundManager.StopSoundAll(true);
                    if (GUILayout.Button("모든 효과음 정지", GUILayout.ExpandWidth(false)))
                        SoundManager.StopSoundAll(false);
                    if (GUILayout.Button("모든 소리 정지", GUILayout.ExpandWidth(false)))
                        SoundManager.StopSoundAll();

                    EditorGUILayout.Space();

                    if (GUILayout.Button("오디오 리셋", GUILayout.ExpandWidth(false)))
                        ResourceManager.AudioReset();

                    bool audioPlay = GUILayout.Button("오디오 재생", GUILayout.ExpandWidth(false));
                    if (GUILayout.Button("오디오 정지", GUILayout.ExpandWidth(false)))
                        SoundManager.StopSound(audioKey, audioNameSpace);

                    if (audioPlay)
                    {
                        if (audioSpatial)
                            SoundManager.PlaySound(audioKey, audioNameSpace, audioVolume, audioLoop, audioPitch, audioTempo, audioPanStereo, audioMinDistance, audioMaxDistance, null, audioLocalPosition.x, audioLocalPosition.y, audioLocalPosition.z);
                        else
                            SoundManager.PlaySound(audioKey, audioNameSpace, audioVolume, audioLoop, audioPitch, audioTempo, audioPanStereo);
                    }

                    GUI.enabled = true;

                    EditorGUILayout.EndHorizontal();
                }
            }

            if (Kernel.isPlaying && InitialLoadManager.isInitialLoadEnd)
            {
                CustomInspectorEditor.DrawLine(2);

                EditorGUILayout.LabelField("재생 목록", EditorStyles.boldLabel);
                if (scrollYSize > 0)
                    audioScrollPos = EditorGUILayout.BeginScrollView(audioScrollPos, GUILayout.Height(scrollYSize));
                else
                    audioScrollPos = EditorGUILayout.BeginScrollView(audioScrollPos);

                for (int i = SoundManager.soundList.Count - 1; i >= 0; i--)
                {
                    SoundPlayerEditor.GUI(SoundManager.soundList[i]);
                    CustomInspectorEditor.DrawLine(2);
                }

                EditorGUILayout.EndScrollView();
            }
        }
    }
}
