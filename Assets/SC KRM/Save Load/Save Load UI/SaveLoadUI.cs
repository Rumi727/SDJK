using ExtendedNumerics;
using SCKRM.Json;
using SCKRM.Object;
using SCKRM.Renderer;
using SCKRM.UI;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using UnityEngine;

namespace SCKRM.SaveLoad.UI
{
    public sealed class SaveLoadUI : UIObjectPooling
    {
        [SerializeField, Tooltip("SaveLoadManager.generalSLCList 리스트를 사용합니다")] bool _autoRefresh = false; public bool autoRefresh { get => _autoRefresh; set => _autoRefresh = value; }
        [SerializeField] string _saveLoadClassName = ""; public string saveLoadClassName { get => _saveLoadClassName; set => _saveLoadClassName = value; }

        [SerializeField] bool _isLineShow = true; public bool isLineShow { get => _isLineShow; set => _isLineShow = value; }

        [SerializeField] string _titlePrefab = "save_load.ui.title"; public string titlePrefab { get => _titlePrefab; set => _titlePrefab = value; }
        [SerializeField] string _linePrefab = "save_load.ui.line"; public string linePrefab { get => _linePrefab; set => _linePrefab = value; }
        [SerializeField] string _spacePrefab = "save_load.ui.space"; public string spacePrefab { get => _spacePrefab; set => _spacePrefab = value; }

        [SerializeField] SaveLoadUIPrefab _saveLoadUIPrefab = new SaveLoadUIPrefab(); public SaveLoadUIPrefab saveLoadUIPrefab { get => _saveLoadUIPrefab; }

        protected override void Awake()
        {
            if (autoRefresh)
                Refresh(SaveLoadManager.generalSLCList);
        }

