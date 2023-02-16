using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SCKRM.SaveLoad;
using SCKRM.Resource;
using SCKRM.Json;
using System.IO;
using SCKRM.Renderer;
using UnityEditor.UI;
using System.Reflection;
using System;
using SpriteMetaData = SCKRM.Resource.SpriteMetaData;

namespace SCKRM.Editor
{
    public class SpriteProjectSetting : SettingsProvider
    {
        public SpriteProjectSetting(string path, SettingsScope scopes) : base(path, scopes) { }

        static SettingsProvider instance;
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            if (instance == null)
                instance = new SpriteProjectSetting("SC KRM/스프라이트", SettingsScope.Project);
            
            return instance;
        }

        public override void OnGUI(string searchContext)
        {
            nameSpace = CustomInspectorEditor.DrawNameSpace("네임스페이스", nameSpace);

            if (nameSpace == null || nameSpace == "")
                nameSpace = ResourceManager.defaultNameSpace;

            string[] types = ResourceManager.GetSpriteTypes(nameSpace);
            if (types == null)
                return;

            type = CustomInspectorEditor.DrawStringArray("타입", type, types);

            string[] spriteKeys = ResourceManager.GetSpriteKeys(type, nameSpace);
            if (spriteKeys == null)
                return;

            name = CustomInspectorEditor.DrawStringArray("이름", name, spriteKeys);

            tag = EditorGUILayout.TextField("태그", tag);

            EditorGUILayout.Space();

            index = EditorGUILayout.IntField("스프라이트 인덱스", index).Clamp(0);

            DrawGUI(nameSpace, type, name, tag, index);
            DrawSprite(nameSpace, type, name, tag, index, ref previewSize);
        }

        public static SaveLoadClass controlProjectSetting;
        string nameSpace = "";
        string type = "";
        string name = "";
        string tag = ResourceManager.spriteDefaultTag;
        int index = 0;
        public static void DrawGUI(string nameSpace, string type, string name, string tag, int index)
        {
            string typePath = PathUtility.Combine(ResourceManager.texturePath.Replace("%NameSpace%", nameSpace), type);
            string filePath = PathUtility.Combine(typePath, name);
            string typeAllPath = PathUtility.Combine(Kernel.streamingAssetsPath, typePath);
            ResourceManager.FileExtensionExists(PathUtility.Combine(Kernel.streamingAssetsPath, filePath), out string fileAllPath, ResourceManager.textureExtension);

            if (Directory.Exists(typeAllPath) && type != null && type != "")
            {
                TextureMetaData textureMetaData = JsonManager.JsonRead<TextureMetaData>(typeAllPath + ".json", true);
                if (textureMetaData == null)
                    textureMetaData = new TextureMetaData();

                CustomInspectorEditor.DrawLine();

                if (Kernel.isPlaying)
                {
                    EditorGUILayout.HelpBox($"이 밑부터는 플레이 모드가 켜져 있어도 변경 사항이 저장됩니다", MessageType.Warning);
                    EditorGUILayout.Space();
                }

                textureMetaData.filterMode = (FilterMode)EditorGUILayout.EnumPopup("필터 모드", textureMetaData.filterMode);
                textureMetaData.mipmapUse = EditorGUILayout.Toggle("밉맵 사용", textureMetaData.mipmapUse);
                textureMetaData.compressionType = (TextureMetaData.CompressionType)EditorGUILayout.EnumPopup("압축 타입", textureMetaData.compressionType);

                Texture2D texture = ResourceManager.GetTexture(fileAllPath, true, textureMetaData, TextureFormat.Alpha8);
                if (texture != null && name != null && name != "")
                {
                    CustomInspectorEditor.DrawLine();

                    Dictionary<string, List<SpriteMetaData>> spriteMetaDataLists = JsonManager.JsonRead<Dictionary<string, List<SpriteMetaData>>>(fileAllPath + ".json", true);
                    List<SpriteMetaData> spriteMetaDatas;

                    if (spriteMetaDataLists == null)
                        spriteMetaDataLists = new Dictionary<string, List<SpriteMetaData>>();

                    if (!spriteMetaDataLists.ContainsKey(ResourceManager.spriteDefaultTag))
                        spriteMetaDataLists.Add(ResourceManager.spriteDefaultTag, new List<SpriteMetaData>());

                    if (spriteMetaDataLists.ContainsKey(tag))
                        spriteMetaDatas = spriteMetaDataLists[tag];
                    else
                        spriteMetaDatas = spriteMetaDataLists[ResourceManager.spriteDefaultTag];

                    if (index < spriteMetaDatas.Count)
                    {
                        SpriteMetaData spriteMetaData = spriteMetaDatas[index];

                        spriteMetaData.RectMinMax(texture.width, texture.height);
                        spriteMetaData.PixelsPreUnitMinSet();

                        spriteMetaData.pivot = EditorGUILayout.Vector2Field("중심", spriteMetaData.pivot);
                        EditorGUILayout.BeginHorizontal();
                        spriteMetaData.rect = EditorGUILayout.Vector4Field("자르기", spriteMetaData.rect);
                        EditorGUILayout.EndHorizontal();
                        spriteMetaData.border = EditorGUILayout.Vector4Field("가장자리", spriteMetaData.border);

                        EditorGUILayout.Space();

                        spriteMetaData.pixelsPerUnit = EditorGUILayout.FloatField("1 픽셀 크기", spriteMetaData.pixelsPerUnit);

                        CustomInspectorEditor.DrawLine();

                        if (GUILayout.Button("스프라이트 지우기"))
                            spriteMetaDatas.RemoveAt(index);
                    }
                    else if (GUILayout.Button("스프라이트 만들기"))
                    {
                        SpriteMetaData spriteMetaData = new SpriteMetaData();
                        spriteMetaData.RectMinMax(texture.width, texture.height);
                        spriteMetaData.PixelsPreUnitMinSet();
                        spriteMetaDatas.Add(spriteMetaData);
                    }

                    if (!spriteMetaDataLists.ContainsKey(tag))
                    {
                        if (GUILayout.Button("태그 만들기"))
                            spriteMetaDataLists.Add(tag, new List<SpriteMetaData>());

                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox($"스프라이트 메타 데이터에 '{tag}' 태그가 없기 때문에 기본 태그를 불러왔습니다", MessageType.None);
                    }
                    else
                    {
                        bool lastEnabled = GUI.enabled;
                        if (tag == ResourceManager.spriteDefaultTag)
                            GUI.enabled = false;

                        if (GUILayout.Button("태그 지우기"))
                            spriteMetaDataLists.Remove(tag);

                        GUI.enabled = lastEnabled;
                    }

                    if (GUI.changed)
                    {
                        File.WriteAllText(typeAllPath + ".json", JsonManager.ObjectToJson(textureMetaData));
                        File.WriteAllText(fileAllPath + ".json", JsonManager.ObjectToJson(spriteMetaDataLists));

                        AssetDatabase.Refresh();
                    }
                }
                else if (GUI.changed)
                {
                    File.WriteAllText(typeAllPath + ".json", JsonManager.ObjectToJson(textureMetaData));
                    AssetDatabase.Refresh();
                }

                UnityEngine.Object.DestroyImmediate(texture);
            }

            CustomInspectorEditor.DrawLine();

            EditorGUILayout.LabelField("경로 - " + filePath);
        }

