using Cysharp.Threading.Tasks;
using SCKRM.Cursor;
using SCKRM.Input;
using SCKRM.Json;
using SCKRM.Renderer;
using SCKRM.SaveLoad;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SCKRM.UI.Setting
{
    public abstract class Setting : UI
    {
        public static Dictionary<string, Setting> settingInstance { get; } = new Dictionary<string, Setting>();
        public static Dictionary<string, List<Setting>> settingInstances { get; } = new Dictionary<string, List<Setting>>();



        [SerializeField] string _saveLoadClassName = ""; public string saveLoadClassName { get => _saveLoadClassName; set => _saveLoadClassName = value; }
        [SerializeField] string _variableName = ""; public string variableName { get => _variableName; set => _variableName = value; }

        [SerializeField] int _roundingDigits = 2; public int roundingDigits { get => _roundingDigits; set => _roundingDigits = value; }
        [SerializeField] string[] _hotkeyToDisplay = new string[0]; public string[] hotkeyToDisplays { get => _hotkeyToDisplay; set => _hotkeyToDisplay = value; }

        [SerializeField] CanvasGroup _resetButton; public CanvasGroup resetButton { get => _resetButton; set => _resetButton = value; }
        [SerializeField] RectTransform _nameText; public RectTransform nameText { get => _nameText; set => _nameText = value; }
        [SerializeField] CustomTextRendererBase _nameTextRenderer; public CustomTextRendererBase nameTextRenderer { get => _nameTextRenderer; set => _nameTextRenderer = value; }



        public VariableType variableType { get; private set; } = VariableType.String;

        public object defaultValue { get; private set; } = null;
        public Type type { get; private set; } = null;
        public PropertyInfo propertyInfo { get; private set; } = null;
        public FieldInfo fieldInfo { get; private set; } = null;

        public bool isDefault { get; protected set; } = true;

        public bool isLoad { get; private set; } = false;



        /// <returns>is Cancel?</returns>
        new protected virtual async UniTask<bool> Awake()
        {
            if (await UniTask.WaitUntil(() => InitialLoadManager.isInitialLoadEnd, PlayerLoopTiming.Initialization, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow())
                return true;

            foreach (var variableType in SaveLoadManager.generalSLCList)
            {
                if (variableType.name == saveLoadClassName)
                {
                    foreach (var propertyInfo in variableType.propertyInfos)
                    {
                        if (propertyInfo.variableInfo.Name == variableName)
                        {
                            type = propertyInfo.variableInfo.PropertyType;

                            defaultValue = propertyInfo.defaultValue;
                            this.propertyInfo = propertyInfo.variableInfo;

                            break;
                        }
                    }

                    foreach (var fieldInfo in variableType.fieldInfos)
                    {
                        if (fieldInfo.variableInfo.Name == variableName)
                        {
                            type = fieldInfo.variableInfo.FieldType;

                            defaultValue = fieldInfo.defaultValue;
                            this.fieldInfo = fieldInfo.variableInfo;

                            break;
                        }
                    }
                }
            }

            if (type == typeof(char))
                variableType = VariableType.Char;
            else if (type == typeof(string))
                variableType = VariableType.String;
            else if (type == typeof(bool))
                variableType = VariableType.Bool;
            else if (type == typeof(byte))
                variableType = VariableType.Byte;
            else if (type == typeof(sbyte))
                variableType = VariableType.Sbyte;
            else if (type == typeof(short))
                variableType = VariableType.Short;
            else if (type == typeof(int))
                variableType = VariableType.Int;
            else if (type == typeof(int))
                variableType = VariableType.Long;
            else if (type == typeof(ushort))
                variableType = VariableType.Ushort;
            else if (type == typeof(uint))
                variableType = VariableType.Uint;
            else if (type == typeof(ulong))
                variableType = VariableType.Ulong;
            else if (type == typeof(float))
                variableType = VariableType.Float;
            else if (type == typeof(double))
                variableType = VariableType.Double;
            else if (type == typeof(decimal))
                variableType = VariableType.Decimal;
            else if (type == typeof(JColor))
                variableType = VariableType.JColor;
            else if (type == typeof(JColor32))
                variableType = VariableType.JColor32;

            isLoad = true;

            string key = saveLoadClassName + "." + variableName;
            if (!settingInstances.ContainsKey(key))
                settingInstances.Add(key, new List<Setting>() { this });
            else
                settingInstances[key].Add(this);

            if (!settingInstance.ContainsKey(key))
                settingInstance.Add(key, this);


            return false;
        }

        protected virtual void Update()
        {
            if (!InitialLoadManager.isInitialLoadEnd || !isLoad)
                return;

            if (isDefault)
            {
                resetButton.interactable = false;
                nameText.offsetMin = nameText.offsetMin.Lerp(new Vector2(0, nameText.offsetMin.y), 0.2f * Kernel.fpsUnscaledDeltaTime);
                resetButton.alpha = resetButton.alpha.Lerp(0, 0.2f * Kernel.fpsUnscaledDeltaTime);

                if (resetButton.alpha < 0.01f)
                    resetButton.alpha = 0;
            }
            else
            {
                resetButton.interactable = true;
                nameText.offsetMin = nameText.offsetMin.Lerp(new Vector2(22, nameText.offsetMin.y), 0.2f * Kernel.fpsUnscaledDeltaTime);
                resetButton.alpha = resetButton.alpha.Lerp(1, 0.2f * Kernel.fpsUnscaledDeltaTime);

                if (resetButton.alpha > 0.99f)
                    resetButton.alpha = 1;
            }
        }

        public virtual void SetDefault()
        {
            if (propertyInfo != null)
                propertyInfo.SetValue(null, defaultValue);
            else if (fieldInfo != null)
                fieldInfo.SetValue(null, defaultValue);
        }

        public virtual object GetValue()
        {
            if (propertyInfo != null)
            {
                return variableType switch
                {
                    VariableType.Float => ((float)propertyInfo.GetValue(null)).Round(roundingDigits),
                    VariableType.Double => ((double)propertyInfo.GetValue(null)).Round(roundingDigits),
                    VariableType.Decimal => ((decimal)propertyInfo.GetValue(null)).Round(roundingDigits),
                    _ => propertyInfo.GetValue(null)
                };
            }
            else if (fieldInfo != null)
            {
                return variableType switch
                {
                    VariableType.Float => ((float)fieldInfo.GetValue(null)).Round(roundingDigits),
                    VariableType.Double => ((double)fieldInfo.GetValue(null)).Round(roundingDigits),
                    VariableType.Decimal => ((decimal)fieldInfo.GetValue(null)).Round(roundingDigits),
                    _ => fieldInfo.GetValue(null)
                };
            }

            return null;
        }

        public virtual int GetValueInt()
        {
            if (propertyInfo != null)
            {
                switch (variableType)
                {
                    case VariableType.Char:
                        return int.Parse(((char)propertyInfo.GetValue(null)).ToString());
                    case VariableType.String:
                        return int.Parse((string)propertyInfo.GetValue(null));
                    case VariableType.Bool:
                        return (bool)propertyInfo.GetValue(null) ? 1 : 0;
                    case VariableType.Byte:
                        return (byte)propertyInfo.GetValue(null);
                    case VariableType.Sbyte:
                        return (sbyte)propertyInfo.GetValue(null);
                    case VariableType.Short:
                        return (short)propertyInfo.GetValue(null);
                    case VariableType.Int:
                        return (int)propertyInfo.GetValue(null);
                    case VariableType.Long:
                        return (int)(long)propertyInfo.GetValue(null);
                    case VariableType.Ushort:
                        return (ushort)propertyInfo.GetValue(null);
                    case VariableType.Uint:
                        return (int)(uint)propertyInfo.GetValue(null);
                    case VariableType.Ulong:
                        return (int)(ulong)propertyInfo.GetValue(null);
                    case VariableType.Float:
                        return (int)(float)propertyInfo.GetValue(null);
                    case VariableType.Double:
                        return (int)(double)propertyInfo.GetValue(null);
                    case VariableType.Decimal:
                        return (int)(decimal)propertyInfo.GetValue(null);
                }
            }
            else if (fieldInfo != null)
            {
                switch (variableType)
                {
                    case VariableType.Char:
                        return int.Parse(((char)fieldInfo.GetValue(null)).ToString());
                    case VariableType.String:
                        return int.Parse((string)fieldInfo.GetValue(null));
                    case VariableType.Bool:
                        return (bool)fieldInfo.GetValue(null) ? 1 : 0;
                    case VariableType.Byte:
                        return (byte)fieldInfo.GetValue(null);
                    case VariableType.Sbyte:
                        return (sbyte)fieldInfo.GetValue(null);
                    case VariableType.Short:
                        return (short)fieldInfo.GetValue(null);
                    case VariableType.Int:
                        return (int)fieldInfo.GetValue(null);
                    case VariableType.Long:
                        return (int)(long)fieldInfo.GetValue(null);
                    case VariableType.Ushort:
                        return (ushort)fieldInfo.GetValue(null);
                    case VariableType.Uint:
                        return (int)(uint)fieldInfo.GetValue(null);
                    case VariableType.Ulong:
                        return (int)(ulong)fieldInfo.GetValue(null);
                    case VariableType.Float:
                        return (int)(float)fieldInfo.GetValue(null);
                    case VariableType.Double:
                        return (int)(double)fieldInfo.GetValue(null);
                    case VariableType.Decimal:
                        return (int)(decimal)fieldInfo.GetValue(null);
                }
            }

            return 0;
        }

        public virtual float GetValueFloat()
        {
            if (propertyInfo != null)
            {
                switch (variableType)
                {
                    case VariableType.Char:
                        return float.Parse(((char)propertyInfo.GetValue(null)).ToString());
                    case VariableType.String:
                        return float.Parse((string)propertyInfo.GetValue(null));
                    case VariableType.Bool:
                        return (bool)propertyInfo.GetValue(null) ? 1 : 0;
                    case VariableType.Byte:
                        return (byte)propertyInfo.GetValue(null);
                    case VariableType.Sbyte:
                        return (sbyte)propertyInfo.GetValue(null);
                    case VariableType.Short:
                        return (short)propertyInfo.GetValue(null);
                    case VariableType.Int:
                        return (int)propertyInfo.GetValue(null);
                    case VariableType.Long:
                        return (long)propertyInfo.GetValue(null);
                    case VariableType.Ushort:
                        return (ushort)propertyInfo.GetValue(null);
                    case VariableType.Uint:
                        return (uint)propertyInfo.GetValue(null);
                    case VariableType.Ulong:
                        return (ulong)propertyInfo.GetValue(null);
                    case VariableType.Float:
                        return (float)propertyInfo.GetValue(null);
                    case VariableType.Double:
                        return (float)((double)propertyInfo.GetValue(null)).Round(roundingDigits);
                    case VariableType.Decimal:
                        return (float)((decimal)propertyInfo.GetValue(null)).Round(roundingDigits);
                }
            }
            else if (fieldInfo != null)
            {
                switch (variableType)
                {
                    case VariableType.Char:
                        return float.Parse(((char)fieldInfo.GetValue(null)).ToString());
                    case VariableType.String:
                        return float.Parse((string)fieldInfo.GetValue(null));
                    case VariableType.Bool:
                        return (bool)fieldInfo.GetValue(null) ? 1 : 0;
                    case VariableType.Byte:
                        return (byte)fieldInfo.GetValue(null);
                    case VariableType.Sbyte:
                        return (sbyte)fieldInfo.GetValue(null);
                    case VariableType.Short:
                        return (short)fieldInfo.GetValue(null);
                    case VariableType.Int:
                        return (int)fieldInfo.GetValue(null);
                    case VariableType.Long:
                        return (long)fieldInfo.GetValue(null);
                    case VariableType.Ushort:
                        return (ushort)fieldInfo.GetValue(null);
                    case VariableType.Uint:
                        return (uint)fieldInfo.GetValue(null);
                    case VariableType.Ulong:
                        return (ulong)fieldInfo.GetValue(null);
                    case VariableType.Float:
                        return (float)fieldInfo.GetValue(null);
                    case VariableType.Double:
                        return (float)((double)fieldInfo.GetValue(null)).Round(roundingDigits);
                    case VariableType.Decimal:
                        return (float)((decimal)fieldInfo.GetValue(null)).Round(roundingDigits);
                }
            }

            return 0;
        }

        public virtual void SaveValue(object value)
        {
            if (propertyInfo != null)
            {
                switch (variableType)
                {
                    case VariableType.Float:
                        propertyInfo.SetValue(null, ((float)value).Round(roundingDigits));
                        break;
                    case VariableType.Double:
                        propertyInfo.SetValue(null, ((double)value).Round(roundingDigits));
                        break;
                    case VariableType.Decimal:
                        propertyInfo.SetValue(null, ((decimal)value).Round(roundingDigits));
                        break;
                    default:
                        propertyInfo.SetValue(null, value);
                        break;
                }
            }
            else if (fieldInfo != null)
            {
                switch (variableType)
                {
                    case VariableType.Float:
                        fieldInfo.SetValue(null, ((float)value).Round(roundingDigits));
                        break;
                    case VariableType.Double:
                        fieldInfo.SetValue(null, ((double)value).Round(roundingDigits));
                        break;
                    case VariableType.Decimal:
                        fieldInfo.SetValue(null, ((decimal)value).Round(roundingDigits));
                        break;
                    default:
                        fieldInfo.SetValue(null, value);
                        break;
                }
            }
        }


        public virtual void SaveValueFloat(float value)
        {
            value = value.Round(roundingDigits);

            if (propertyInfo != null)
            {
                switch (variableType)
                {
                    case VariableType.Char:
                        throw new ArgumentException();
                    case VariableType.String:
                        propertyInfo.SetValue(null, value.ToString());
                        break;
                    case VariableType.Byte:
                        propertyInfo.SetValue(null, (byte)value);
                        break;
                    case VariableType.Sbyte:
                        propertyInfo.SetValue(null, (sbyte)value);
                        break;
                    case VariableType.Short:
                        propertyInfo.SetValue(null, (short)value);
                        break;
                    case VariableType.Int:
                        propertyInfo.SetValue(null, (int)value);
                        break;
                    case VariableType.Long:
                        propertyInfo.SetValue(null, (long)value);
                        break;
                    case VariableType.Ushort:
                        propertyInfo.SetValue(null, (ushort)value);
                        break;
                    case VariableType.Uint:
                        propertyInfo.SetValue(null, (uint)value);
                        break;
                    case VariableType.Ulong:
                        propertyInfo.SetValue(null, (ulong)value);
                        break;
                    case VariableType.Float:
                        propertyInfo.SetValue(null, value);
                        break;
                    case VariableType.Double:
                        propertyInfo.SetValue(null, (double)value);
                        break;
                    case VariableType.Decimal:
                        propertyInfo.SetValue(null, (decimal)value);
                        break;
                }
            }
            else if (fieldInfo != null)
            {
                switch (variableType)
                {
                    case VariableType.Char:
                        throw new ArgumentException();
                    case VariableType.String:
                        fieldInfo.SetValue(null, value.ToString());
                        break;
                    case VariableType.Byte:
                        fieldInfo.SetValue(null, (byte)value);
                        break;
                    case VariableType.Sbyte:
                        fieldInfo.SetValue(null, (sbyte)value);
                        break;
                    case VariableType.Short:
                        fieldInfo.SetValue(null, (short)value);
                        break;
                    case VariableType.Int:
                        fieldInfo.SetValue(null, (int)value);
                        break;
                    case VariableType.Long:
                        fieldInfo.SetValue(null, (long)value);
                        break;
                    case VariableType.Ushort:
                        fieldInfo.SetValue(null, (ushort)value);
                        break;
                    case VariableType.Uint:
                        fieldInfo.SetValue(null, (uint)value);
                        break;
                    case VariableType.Ulong:
                        fieldInfo.SetValue(null, (ulong)value);
                        break;
                    case VariableType.Float:
                        fieldInfo.SetValue(null, value);
                        break;
                    case VariableType.Double:
                        fieldInfo.SetValue(null, (double)value);
                        break;
                    case VariableType.Decimal:
                        fieldInfo.SetValue(null, (decimal)value);
                        break;
                }
            }
        }

        public virtual void SaveStringValue(string value)
        {
            try
            {
                if (propertyInfo != null)
                {
                    switch (variableType)
                    {
                        case VariableType.Char:
                            if (value.Length > 0)
                                propertyInfo.SetValue(null, value[0]);
                            else
                                propertyInfo.SetValue(null, char.MinValue);
                            break;
                        case VariableType.String:
                            propertyInfo.SetValue(null, value);
                            break;
                        case VariableType.Byte:
                            propertyInfo.SetValue(null, byte.Parse(value));
                            break;
                        case VariableType.Sbyte:
                            propertyInfo.SetValue(null, sbyte.Parse(value));
                            break;
                        case VariableType.Short:
                            propertyInfo.SetValue(null, short.Parse(value));
                            break;
                        case VariableType.Int:
                            propertyInfo.SetValue(null, int.Parse(value));
                            break;
                        case VariableType.Long:
                            propertyInfo.SetValue(null, long.Parse(value));
                            break;
                        case VariableType.Ushort:
                            propertyInfo.SetValue(null, ushort.Parse(value));
                            break;
                        case VariableType.Uint:
                            propertyInfo.SetValue(null, uint.Parse(value));
                            break;
                        case VariableType.Ulong:
                            propertyInfo.SetValue(null, ulong.Parse(value));
                            break;
                        case VariableType.Float:
                            propertyInfo.SetValue(null, float.Parse(value).Round(roundingDigits));
                            break;
                        case VariableType.Double:
                            propertyInfo.SetValue(null, double.Parse(value).Round(roundingDigits));
                            break;
                        case VariableType.Decimal:
                            propertyInfo.SetValue(null, decimal.Parse(value).Round(roundingDigits));
                            break;
                    }
                }
                else if (fieldInfo != null)
                {
                    switch (variableType)
                    {
                        case VariableType.Char:
                            if (value.Length > 0)
                                fieldInfo.SetValue(null, value[0]);
                            else
                                fieldInfo.SetValue(null, char.MinValue);
                            break;
                        case VariableType.String:
                            fieldInfo.SetValue(null, value);
                            break;
                        case VariableType.Byte:
                            fieldInfo.SetValue(null, byte.Parse(value));
                            break;
                        case VariableType.Sbyte:
                            fieldInfo.SetValue(null, sbyte.Parse(value));
                            break;
                        case VariableType.Short:
                            fieldInfo.SetValue(null, short.Parse(value));
                            break;
                        case VariableType.Int:
                            fieldInfo.SetValue(null, int.Parse(value));
                            break;
                        case VariableType.Long:
                            fieldInfo.SetValue(null, long.Parse(value));
                            break;
                        case VariableType.Ushort:
                            fieldInfo.SetValue(null, ushort.Parse(value));
                            break;
                        case VariableType.Uint:
                            fieldInfo.SetValue(null, uint.Parse(value));
                            break;
                        case VariableType.Ulong:
                            fieldInfo.SetValue(null, ulong.Parse(value));
                            break;
                        case VariableType.Float:
                            fieldInfo.SetValue(null, float.Parse(value).Round(roundingDigits));
                            break;
                        case VariableType.Double:
                            fieldInfo.SetValue(null, double.Parse(value).Round(roundingDigits));
                            break;
                        case VariableType.Decimal:
                            fieldInfo.SetValue(null, decimal.Parse(value).Round(roundingDigits));
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void SettingInfoInvoke(NameSpacePathPair value) => SettingInfoManager.Show(new NameSpacePathPair(nameTextRenderer.nameSpace, nameTextRenderer.path), value, hotkeyToDisplays);
        public abstract void ScriptOnValueChanged(bool settingInfoInvoke = true);



        public enum VariableType
        {
            Char,
            String,
            Bool,
            Byte,
            Sbyte,
            Short,
            Int,
            Long,
            Ushort,
            Uint,
            Ulong,
            Float,
            Double,
            Decimal,
            JColor,
            JColor32
        }
    }

    public abstract class SettingDrag : Setting
    {
        [SerializeField] float _mouseSensitivity = 1; public float mouseSensitivity { get => _mouseSensitivity; set => _mouseSensitivity = value; }

        public virtual void OnDrag()
        {
#if UNITY_STANDALONE_WIN
            Vector2Int mousePos = CursorManager.GetCursorPosition();
            if (mousePos.x < 10)
                CursorManager.SetCursorPosition(ScreenManager.currentResolution.width - 10, mousePos.y);
            else if (mousePos.x > ScreenManager.currentResolution.width - 10)
                CursorManager.SetCursorPosition(10, mousePos.y);

            if (mousePos.y < 10)
                CursorManager.SetCursorPosition(mousePos.x, ScreenManager.currentResolution.height - 10);
            else if (mousePos.y > ScreenManager.currentResolution.height - 10)
                CursorManager.SetCursorPosition(mousePos.x, 10);
#endif

            Vector2 mouseDeltaVector = InputManager.GetMouseDelta(false, "all");
            float mouseDelta = mouseDeltaVector.magnitude * mouseSensitivity;

            if ((mouseDeltaVector.x + mouseDeltaVector.y) * 0.5f < 0)
                mouseDelta *= -1;

            if (propertyInfo != null)
            {
                object value = propertyInfo.GetValue(null);

                if (variableType == VariableType.Byte)
                    propertyInfo.SetValue(null, (byte)((byte)value + mouseDelta));
                if (variableType == VariableType.Sbyte)
                    propertyInfo.SetValue(null, (sbyte)((sbyte)value + mouseDelta));
                if (variableType == VariableType.Short)
                    propertyInfo.SetValue(null, (short)((short)value + mouseDelta));
                if (variableType == VariableType.Int)
                    propertyInfo.SetValue(null, (int)((int)value + mouseDelta));
                if (variableType == VariableType.Long)
                    propertyInfo.SetValue(null, (long)((long)value + mouseDelta));
                if (variableType == VariableType.Ushort)
                    propertyInfo.SetValue(null, (ushort)((ushort)value + mouseDelta));
                if (variableType == VariableType.Uint)
                    propertyInfo.SetValue(null, (uint)((uint)value + mouseDelta));
                if (variableType == VariableType.Ulong)
                    propertyInfo.SetValue(null, (ulong)((ulong)value + mouseDelta));
                if (variableType == VariableType.Float)
                    propertyInfo.SetValue(null, ((float)value + mouseDelta).Round(roundingDigits));
                if (variableType == VariableType.Double)
                    propertyInfo.SetValue(null, ((double)value + mouseDelta).Round(roundingDigits));
                if (variableType == VariableType.Decimal)
                    propertyInfo.SetValue(null, ((decimal)value + (decimal)mouseDelta).Round(roundingDigits));
            }
            else if (fieldInfo != null)
            {
                object value2 = fieldInfo.GetValue(null);

                if (variableType == VariableType.Byte)
                    fieldInfo.SetValue(null, (byte)((byte)value2 + mouseDelta));
                if (variableType == VariableType.Sbyte)
                    fieldInfo.SetValue(null, (sbyte)((sbyte)value2 + mouseDelta));
                if (variableType == VariableType.Short)
                    fieldInfo.SetValue(null, (short)((short)value2 + mouseDelta));
                if (variableType == VariableType.Int)
                    fieldInfo.SetValue(null, (int)((int)value2 + mouseDelta));
                if (variableType == VariableType.Long)
                    fieldInfo.SetValue(null, (long)((long)value2 + mouseDelta));
                if (variableType == VariableType.Ushort)
                    fieldInfo.SetValue(null, (ushort)((ushort)value2 + mouseDelta));
                if (variableType == VariableType.Uint)
                    fieldInfo.SetValue(null, (uint)((uint)value2 + mouseDelta));
                if (variableType == VariableType.Ulong)
                    fieldInfo.SetValue(null, (ulong)((ulong)value2 + mouseDelta));
                if (variableType == VariableType.Float)
                    fieldInfo.SetValue(null, ((float)value2 + mouseDelta).Round(roundingDigits));
                if (variableType == VariableType.Double)
                    fieldInfo.SetValue(null, ((double)value2 + mouseDelta).Round(roundingDigits));
                if (variableType == VariableType.Decimal)
                    fieldInfo.SetValue(null, ((decimal)value2 + (decimal)mouseDelta).Round(roundingDigits));
            }
        }
    }
}