using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    public sealed class GameObjectAddMenu
    {
        static void PrefabInstantiate(string name, MenuCommand menuCommand)
        {
            UnityEngine.Object gameObject = AssetDatabase.LoadAssetAtPath($"Assets/SC KRM/Editor/Game Object Add Menu/{name}.prefab", typeof(UnityEngine.Object));
            if (menuCommand.context != null)
                gameObject = PrefabUtility.InstantiatePrefab(gameObject, ((GameObject)menuCommand.context).transform);
            else
                gameObject = PrefabUtility.InstantiatePrefab(gameObject, null);

            gameObject.name = name;

            Undo.RegisterCreatedObjectUndo(gameObject, "Create " + name);
            Selection.activeObject = gameObject;
        }

        [MenuItem("GameObject/Kernel/UI/Color Picker")]
        public static void ColorPicker(MenuCommand menuCommand) => PrefabInstantiate("Color Picker", menuCommand);

        [MenuItem("GameObject/Kernel/UI/Dropdown")]
        public static void Dropdown(MenuCommand menuCommand) => PrefabInstantiate("Dropdown", menuCommand);

        [MenuItem("GameObject/Kernel/Setting UI/Title")]
        public static void SettingTitle(MenuCommand menuCommand) => PrefabInstantiate("Title", menuCommand);

        [MenuItem("GameObject/Kernel/Setting UI/Input Field")]
        public static void SettingInputField(MenuCommand menuCommand) => PrefabInstantiate("Setting Input Field", menuCommand);

        [MenuItem("GameObject/Kernel/Setting UI/Slider")]
        public static void SettingSlider(MenuCommand menuCommand) => PrefabInstantiate("Setting Slider", menuCommand);

        [MenuItem("GameObject/Kernel/Setting UI/Toggle")]
        public static void SettingToggle(MenuCommand menuCommand) => PrefabInstantiate("Setting Toggle", menuCommand);

        [MenuItem("GameObject/Kernel/Setting UI/Color Picker")]
        public static void SettingColorPicker(MenuCommand menuCommand) => PrefabInstantiate("Setting Color Picker", menuCommand);

        [MenuItem("GameObject/Kernel/Setting UI/Dropdown")]
        public static void SettingDropdown(MenuCommand menuCommand) => PrefabInstantiate("Setting Dropdown", menuCommand);

        [MenuItem("GameObject/Kernel/Setting UI/Space")]
        public static void SettingSpace(MenuCommand menuCommand) => PrefabInstantiate("Space", menuCommand);

        [MenuItem("GameObject/Kernel/Setting UI/Line")]
        public static void SettingLine(MenuCommand menuCommand) => PrefabInstantiate("Line", menuCommand);

        [MenuItem("GameObject/Kernel/Setting UI/Button")]
        public static void SettingButton(MenuCommand menuCommand) => PrefabInstantiate("Setting Button", menuCommand);

        [MenuItem("GameObject/Kernel/UI/Side Bar/Left")]
        public static void SideBarLeft(MenuCommand menuCommand) => PrefabInstantiate("Side Bar", menuCommand);

        [MenuItem("GameObject/Kernel/UI/Side Bar/Right")]
        public static void SideBarRight(MenuCommand menuCommand) => PrefabInstantiate("Side Bar Right", menuCommand);

        [MenuItem("GameObject/Kernel/UI/Progress Bar")]
        public static void ProgressBar(MenuCommand menuCommand) => PrefabInstantiate("Progress Bar", menuCommand);

        [MenuItem("GameObject/Kernel/Input/Control Setting")]
        public static void ControlSetting(MenuCommand menuCommand) => PrefabInstantiate("Control Setting", menuCommand);
    }
}