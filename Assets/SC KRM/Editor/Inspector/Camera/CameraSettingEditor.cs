using SCKRM.Camera;
using UnityEditor;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CameraSetting))]
    public class CameraSettingEditor : CustomInspectorEditor
    {
        public override void OnInspectorGUI() => UseProperty("_customSetting", "커스텀 설정");
    }
}