        static Assembly assembly = typeof(ImageEditor).Assembly;
        static Type spriteDrawUtility;
        static MethodInfo drawSpriteMethod;
        float previewSize = 200;
        public static void DrawSprite(string nameSpace, string type, string name, string tag, int index, ref float previewSize)
        {
            if (spriteDrawUtility == null)
            {
                spriteDrawUtility = assembly.GetType("UnityEditor.UI.SpriteDrawUtility");

                MethodInfo[] methodInfos = spriteDrawUtility.GetMethods(BindingFlags.Public | BindingFlags.Static);
                for (int i = 0; i < methodInfos.Length; i++)
                {
                    MethodInfo methodInfo = methodInfos[i];
                    ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                    if (parameterInfos.Length == 3)
                    {
                        bool loopContinue = false;
                        for (int k = 0; k < parameterInfos.Length; k++)
                        {
                            ParameterInfo parameterInfo = parameterInfos[k];
                            if ((k == 0 && parameterInfo.ParameterType == typeof(Sprite)) || (k == 1 && parameterInfo.ParameterType == typeof(Rect)) || (k == 2 && parameterInfo.ParameterType == typeof(Color)))
                            {
                                if (k == 2)
                                {
                                    drawSpriteMethod = methodInfo;
                                    return;
                                }
                            }
                            else
                            {
                                loopContinue = true;
                                break;
                            }
                        }

                        if (loopContinue)
                            continue;
                    }
                }

                return;
            }

            Sprite sprite = CustomSpriteRendererBase.GetSprite(type, name, index, nameSpace, tag, true);
            if (sprite == null)
                return;

            CustomInspectorEditor.DrawLine();

            GUILayout.BeginHorizontal();

            previewSize = GUILayout.VerticalSlider(previewSize, 0, 400, GUILayout.Height(400));
            drawSpriteMethod.Invoke(null, new object[] { sprite, EditorGUILayout.GetControlRect(false, previewSize), Color.white });

            GUILayout.EndHorizontal();
        }
    }
}
