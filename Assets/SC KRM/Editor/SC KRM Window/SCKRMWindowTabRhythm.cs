using SCKRM.NBS;
using SCKRM.Rhythm;
using SCKRM.Sound;
using UnityEditor;

namespace SCKRM.Editor
{
    public sealed class SCKRMWindowTabRhythm : ISCKRMWindowTab
    {
        [InitializeOnLoadMethod] public static void Init() => SCKRMWindowEditor.TabAdd(new SCKRMWindowTabRhythm());

        public string name => "리듬";
        public int sortIndex => 400;


        public void OnGUI() => render();
        public static void Render(SCKRMWindowTabRhythm window) => window.render();

        void render()
        {
            if (RhythmManager.isPlaying)
                EditorGUILayout.LabelField("플레이 중");
            else
                EditorGUILayout.LabelField("플레이 중 아님");

            CustomInspectorEditor.DrawLine();

            EditorGUILayout.LabelField("현재 시간 - " + RhythmManager.time);

            CustomInspectorEditor.DrawLine();

            EditorGUILayout.LabelField("현재 비트 - " + RhythmManager.currentBeat);

            CustomInspectorEditor.Space();

            EditorGUILayout.LabelField("현재 비트 (화면) - " + RhythmManager.currentBeatScreen);
            EditorGUILayout.LabelField("현재 비트 (소리) - " + RhythmManager.currentBeatSound);

            CustomInspectorEditor.DrawLine();

            EditorGUILayout.LabelField("현재 속도 - " + RhythmManager.speed);

            CustomInspectorEditor.DrawLine();

            if (RhythmManager.screenYukiMode)
                EditorGUILayout.LabelField("유키 모드 O");
            else
                EditorGUILayout.LabelField("유키 모드 X");

            CustomInspectorEditor.DrawLine();

            EditorGUILayout.LabelField("BPM - " + RhythmManager.bpm);

            CustomInspectorEditor.Space();

            EditorGUILayout.LabelField("BPM 델타 타임 - " + RhythmManager.bpmDeltaTime);
            EditorGUILayout.LabelField("BPM FPS 델타 타임 - " + RhythmManager.bpmFpsDeltaTime);
            EditorGUILayout.LabelField("BPM 스케일 되지 않은 델타 타임 - " + RhythmManager.bpmUnscaledDeltaTime);
            EditorGUILayout.LabelField("BPM 스케일 되지 않은 FPS 델타 타임 - " + RhythmManager.bpmUnscaledDeltaTime);

            CustomInspectorEditor.DrawLine();

            EditorGUILayout.LabelField("오프셋 - " + RhythmManager.offset);

            CustomInspectorEditor.DrawLine();

            if (RhythmManager.soundPlayer != null)
            {
                if (RhythmManager.soundPlayer is SoundPlayer)
                    SoundPlayerEditor.GUI((SoundPlayer)RhythmManager.soundPlayer);
                else if (RhythmManager.soundPlayer is NBSPlayer)
                    NBSPlayerEditor.GUI((NBSPlayer)RhythmManager.soundPlayer);
                else
                    EditorGUILayout.HelpBox("제작자란 놈이 코딩을 타로마루 보다 못해서 커스텀 사운드 플레이어는 미리보기를 지원하지 않습니다\n제가 대신 사과드립니다 죄송합니다 - Ebisuzawa Kurumi", MessageType.Warning);
            }
            else
                EditorGUILayout.HelpBox("현재 리듬 매니저랑 연동된 사운드 플레이어가 없습니다", MessageType.Info);
        }
    }
}
