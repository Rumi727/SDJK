using SCKRM.NBS;
using SCKRM.Resource;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NBSPlayer))]
    public class NBSPlayerEditor : CustomInspectorEditor
    {
        NBSPlayer editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (NBSPlayer)target;
        }

        public override void OnInspectorGUI() => GUI(editor);

        public static void GUI(NBSPlayer nbsPlayer)
        {
            if (!Kernel.isPlaying || !InitialLoadManager.isInitialLoadEnd || nbsPlayer == null || nbsPlayer.soundData == null || nbsPlayer.metaData == null || nbsPlayer.nbsFile == null)
                return;

            bool refesh;
            bool pauseToggle;
            bool stop;
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("네임스페이스", GUILayout.ExpandWidth(false));
                nbsPlayer.nameSpace = DrawNameSpace(nbsPlayer.nameSpace);
                GUILayout.Label("NBS 키", GUILayout.ExpandWidth(false));
                nbsPlayer.key = DrawStringArray(nbsPlayer.key, ResourceManager.GetNBSDataKeys(nbsPlayer.nameSpace));

                refesh = GUILayout.Button("새로고침", GUILayout.ExpandWidth(false));
                string text;
                if (!nbsPlayer.isPaused)
                    text = "일시정지";
                else
                    text = "재생";
                pauseToggle = GUILayout.Button(text, GUILayout.ExpandWidth(false));
                stop = GUILayout.Button("정지", GUILayout.ExpandWidth(false));

                EditorGUILayout.EndHorizontal();
            }
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("볼륨", GUILayout.ExpandWidth(false));
                nbsPlayer.volume = EditorGUILayout.Slider(nbsPlayer.volume, 0, 1f.Max(nbsPlayer.volume));
                GUILayout.Label("반복", GUILayout.ExpandWidth(false));
                nbsPlayer.loop = EditorGUILayout.Toggle(nbsPlayer.loop, GUILayout.Width(15));

                GUILayout.Label("피치", GUILayout.ExpandWidth(false));
                nbsPlayer.pitch = EditorGUILayout.Slider(nbsPlayer.pitch, -3f.Min(nbsPlayer.pitch), 3f.Max(nbsPlayer.pitch));

                GUILayout.Label("템포", GUILayout.ExpandWidth(false));
                nbsPlayer.tempo = EditorGUILayout.Slider(nbsPlayer.tempo, -3f.Min(nbsPlayer.tempo), 3f.Max(nbsPlayer.tempo));

                EditorGUILayout.EndHorizontal();
            }
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("3D", GUILayout.ExpandWidth(false));
                nbsPlayer.spatial = EditorGUILayout.Toggle(nbsPlayer.spatial, GUILayout.Width(15));

                if (nbsPlayer.spatial)
                {
                    GUILayout.Label("최소 거리", GUILayout.ExpandWidth(false));
                    nbsPlayer.minDistance = EditorGUILayout.Slider(nbsPlayer.minDistance, 0, 64f.Max(nbsPlayer.minDistance));
                    GUILayout.Label("최대 거리", GUILayout.ExpandWidth(false));
                    nbsPlayer.maxDistance = EditorGUILayout.Slider(nbsPlayer.maxDistance, 0, 64f.Max(nbsPlayer.maxDistance));

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label("위치", GUILayout.ExpandWidth(false));
                    nbsPlayer.localPosition = EditorGUILayout.Vector3Field("", nbsPlayer.localPosition);

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.Label("스테레오", GUILayout.ExpandWidth(false));
                    nbsPlayer.panStereo = EditorGUILayout.Slider(nbsPlayer.panStereo, -1, 1);

                    EditorGUILayout.EndHorizontal();
                }
            }

            DrawLine(1);

            {
                EditorGUILayout.BeginHorizontal();

                if (nbsPlayer.soundData == null)
                {
                    GUILayout.Label("--:-- / --:--", GUILayout.ExpandWidth(false));
                    GUILayout.HorizontalSlider(0, 0, 1);
                }
                else
                {
                    float timer = nbsPlayer.time;
                    float length = nbsPlayer.length * 0.05f;

                    string time = timer.ToTime();
                    string endTime = length.ToTime();

                    if (nbsPlayer.tempo == 0)
                        GUILayout.Label($"--:-- / --:-- ({time} / {endTime})", GUILayout.ExpandWidth(false));
                    else if (nbsPlayer.tempo.Abs() != 1)
                    {
                        string pitchTime = nbsPlayer.realTime.ToTime();
                        string pitchEndTime = nbsPlayer.realLength.ToTime();

                        GUILayout.Label($"{pitchTime} / {pitchEndTime} ({time} / {endTime}) ({nbsPlayer.tick} / {nbsPlayer.length})", GUILayout.ExpandWidth(false));
                    }
                    else
                        GUILayout.Label($"{time} / {endTime} ({nbsPlayer.tick} / {nbsPlayer.length})", GUILayout.ExpandWidth(false));

                    float audioTime = GUILayout.HorizontalSlider(timer, 0, length);
                    if (timer != audioTime && !refesh)
                        nbsPlayer.time = audioTime;
                }

                GUILayout.Label($"{nbsPlayer.index} / {nbsPlayer.nbsFile.nbsNotes.Count - 1}", GUILayout.ExpandWidth(false));

                EditorGUILayout.EndHorizontal();
            }

            if (refesh)
                nbsPlayer.Refresh();
            else if (pauseToggle)
                nbsPlayer.isPaused = !nbsPlayer.isPaused;
            else if (stop)
                nbsPlayer.Remove();
        }
    }
}