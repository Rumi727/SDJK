using SCKRM.UI;
using UnityEditor;
using UnityEngine;

namespace SCKRM.Editor
{
    public static class AutoSceneVisibility
    {
        [InitializeOnLoadMethod]
        static void Init()
        {
            Selection.selectionChanged += Update;
            Update();
        }

        public static void Update()
        {
            CanvasSetting[] canvasSettings = UnityEngine.Object.FindObjectsOfType<CanvasSetting>(true);
            for (int i = 0; i < canvasSettings.Length; i++)
            {
                CanvasSetting canvasSetting = canvasSettings[i];
                if (canvasSetting.alwaysVisible)
                    SceneVisibilityManager.instance.Show(canvasSetting.gameObject, true);
                else
                    SceneVisibilityManager.instance.Hide(canvasSetting.gameObject, true);
            }

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Canvas canvas = Selection.gameObjects[i].GetComponentInParent<Canvas>(true);
                if (canvas != null)
                    SceneVisibilityManager.instance.Show(canvas.gameObject, true);
            }
        }
    }
}
