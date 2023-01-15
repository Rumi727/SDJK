using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SCKRM.Renderer;
using SCKRM.Resource;
using System.IO;
using SCKRM.Json;

namespace SCKRM.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CustomSpriteRendererBase), true)]
    public class CustomAllSpriteRendererEditor : CustomInspectorEditor
    {
        CustomSpriteRendererBase editor;

        protected override void OnEnable()
        {
            base.OnEnable();
            editor = (CustomSpriteRendererBase)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();

            editor.nameSpace = UsePropertyAndDrawNameSpace("_nameSpace", "네임스페이스", editor.nameSpace);

            string[] types = ResourceManager.GetSpriteTypes(editor.nameSpace);
            if (types == null)
                return;

            editor.type = UsePropertyAndDrawStringArray("_type", "타입", editor.type, types);

            string[] spriteKeys = ResourceManager.GetSpriteKeys(editor.type, editor.nameSpace);
            if (spriteKeys == null)
                return;

            editor.path = UsePropertyAndDrawStringArray("_path", "이름", editor.path, spriteKeys);

            EditorGUILayout.Space();

            UseProperty("_index", "스프라이트 인덱스");

            if (Kernel.isPlaying)
                return;

            string nameSpace = editor.nameSpace;
            if (nameSpace == null || nameSpace == "")
                nameSpace = ResourceManager.defaultNameSpace;

            string typePath = PathTool.Combine(ResourceManager.texturePath.Replace("%NameSpace%", nameSpace), editor.type);
            string filePath = PathTool.Combine(typePath, editor.path);
            string typeAllPath = PathTool.Combine(Kernel.streamingAssetsPath, typePath);
            ResourceManager.FileExtensionExists(PathTool.Combine(Kernel.streamingAssetsPath, filePath), out string fileAllPath, ResourceManager.textureExtension);

            if (Directory.Exists(typeAllPath) && editor.type != null && editor.type != "")
            {
                TextureMetaData textureMetaData = JsonManager.JsonRead<TextureMetaData>(typeAllPath + ".json", true);
                if (textureMetaData == null)
                    textureMetaData = new TextureMetaData();

                DrawLine();

                textureMetaData.filterMode = (FilterMode)EditorGUILayout.EnumPopup("필터 모드", textureMetaData.filterMode);
                textureMetaData.mipmapUse = EditorGUILayout.Toggle("밉맵 사용", textureMetaData.mipmapUse);
                textureMetaData.compressionType = (TextureMetaData.CompressionType)EditorGUILayout.EnumPopup("압축 타입", textureMetaData.compressionType);

                Texture2D texture = ResourceManager.GetTexture(fileAllPath, true, textureMetaData, TextureFormat.Alpha8);
                if (texture != null && editor.path != null && editor.path != "")
                {
                    DrawLine();

                    List<Resource.SpriteMetaData> spriteMetaDatas = JsonManager.JsonRead<List<Resource.SpriteMetaData>>(fileAllPath + ".json", true);
                    if (spriteMetaDatas == null)
                        spriteMetaDatas = new List<Resource.SpriteMetaData>();

                    if (editor.index < spriteMetaDatas.Count)
                    {
                        Resource.SpriteMetaData spriteMetaData = spriteMetaDatas[editor.index];

                        spriteMetaData.RectMinMax(texture.width, texture.height);
                        spriteMetaData.PixelsPreUnitMinSet();

                        spriteMetaData.pivot = EditorGUILayout.Vector2Field("중심", spriteMetaData.pivot);
                        EditorGUILayout.BeginHorizontal();
                        spriteMetaData.rect = EditorGUILayout.Vector4Field("자르기", spriteMetaData.rect);
                        EditorGUILayout.EndHorizontal();
                        spriteMetaData.border = EditorGUILayout.Vector4Field("가장자리", spriteMetaData.border);

                        EditorGUILayout.Space();

                        spriteMetaData.pixelsPerUnit = EditorGUILayout.FloatField("1 픽셀 크기", spriteMetaData.pixelsPerUnit);

                        DrawLine();

                        if (GUILayout.Button("스프라이트 지우기"))
                            spriteMetaDatas.RemoveAt(editor.index);
                    }
                    else if (GUILayout.Button("스프라이트 만들기"))
                    {
                        Resource.SpriteMetaData spriteMetaData = new Resource.SpriteMetaData();
                        spriteMetaData.RectMinMax(texture.width, texture.height);
                        spriteMetaData.PixelsPreUnitMinSet();
                        spriteMetaDatas.Add(spriteMetaData);
                    }

                    GUI.enabled = true;

                    if (GUI.changed || GUILayout.Button("새로고침"))
                    {
                        EditorUtility.SetDirty(target);

                        File.WriteAllText(typeAllPath + ".json", JsonManager.ObjectToJson(textureMetaData));
                        File.WriteAllText(fileAllPath + ".json", JsonManager.ObjectToJson(spriteMetaDatas));

                        AssetDatabase.Refresh();

                        if (editor.enabled)
                            editor.Refresh();
                    }
                }
                else if (GUI.changed || GUILayout.Button("새로고침"))
                {
                    EditorUtility.SetDirty(target);

                    File.WriteAllText(typeAllPath + ".json", JsonManager.ObjectToJson(textureMetaData));

                    AssetDatabase.Refresh();

                    if (editor.enabled)
                        editor.Refresh();
                }

                DestroyImmediate(texture);
            }
            else if (GUI.changed)
            {
                EditorUtility.SetDirty(target);

                if (editor.enabled)
                    editor.Refresh();
            }

            DrawLine();

            EditorGUILayout.LabelField("경로 - " + filePath);
        }
    }
}