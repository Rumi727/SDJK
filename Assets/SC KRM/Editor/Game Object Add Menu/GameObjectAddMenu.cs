using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    public sealed class GameObjectAddMenu
    {
        static void PrefabInstantiate(string name, MenuCommand menuCommand, string path = "Assets/SC KRM/Editor/Game Object Add Menu")
        {
            UnityEngine.Object gameObject = AssetDatabase.LoadAssetAtPath($"{PathUtility.Combine(path, name)}.prefab", typeof(UnityEngine.Object));
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

        [MenuItem("GameObject/Kernel/Save Load UI/Title")]
        public static void SaveLoadUITitle(MenuCommand menuCommand) => PrefabInstantiate("Title", menuCommand, "Assets/SC KRM/Resources/Prefab/Save Load UI");

        [MenuItem("GameObject/Kernel/Save Load UI/Input Field")]
        public static void SaveLoadUIInputField(MenuCommand menuCommand) => PrefabInstantiate("Save Load UI Input Field", menuCommand, "Assets/SC KRM/Resources/Prefab/Save Load UI");

        [MenuItem("GameObject/Kernel/Save Load UI/Slider")]
        public static void SaveLoadUISlider(MenuCommand menuCommand) => PrefabInstantiate("Save Load UI Slider", menuCommand, "Assets/SC KRM/Resources/Prefab/Save Load UI");

        [MenuItem("GameObject/Kernel/Save Load UI/Toggle")]
        public static void SaveLoadUIToggle(MenuCommand menuCommand) => PrefabInstantiate("Save Load UI Toggle", menuCommand, "Assets/SC KRM/Resources/Prefab/Save Load UI");

        [MenuItem("GameObject/Kernel/Save Load UI/Color Picker")]
        public static void SaveLoadUIColorPicker(MenuCommand menuCommand) => PrefabInstantiate("Save Load UI Color Picker", menuCommand, "Assets/SC KRM/Resources/Prefab/Save Load UI");

        [MenuItem("GameObject/Kernel/Save Load UI/Dropdown")]
        public static void SaveLoadUIDropdown(MenuCommand menuCommand) => PrefabInstantiate("Save Load UI Dropdown", menuCommand, "Assets/SC KRM/Resources/Prefab/Save Load UI");

        [MenuItem("GameObject/Kernel/Save Load UI/Space")]
        public static void SaveLoadUISpace(MenuCommand menuCommand) => PrefabInstantiate("Space", menuCommand, "Assets/SC KRM/Resources/Prefab/Save Load UI");

        [MenuItem("GameObject/Kernel/Save Load UI/Line")]
        public static void SaveLoadUILine(MenuCommand menuCommand) => PrefabInstantiate("Line", menuCommand, "Assets/SC KRM/Resources/Prefab/Save Load UI");

        [MenuItem("GameObject/Kernel/Save Load UI/Button")]
        public static void SaveLoadUIButton(MenuCommand menuCommand) => PrefabInstantiate("Save Load UI Button", menuCommand, "Assets/SC KRM/Save Load/Save Load UI");

        [MenuItem("GameObject/Kernel/Save Load UI/Save Load UI")]
        public static void SaveLoadUI(MenuCommand menuCommand) => PrefabInstantiate("Save Load UI", menuCommand, "Assets/SC KRM/Resources/Prefab/Save Load UI");

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