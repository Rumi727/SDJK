using SCKRM.Resource;
using SCKRM.Sound;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    public sealed class SCKRMWindowTabNBS : ISCKRMWindowTab
    {
        [InitializeOnLoadMethod] public static void Init() => SCKRMWindowEditor.TabAdd(new SCKRMWindowTabNBS());

        public string name => "NBS";
        public int sortIndex => 200;


        public void OnGUI() => render();
        public static void Render(SCKRMWindowTabNBS window, int scrollYSize = 0) => window.render(scrollYSize);

        string nbsNameSpace = "";
        string nbsKey = "";

        float nbsVolume = 1;
        bool nbsLoop = false;

        float nbsPitch = 1;
        float nbsTempo = 1;

        float nbsPanStereo = 0;
        bool nbsSpatial = false;

        float nbsMinDistance = 0;
        float nbsMaxDistance = 48;
        Vector3 nbsLocalPosition = Vector3.zero;

        Vector2 nbsScrollPos = Vector2.zero;
        void render(int scrollYSize = 0)
        {
            EditorGUILayout.LabelField("제어판", EditorStyles.boldLabel);

            {
                {
                    bool nbsPlay;
                    {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label("네임스페이스", GUILayout.ExpandWidth(false));
                        nbsNameSpace = CustomInspectorEditor.DrawNameSpace(nbsNameSpace);
                        GUILayout.Label("NBS 키", GUILayout.ExpandWidth(false));
                        nbsKey = CustomInspectorEditor.DrawStringArray(nbsKey, ResourceManager.GetNBSDataKeys(nbsNameSpace));

                        if (!Kernel.isPlaying)
                            GUI.enabled = false;

                        nbsPlay = GUILayout.Button("NBS 재생", GUILayout.ExpandWidth(false));
                        if (GUILayout.Button("NBS 정지", GUILayout.ExpandWidth(false)))
                            SoundManager.StopNBS(nbsKey, nbsNameSpace);
                        if (GUILayout.Button("모든 NBS 정지", GUILayout.ExpandWidth(false)))
                            SoundManager.StopNBSAll();

                        GUI.enabled = true;

                        EditorGUILayout.EndHorizontal();
                    }
                    {
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label("볼륨", GUILayout.ExpandWidth(false));
                        nbsVolume = EditorGUILayout.Slider(nbsVolume, 0, 1);
                        GUILayout.Label("반복", GUILayout.ExpandWidth(false));
                        nbsLoop = EditorGUILayout.Toggle(nbsLoop, GUILayout.Width(15));

                        GUILayout.Label("피치", GUILayout.ExpandWidth(false));
                        nbsPitch = EditorGUILayout.Slider(nbsPitch, -3, 3);
                        GUILayout.Label("템포", GUILayout.ExpandWidth(false));
                        nbsTempo = EditorGUILayout.Slider(nbsTempo, -3, 3);

                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();

                        GUILayout.Label($"{SoundManager.nbsList.Count} / {SoundManager.maxNBSCount}", GUILayout.ExpandWidth(false));

                        GUILayout.Label("3D", GUILayout.ExpandWidth(false));
                        nbsSpatial = EditorGUILayout.Toggle(nbsSpatial, GUILayout.Width(15));

                        if (nbsSpatial)
                        {
                            GUILayout.Label("최소 거리", GUILayout.ExpandWidth(false));
                            nbsMinDistance = EditorGUILayout.Slider(nbsMinDistance, 0, 64);
                            GUILayout.Label("최대 거리", GUILayout.ExpandWidth(false));
                            nbsMaxDistance = EditorGUILayout.Slider(nbsMaxDistance, 0, 64);

                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();

                            GUILayout.Label("위치", GUILayout.ExpandWidth(false));
                            nbsLocalPosition = EditorGUILayout.Vector3Field("", nbsLocalPosition);

                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.Label("스테레오", GUILayout.ExpandWidth(false));
                            nbsPanStereo = EditorGUILayout.Slider(nbsPanStereo, -1, 1);

                            EditorGUILayout.EndHorizontal();
                        }
                    }

                    if (nbsPlay)
                    {
                        if (nbsSpatial)
                            SoundManager.PlayNBS(nbsKey, nbsNameSpace, nbsVolume, nbsLoop, nbsPitch, nbsTempo, nbsPanStereo, nbsMinDistance, nbsMaxDistance, null, nbsLocalPosition.x, nbsLocalPosition.y, nbsLocalPosition.z);
                        else
                            SoundManager.PlayNBS(nbsKey, nbsNameSpace, nbsVolume, nbsLoop, nbsPitch, nbsTempo, nbsPanStereo);
                    }
                }
            }

            if (Kernel.isPlaying && InitialLoadManager.isInitialLoadEnd)
            {
                CustomInspectorEditor.DrawLine(2);

                EditorGUILayout.LabelField("재생 목록", EditorStyles.boldLabel);

                if (scrollYSize > 0)
                    nbsScrollPos = EditorGUILayout.BeginScrollView(nbsScrollPos, GUILayout.Height(scrollYSize));
                else
                    nbsScrollPos = EditorGUILayout.BeginScrollView(nbsScrollPos);

                for (int i = SoundManager.nbsList.Count - 1; i >= 0; i--)
                {
                    NBSPlayerEditor.GUI(SoundManager.nbsList[i]);
                    CustomInspectorEditor.DrawLine(2);
                }

                EditorGUILayout.EndScrollView();
            }

            GUI.enabled = true;
        }
    }
}
