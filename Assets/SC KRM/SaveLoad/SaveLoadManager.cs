using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SCKRM.SaveLoad
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SaveLoadAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class GeneralSaveLoadAttribute : SaveLoadAttribute
    {
        public GeneralSaveLoadAttribute()
        {

        }
    }

    public class SaveLoadClass
    {
        public string name { get; }
        public Type type { get; }
        public SaveLoadVariable<PropertyInfo>[] propertyInfos { get; } = new SaveLoadVariable<PropertyInfo>[0];
        public SaveLoadVariable<FieldInfo>[] fieldInfos { get; } = new SaveLoadVariable<FieldInfo>[0];

        public SaveLoadClass(string name, Type type, SaveLoadVariable<PropertyInfo>[] propertyInfos, SaveLoadVariable<FieldInfo>[] fieldInfos)
        {
            this.name = name;
            this.type = type;
            this.propertyInfos = propertyInfos;
            this.fieldInfos = fieldInfos;
        }

        public class SaveLoadVariable<T> where T : MemberInfo
        {
            public T variableInfo { get; }
            public object defaultValue { get; }

            public SaveLoadVariable(T variableInfo, object defaultValue)
            {
                this.variableInfo = variableInfo;
                this.defaultValue = defaultValue;
            }
        }

        public string[] GetVariableNames()
        {
            string[] propertyNames = new string[propertyInfos.Length];
            for (int i = 0; i < propertyInfos.Length; i++)
                propertyNames[i] = propertyInfos[i].variableInfo.Name;

            string[] fieldNames = new string[fieldInfos.Length];
            for (int i = 0; i < fieldInfos.Length; i++)
                fieldNames[i] = fieldInfos[i].variableInfo.Name;

            return propertyNames.Union(fieldNames).ToArray();
        }
    }

    [WikiDescription("세이브 로드를 관리하는 클래스 입니다")]
    public static class SaveLoadManager
    {
        [WikiDescription("캐싱된 세이브 로드 클래스")]
        public static SaveLoadClass[] generalSLCList { get; [Obsolete("It is managed by the InitialLoadManager class. Please do not touch it.", false)] internal set; } = new SaveLoadClass[0];

        [WikiDescription("전부 초기화")]
        public static void InitializeAll<T>(out SaveLoadClass[] result) where T : SaveLoadAttribute
        {
            List<SaveLoadClass> saveLoadClassList = new List<SaveLoadClass>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int assemblysIndex = 0; assemblysIndex < assemblys.Length; assemblysIndex++)
            {
                Type[] types = assemblys[assemblysIndex].GetTypes();
                for (int typesIndex = 0; typesIndex < types.Length; typesIndex++)
                {
                    Type type = types[typesIndex];

                    Initialize<T>(type, out SaveLoadClass result2);

                    if (result2 != null)
                        saveLoadClassList.Add(result2);
                }
            }

            result = saveLoadClassList.ToArray();
        }

        [WikiDescription("초기화")]
        public static void Initialize<T>(Type type, out SaveLoadClass result) where T : SaveLoadAttribute
        {
            if (Attribute.GetCustomAttributes(type, typeof(T)).Length <= 0)
            {
                result = null;
                return;
            }

            #region 경고 및 기본값 저장
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
            List<SaveLoadClass.SaveLoadVariable<PropertyInfo>> propertyInfoList = new List<SaveLoadClass.SaveLoadVariable<PropertyInfo>>();
            List<SaveLoadClass.SaveLoadVariable<FieldInfo>> fieldInfoList = new List<SaveLoadClass.SaveLoadVariable<FieldInfo>>();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                bool ignore = Attribute.GetCustomAttributes(propertyInfo, typeof(JsonIgnoreAttribute)).Length > 0;
                if (Attribute.GetCustomAttributes(propertyInfo, typeof(JsonPropertyAttribute)).Length <= 0 && !ignore)
                    Debug.LogWarning(type.FullName + " " + propertyInfo.PropertyType + " " + propertyInfo.Name + " 에 [JsonProperty] 어트리뷰트가 추가되어있지 않습니다.\n이 변수는 로드되지 않을것입니다.\n무시를 원하신다면 [JsonIgnore] 어트리뷰트를 추가해주세요");
                else if (!ignore)
                    propertyInfoList.Add(new SaveLoadClass.SaveLoadVariable<PropertyInfo>(propertyInfo, propertyInfo.GetValue(propertyInfo.PropertyType)));
            }

            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo fieldInfo = fieldInfos[i];
                bool ignore = Attribute.GetCustomAttributes(fieldInfo, typeof(JsonIgnoreAttribute)).Length > 0;
                if (Attribute.GetCustomAttributes(fieldInfo, typeof(JsonPropertyAttribute)).Length <= 0 && !ignore)
                    Debug.LogWarning(type.FullName + " " + fieldInfo.FieldType + " " + fieldInfo.Name + " 에 [JsonProperty] 어트리뷰트가 추가되어있지 않습니다.\n이 변수는 로드되지 않을것입니다.\n무시를 원하신다면 [JsonIgnore] 어트리뷰트를 추가해주세요");
                else if (!ignore)
                    fieldInfoList.Add(new SaveLoadClass.SaveLoadVariable<FieldInfo>(fieldInfo, fieldInfo.GetValue(fieldInfo.FieldType)));
            }

            result = new SaveLoadClass(type.FullName, type, propertyInfoList.ToArray(), fieldInfoList.ToArray());
            #endregion
        }

        [WikiDescription("모두 저장")]
        public static void SaveAll(SaveLoadClass[] saveLoadClassList, string saveDataPath, bool noExistsCheck = false)
        {
            if (saveLoadClassList == null || saveDataPath == null || saveDataPath == "")
                return;
            else if (!noExistsCheck && !Directory.Exists(saveDataPath))
                Directory.CreateDirectory(saveDataPath);

            for (int i = 0; i < saveLoadClassList.Length; i++)
                Save(saveLoadClassList[i], saveDataPath);
        }

        [WikiDescription("저장")]
        public static void Save(SaveLoadClass saveLoadClass, string saveDataPath)
        {
            if (saveLoadClass == null || saveDataPath == null || saveDataPath == "")
                return;

            JObject jObject = new JObject();
            for (int j = 0; j < saveLoadClass.propertyInfos.Length; j++)
            {
                SaveLoadClass.SaveLoadVariable<PropertyInfo> propertyInfo = saveLoadClass.propertyInfos[j];
                jObject.Add(propertyInfo.variableInfo.Name, JToken.FromObject(propertyInfo.variableInfo.GetValue(propertyInfo.variableInfo.PropertyType)));
            }

            for (int j = 0; j < saveLoadClass.fieldInfos.Length; j++)
            {
                SaveLoadClass.SaveLoadVariable<FieldInfo> fieldInfo = saveLoadClass.fieldInfos[j];
                jObject.Add(fieldInfo.variableInfo.Name, JToken.FromObject(fieldInfo.variableInfo.GetValue(fieldInfo.variableInfo.FieldType)));
            }

            File.WriteAllText(PathTool.Combine(saveDataPath, saveLoadClass.name) + ".json", jObject.ToString());
        }

        [WikiDescription("전부 로드")]
        public static void LoadAll(SaveLoadClass[] saveLoadClassList, string loadDataPath, bool noExistsCheck = false)
        {
            if (saveLoadClassList == null || loadDataPath == null || loadDataPath == "")
                return;
            else if (!noExistsCheck && !Directory.Exists(loadDataPath))
                Directory.CreateDirectory(loadDataPath);

            for (int i = 0; i < saveLoadClassList.Length; i++)
                Load(saveLoadClassList[i], loadDataPath, noExistsCheck);
        }

        [WikiDescription("로드")]
        public static void Load(SaveLoadClass saveLoadClass, string loadDataPath, bool noExistsCheck = false)
        {
            if (saveLoadClass == null || loadDataPath == null || loadDataPath == "")
                return;
            else if (!noExistsCheck && !Directory.Exists(loadDataPath))
                Directory.CreateDirectory(loadDataPath);

            string path = PathTool.Combine(loadDataPath, saveLoadClass.name) + ".json";
            if (!noExistsCheck && !File.Exists(path))
                return;

            #region null 설정
            for (int j = 0; j < saveLoadClass.propertyInfos.Length; j++)
            {
                SaveLoadClass.SaveLoadVariable<PropertyInfo> propertyInfo = saveLoadClass.propertyInfos[j];
                propertyInfo.variableInfo.SetValue(propertyInfo.variableInfo.PropertyType, null);
            }

            for (int j = 0; j < saveLoadClass.fieldInfos.Length; j++)
            {
                SaveLoadClass.SaveLoadVariable<FieldInfo> fieldInfo = saveLoadClass.fieldInfos[j];
                fieldInfo.variableInfo.SetValue(fieldInfo.variableInfo.FieldType, null);
            }
            #endregion

            {
                string json = File.ReadAllText(path);
                try
                {
                    JsonConvert.DeserializeObject(json, saveLoadClass.type);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            #region json을 로드 했는데도 null이면 기본값으로 되돌리기
            for (int j = 0; j < saveLoadClass.propertyInfos.Length; j++)
            {
                SaveLoadClass.SaveLoadVariable<PropertyInfo> propertyInfo = saveLoadClass.propertyInfos[j];
                if (propertyInfo.variableInfo.GetValue(propertyInfo.variableInfo.PropertyType) == null)
                    propertyInfo.variableInfo.SetValue(propertyInfo.variableInfo.PropertyType, propertyInfo.defaultValue);
            }

            for (int j = 0; j < saveLoadClass.fieldInfos.Length; j++)
            {
                SaveLoadClass.SaveLoadVariable<FieldInfo> fieldInfo = saveLoadClass.fieldInfos[j];
                if (fieldInfo.variableInfo.GetValue(fieldInfo.variableInfo.FieldType) == null)
                    fieldInfo.variableInfo.SetValue(fieldInfo.variableInfo.FieldType, fieldInfo.defaultValue);
            }
            #endregion
        }
    }
}