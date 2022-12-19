using SCKRM.Sound;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SoundManager))]
    public class SoundManagerEditor : CustomInspectorEditor
    {
        static SCKRMWindowTabSound window = new();

        public override void OnInspectorGUI()
        {
            SCKRMWindowTabSound.Render(window, 300);
            DrawLine();

            EditorGUILayout.HelpBox("오디오 믹서 그룹을 넣어주세요", MessageType.None);
            UseProperty("_audioMixerGroup");
        }
    }
}