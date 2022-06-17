using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityToolbarExtender
{
	internal class CustomToolbarSetting : ScriptableObject
	{
		const string SETTING_PATH = "Assets/SC KRM/CustomToolbar/Editor/CustomToolbarSetting.asset";

		[SerializeReference] internal List<BaseToolbarElement> elements = new List<BaseToolbarElement>();

		internal static CustomToolbarSetting GetOrCreateSetting()
		{
			var setting = AssetDatabase.LoadAssetAtPath<CustomToolbarSetting>(SETTING_PATH);
			if (setting == null)
			{
				setting = ScriptableObject.CreateInstance<CustomToolbarSetting>();
				//TODO: default setup in another ScriptableObject
				setting.elements = new List<BaseToolbarElement>() {
					new ToolbarEnterPlayMode(),
					new ToolbarSceneSelection(),
					new ToolbarSpace(),

					new ToolbarSavingPrefs(),
					new ToolbarClearPrefs(),
					new ToolbarSpace(),

					new ToolbarReloadScene(),
					new ToolbarStartFromFirstScene(),
					new ToolbarSpace(),

					new ToolbarSides(),

					new ToolbarTimeslider(),
					new ToolbarFPSSlider(),
					new ToolbarSpace(),

					new ToolbarRecompile(),
					new ToolbarReserializeSelected(),
					new ToolbarReserializeAll(),
				};

				if (!Directory.Exists("Assets/SC KRM/CustomToolbar/Editor"))
				{
					AssetDatabase.CreateFolder("Assets/SC KRM/CustomToolbar", "Editor");
					AssetDatabase.SaveAssets();
				}

				AssetDatabase.CreateAsset(setting, SETTING_PATH);
				AssetDatabase.SaveAssets();
			}

			return setting;
		}

		internal static SerializedObject GetSerializedSetting()
		{
			return new SerializedObject(GetOrCreateSetting());
		}
	}
}