        List<IObjectPooling> objectPoolingBases = new List<IObjectPooling>();
        public void Refresh(params SaveLoadClass[] slcs)
        {
            for (int i = 0; i < objectPoolingBases.Count; i++)
                objectPoolingBases[i].Remove();

            objectPoolingBases.Clear();

            SaveLoadClass slc = null;
            for (int i = 0; i < slcs.Length; i++)
            {
                SaveLoadClass tempSlc = slcs[i];
                if (tempSlc.name == saveLoadClassName)
                    slc = tempSlc;
            }

            if (slc == null)
                return;

            Attribute[] attributes = Attribute.GetCustomAttributes(slc.type, typeof(SaveLoadUIAttribute));
            if (attributes == null || attributes.Length <= 0)
                return;

            SaveLoadUIAttribute saveLoadUIAttribute = (SaveLoadUIAttribute)attributes[0];
            NameSpacePathPair titleName = saveLoadUIAttribute.name;

            SaveLoadUITitle title = ObjectCreate<SaveLoadUITitle>(titlePrefab);
            title.customTextMeshProRenderer.nameSpacePathPair = titleName;
            title.customTextMeshProRenderer.Refresh();

            for (int j = 0; j < slc.propertyInfos.Length; j++)
                FieldCreate(slc.propertyInfos[j]);

            for (int j = 0; j < slc.fieldInfos.Length; j++)
                FieldCreate(slc.fieldInfos[j]);

            if (!isLineShow)
            {
                ObjectCreate<MonoBehaviour>(spacePrefab);
                ObjectCreate<MonoBehaviour>(linePrefab);
                ObjectCreate<MonoBehaviour>(spacePrefab);
            }

            void FieldCreate<T>(SaveLoadClass.SaveLoadVariable<T> slv) where T : MemberInfo
            {
                T memberInfo = slv.variableInfo;
                if (Attribute.GetCustomAttribute(memberInfo, typeof(SaveLoadUIIgnoreAttribute)) != null)
                    return;

                Type type;
                string name = memberInfo.Name;
                if (memberInfo is PropertyInfo)
                    type = ((PropertyInfo)(MemberInfo)memberInfo).PropertyType;
                else if (memberInfo is FieldInfo)
                    type = ((FieldInfo)(MemberInfo)memberInfo).FieldType;
                else
                    return;

                SaveLoadUIBase saveLoadUIBase;
                SaveLoadUIConfigBaseAttribute saveLoadUIConfigBase;

                #region Type Method Invoke
                if (type == typeof(char)
                    || type == typeof(string)
                    || type == typeof(byte)
                    || type == typeof(sbyte)
                    || type == typeof(short)
                    || type == typeof(int)
                    || type == typeof(int)
                    || type == typeof(ushort)
                    || type == typeof(ulong)
                    || type == typeof(float)
                    || type == typeof(double)
                    || type == typeof(decimal)
                    || type == typeof(BigInteger)
                    || type == typeof(BigDecimal))
                    Text();
                else if (type == typeof(bool))
                    Toggle();
                else if (type == typeof(JColor) || type == typeof(JColor32))
                    Color();
                else
                    return;
                #endregion

                saveLoadUIBase.saveLoadClassName = slc.name;
                saveLoadUIBase.variableName = name;

                if (saveLoadUIConfigBase != null)
                {
                    saveLoadUIBase.nameTextRenderer.nameSpacePathPair = saveLoadUIConfigBase.name;
                    saveLoadUIBase.tooltip.nameSpacePathPair = saveLoadUIConfigBase.tooltip;

                    saveLoadUIBase.nameTextRenderer.Refresh();

                    saveLoadUIBase.roundingDigits = saveLoadUIConfigBase.roundingDigits;
                    saveLoadUIBase.hotkeyToDisplays = saveLoadUIConfigBase.hotkeyToDisplays;
                }

                saveLoadUIBase.Refresh(slcs);

                #region Type Method
                void Text()
                {
                    if (type != typeof(char) && type != typeof(string))
                    {
                        SaveLoadUISliderConfigAttribute sliderConfig = (SaveLoadUISliderConfigAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(SaveLoadUISliderConfigAttribute));
                        saveLoadUIConfigBase = sliderConfig;

                        if (sliderConfig == null)
                        {
                            InputField();
                            return;
                        }

                        SaveLoadUISlider slider = ObjectCreate<SaveLoadUISlider>(saveLoadUIPrefab.slider);
                        saveLoadUIBase = slider;

                        slider.mouseSensitivity = sliderConfig.mouseSensitivity;

                        slider.invokeLock = true;
                        slider.slider.minValue = sliderConfig.min;
                        slider.slider.maxValue = sliderConfig.max;
                        slider.invokeLock = false;
                    }
                    else
                        InputField();

                    void InputField()
                    {
                        SaveLoadUIInputField inputField = ObjectCreate<SaveLoadUIInputField>(saveLoadUIPrefab.inputField);
                        saveLoadUIBase = inputField;

                        SaveLoadUIInputFieldConfigAttribute inputFieldConfig = (SaveLoadUIInputFieldConfigAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(SaveLoadUIInputFieldConfigAttribute));
                        saveLoadUIConfigBase = inputFieldConfig;

                        if (inputFieldConfig == null)
                            return;

                        inputField.mouseSensitivity = inputFieldConfig.mouseSensitivity;
                    }
                }

                void Color()
                {
                    SaveLoadUIColorPicker colorPicker = ObjectCreate<SaveLoadUIColorPicker>(saveLoadUIPrefab.colorPicker);
                    saveLoadUIBase = colorPicker;

                    SaveLoadUIColorPickerConfigAttribute colorPickerConfig = (SaveLoadUIColorPickerConfigAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(SaveLoadUIColorPickerConfigAttribute));
                    saveLoadUIConfigBase = colorPickerConfig;

                    if (colorPickerConfig == null)
                        return;

                    colorPicker.colorPicker.Setup.ShowAlpha = colorPickerConfig.alphaShow;
                }

                void Toggle()
                {
                    SaveLoadUIToggle toggle = ObjectCreate<SaveLoadUIToggle>(saveLoadUIPrefab.toggle);
                    saveLoadUIBase = toggle;

                    SaveLoadUIToggleConfigAttribute toggleConfig = (SaveLoadUIToggleConfigAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(SaveLoadUIToggleConfigAttribute));
                    saveLoadUIConfigBase = toggleConfig;

                    if (toggleConfig == null)
                        return;
                }
                #endregion
            }

            T ObjectCreate<T>(string key) where T : MonoBehaviour
            {
                (MonoBehaviour monoBehaviour, IObjectPooling objectPooling) = ObjectPoolingSystem.ObjectCreate(key, transform);
                objectPoolingBases.Add(objectPooling);

                return (T)monoBehaviour;
            }
        }

        [Serializable]
        public sealed class SaveLoadUIPrefab
        {
            public string colorPicker = "save_load.ui.color_picker";
            public string dropdown = "save_load.ui.dropdown";
            public string inputField = "save_load.ui.input_field";
            public string slider = "save_load.ui.slider";
            public string toggle = "save_load.ui.toggle";
        }
    }
}
