using Cysharp.Threading.Tasks;
using ExtendedNumerics;
using SCKRM.Cursor;
using SCKRM.Flee;
using SCKRM.Input;
using SCKRM.Json;
using SCKRM.Renderer;
using SCKRM.UI;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using UnityEngine;

using Vector2 = UnityEngine.Vector2;

namespace SCKRM.SaveLoad.UI
{
    public abstract class SaveLoadUIBase : UIObjectPooling
    {
        public static Dictionary<string, SaveLoadUIBase> settingInstance { get; } = new Dictionary<string, SaveLoadUIBase>();
        public static Dictionary<string, List<SaveLoadUIBase>> settingInstances { get; } = new Dictionary<string, List<SaveLoadUIBase>>();


        [SerializeField, Tooltip("SaveLoadManager.generalSLCList 리스트를 사용합니다")] bool _autoRefresh = false; public bool autoRefresh { get => _autoRefresh; set => _autoRefresh = value; }

        [SerializeField] string _saveLoadClassName = ""; public string saveLoadClassName { get => _saveLoadClassName; set => _saveLoadClassName = value; }
        [SerializeField] string _variableName = ""; public string variableName { get => _variableName; set => _variableName = value; }

        [SerializeField] int _roundingDigits = 2; public int roundingDigits { get => _roundingDigits; set => _roundingDigits = value; }
        [SerializeField] string[] _hotkeyToDisplay = new string[0]; public string[] hotkeyToDisplays { get => _hotkeyToDisplay; set => _hotkeyToDisplay = value; }

        [SerializeField, NotNull] CanvasGroup _resetButton; public CanvasGroup resetButton => _resetButton;
        [SerializeField, NotNull] RectTransform _nameText; public RectTransform nameText => _nameText;
        [SerializeField, NotNull] CustomTextRendererBase _nameTextRenderer; public CustomTextRendererBase nameTextRenderer => _nameTextRenderer;
        [SerializeField, NotNull] Tooltip.Tooltip _tooltip; public Tooltip.Tooltip tooltip => _tooltip;



        public VariableType variableType { get; private set; } = VariableType.String;

        public object defaultValue { get; private set; } = null;
        public Type type { get; private set; } = null;
        public PropertyInfo propertyInfo { get; private set; } = null;
        public FieldInfo fieldInfo { get; private set; } = null;
        public object instance { get; private set; } = null;

        public bool isDefault { get; protected set; } = true;

        public bool isLoad { get; private set; } = false;

        public bool invokeLock { get; set; } = false;



        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            VarReset();
            return true;
        }

        public virtual void VarReset()
        {
            settingInstance.Clear();
            settingInstances.Clear();

            variableType = VariableType.String;

            defaultValue = null;
            type = null;
            propertyInfo = null;
            fieldInfo = null;

            isLoad = false;
            isDefault = true;
        }

        public virtual void Refresh(params SaveLoadClass[] slcs)
        {
            VarReset();

            for (int i = 0; i < slcs.Length; i++)
            {
                SaveLoadClass slc = slcs[i];
                if (slc.name == saveLoadClassName)
                {
                    foreach (var propertyInfo in slc.propertyInfos)
                    {
                        if (propertyInfo.variableInfo.Name == variableName)
                        {
                            type = propertyInfo.variableInfo.PropertyType;

                            defaultValue = propertyInfo.defaultValue;
                            this.propertyInfo = propertyInfo.variableInfo;

                            instance = slc.instance;
                            break;
                        }
                    }

                    foreach (var fieldInfo in slc.fieldInfos)
                    {
                        if (fieldInfo.variableInfo.Name == variableName)
                        {
                            type = fieldInfo.variableInfo.FieldType;

                            defaultValue = fieldInfo.defaultValue;
                            this.fieldInfo = fieldInfo.variableInfo;

                            instance = slc.instance;
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
            else if (type == typeof(BigInteger))
                variableType = VariableType.BigInteger;
            else if (type == typeof(BigDecimal))
                variableType = VariableType.BigDecimal;

            isLoad = true;

            string key = saveLoadClassName + "." + variableName;
            if (!settingInstances.ContainsKey(key))
                settingInstances.Add(key, new List<SaveLoadUIBase>() { this });
            else
                settingInstances[key].Add(this);

            if (!settingInstance.ContainsKey(key))
                settingInstance.Add(key, this);
        }



        /// <returns>is Cancel?</returns>
        new protected virtual async UniTask<bool> Awake()
        {
            if (!autoRefresh)
                return true;
            else if (await UniTask.WaitUntil(() => InitialLoadManager.isInitialLoadEnd, PlayerLoopTiming.Initialization, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow())
                return true;

            Refresh(SaveLoadManager.generalSLCList);
            return false;
        }

        protected virtual void Update()
        {
            if (!InitialLoadManager.isInitialLoadEnd || !isLoad)
                return;

            if (isDefault)
            {
                resetButton.interactable = false;
                nameText.offsetMin = nameText.offsetMin.Lerp(new Vector2(0, nameText.offsetMin.y), 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                resetButton.alpha = resetButton.alpha.Lerp(0, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);

                if (resetButton.alpha < 0.01f)
                    resetButton.alpha = 0;
            }
            else
            {
                resetButton.interactable = true;
                nameText.offsetMin = nameText.offsetMin.Lerp(new Vector2(22, nameText.offsetMin.y), 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                resetButton.alpha = resetButton.alpha.Lerp(1, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);

                if (resetButton.alpha > 0.99f)
                    resetButton.alpha = 1;
            }
        }

        public virtual void SetDefault()
        {
            if (propertyInfo != null)
                propertyInfo.SetValue(instance, defaultValue);
            else if (fieldInfo != null)
                fieldInfo.SetValue(instance, defaultValue);
        }

        public virtual object GetValue()
        {
            if (propertyInfo != null)
            {
                return variableType switch
                {
                    VariableType.Float => ((float)propertyInfo.GetValue(instance)).Round(roundingDigits),
                    VariableType.Double => ((double)propertyInfo.GetValue(instance)).Round(roundingDigits),
                    VariableType.Decimal => ((decimal)propertyInfo.GetValue(instance)).Round(roundingDigits),
                    VariableType.BigDecimal => ((BigDecimal)propertyInfo.GetValue(instance)).Round(roundingDigits),
                    _ => propertyInfo.GetValue(instance)
                };
            }
            else if (fieldInfo != null)
            {
                return variableType switch
                {
                    VariableType.Float => ((float)fieldInfo.GetValue(instance)).Round(roundingDigits),
                    VariableType.Double => ((double)fieldInfo.GetValue(instance)).Round(roundingDigits),
                    VariableType.BigDecimal => ((BigDecimal)fieldInfo.GetValue(instance)).Round(roundingDigits),
                    _ => fieldInfo.GetValue(instance)
                };
            }

            return null;
        }

        public virtual int GetValueInt()
        {
            if (propertyInfo != null)
            {
                return variableType switch
                {
                    VariableType.Char => int.Parse(((char)propertyInfo.GetValue(instance)).ToString()),
                    VariableType.String => int.Parse((string)propertyInfo.GetValue(instance)),
                    VariableType.Bool => (bool)propertyInfo.GetValue(instance) ? 1 : 0,
                    VariableType.Byte => (byte)propertyInfo.GetValue(instance),
                    VariableType.Sbyte => (sbyte)propertyInfo.GetValue(instance),
                    VariableType.Short => (short)propertyInfo.GetValue(instance),
                    VariableType.Int => (int)propertyInfo.GetValue(instance),
                    VariableType.Long => (int)(long)propertyInfo.GetValue(instance),
                    VariableType.Ushort => (ushort)propertyInfo.GetValue(instance),
                    VariableType.Uint => (int)(uint)propertyInfo.GetValue(instance),
                    VariableType.Ulong => (int)(ulong)propertyInfo.GetValue(instance),
                    VariableType.Float => (int)(float)propertyInfo.GetValue(instance),
                    VariableType.Double => (int)(double)propertyInfo.GetValue(instance),
                    VariableType.Decimal => (int)(decimal)propertyInfo.GetValue(instance),
                    VariableType.BigInteger => (int)(BigInteger)propertyInfo.GetValue(instance),
                    VariableType.BigDecimal => (int)(BigDecimal)propertyInfo.GetValue(instance),
                    _ => 0,
                };
            }
            else if (fieldInfo != null)
            {
                return variableType switch
                {
                    VariableType.Char => int.Parse(((char)fieldInfo.GetValue(instance)).ToString()),
                    VariableType.String => int.Parse((string)fieldInfo.GetValue(instance)),
                    VariableType.Bool => (bool)fieldInfo.GetValue(instance) ? 1 : 0,
                    VariableType.Byte => (byte)fieldInfo.GetValue(instance),
                    VariableType.Sbyte => (sbyte)fieldInfo.GetValue(instance),
                    VariableType.Short => (short)fieldInfo.GetValue(instance),
                    VariableType.Int => (int)fieldInfo.GetValue(instance),
                    VariableType.Long => (int)(long)fieldInfo.GetValue(instance),
                    VariableType.Ushort => (ushort)fieldInfo.GetValue(instance),
                    VariableType.Uint => (int)(uint)fieldInfo.GetValue(instance),
                    VariableType.Ulong => (int)(ulong)fieldInfo.GetValue(instance),
                    VariableType.Float => (int)(float)fieldInfo.GetValue(instance),
                    VariableType.Double => (int)(double)fieldInfo.GetValue(instance),
                    VariableType.Decimal => (int)(decimal)fieldInfo.GetValue(instance),
                    VariableType.BigInteger => (int)(BigInteger)fieldInfo.GetValue(instance),
                    VariableType.BigDecimal => (int)(BigDecimal)fieldInfo.GetValue(instance),
                    _ => 0,
                };
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
                        return float.Parse(((char)propertyInfo.GetValue(instance)).ToString());
                    case VariableType.String:
                        return float.Parse((string)propertyInfo.GetValue(instance));
                    case VariableType.Bool:
                        return (bool)propertyInfo.GetValue(instance) ? 1 : 0;
                    case VariableType.Byte:
                        return (byte)propertyInfo.GetValue(instance);
                    case VariableType.Sbyte:
                        return (sbyte)propertyInfo.GetValue(instance);
                    case VariableType.Short:
                        return (short)propertyInfo.GetValue(instance);
                    case VariableType.Int:
                        return (int)propertyInfo.GetValue(instance);
                    case VariableType.Long:
                        return (long)propertyInfo.GetValue(instance);
                    case VariableType.Ushort:
                        return (ushort)propertyInfo.GetValue(instance);
                    case VariableType.Uint:
                        return (uint)propertyInfo.GetValue(instance);
                    case VariableType.Ulong:
                        return (ulong)propertyInfo.GetValue(instance);
                    case VariableType.Float:
                        return (float)propertyInfo.GetValue(instance);
                    case VariableType.Double:
                        return (float)((double)propertyInfo.GetValue(instance)).Round(roundingDigits);
                    case VariableType.Decimal:
                        return (float)((decimal)propertyInfo.GetValue(instance)).Round(roundingDigits);
                    case VariableType.BigInteger:
                        return (float)(BigInteger)propertyInfo.GetValue(instance);
                    case VariableType.BigDecimal:
                        return (float)((BigDecimal)propertyInfo.GetValue(instance)).Round(roundingDigits);
                }
            }
            else if (fieldInfo != null)
            {
                switch (variableType)
                {
                    case VariableType.Char:
                        return float.Parse(((char)fieldInfo.GetValue(instance)).ToString());
                    case VariableType.String:
                        return float.Parse((string)fieldInfo.GetValue(instance));
                    case VariableType.Bool:
                        return (bool)fieldInfo.GetValue(instance) ? 1 : 0;
                    case VariableType.Byte:
                        return (byte)fieldInfo.GetValue(instance);
                    case VariableType.Sbyte:
                        return (sbyte)fieldInfo.GetValue(instance);
                    case VariableType.Short:
                        return (short)fieldInfo.GetValue(instance);
                    case VariableType.Int:
                        return (int)fieldInfo.GetValue(instance);
                    case VariableType.Long:
                        return (long)fieldInfo.GetValue(instance);
                    case VariableType.Ushort:
                        return (ushort)fieldInfo.GetValue(instance);
                    case VariableType.Uint:
                        return (uint)fieldInfo.GetValue(instance);
                    case VariableType.Ulong:
                        return (ulong)fieldInfo.GetValue(instance);
                    case VariableType.Float:
                        return (float)fieldInfo.GetValue(instance);
                    case VariableType.Double:
                        return (float)((double)fieldInfo.GetValue(instance)).Round(roundingDigits);
                    case VariableType.Decimal:
                        return (float)((decimal)fieldInfo.GetValue(instance)).Round(roundingDigits);
                    case VariableType.BigInteger:
                        return (float)(BigInteger)fieldInfo.GetValue(instance);
                    case VariableType.BigDecimal:
                        return (float)((BigDecimal)fieldInfo.GetValue(instance)).Round(roundingDigits);
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
                        propertyInfo.SetValue(instance, ((float)value).Round(roundingDigits));
                        break;
                    case VariableType.Double:
                        propertyInfo.SetValue(instance, ((double)value).Round(roundingDigits));
                        break;
                    case VariableType.Decimal:
                        propertyInfo.SetValue(instance, ((decimal)value).Round(roundingDigits));
                        break;
                    case VariableType.BigDecimal:
                        propertyInfo.SetValue(instance, ((BigDecimal)value).Round(roundingDigits));
                        break;
                    default:
                        propertyInfo.SetValue(instance, value);
                        break;
                }
            }
            else if (fieldInfo != null)
            {
                switch (variableType)
                {
                    case VariableType.Float:
                        fieldInfo.SetValue(instance, ((float)value).Round(roundingDigits));
                        break;
                    case VariableType.Double:
                        fieldInfo.SetValue(instance, ((double)value).Round(roundingDigits));
                        break;
                    case VariableType.Decimal:
                        fieldInfo.SetValue(instance, ((decimal)value).Round(roundingDigits));
                        break;
                    case VariableType.BigDecimal:
                        fieldInfo.SetValue(instance, ((BigDecimal)value).Round(roundingDigits));
                        break;
                    default:
                        fieldInfo.SetValue(instance, value);
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
                        propertyInfo.SetValue(instance, value.ToString()[0]);
                        break;
                    case VariableType.String:
                        propertyInfo.SetValue(instance, value.ToString());
                        break;
                    case VariableType.Byte:
                        propertyInfo.SetValue(instance, (byte)value);
                        break;
                    case VariableType.Sbyte:
                        propertyInfo.SetValue(instance, (sbyte)value);
                        break;
                    case VariableType.Short:
                        propertyInfo.SetValue(instance, (short)value);
                        break;
                    case VariableType.Int:
                        propertyInfo.SetValue(instance, (int)value);
                        break;
                    case VariableType.Long:
                        propertyInfo.SetValue(instance, (long)value);
                        break;
                    case VariableType.Ushort:
                        propertyInfo.SetValue(instance, (ushort)value);
                        break;
                    case VariableType.Uint:
                        propertyInfo.SetValue(instance, (uint)value);
                        break;
                    case VariableType.Ulong:
                        propertyInfo.SetValue(instance, (ulong)value);
                        break;
                    case VariableType.Float:
                        propertyInfo.SetValue(instance, value);
                        break;
                    case VariableType.Double:
                        propertyInfo.SetValue(instance, (double)value);
                        break;
                    case VariableType.Decimal:
                        propertyInfo.SetValue(instance, (decimal)value);
                        break;
                    case VariableType.BigInteger:
                        propertyInfo.SetValue(instance, (BigInteger)value);
                        break;
                    case VariableType.BigDecimal:
                        propertyInfo.SetValue(instance, (BigDecimal)value);
                        break;
                    default:
                        break;
                }
            }
            else if (fieldInfo != null)
            {
                switch (variableType)
                {
                    case VariableType.Char:
                        fieldInfo.SetValue(instance, value.ToString()[0]);
                        break;
                    case VariableType.String:
                        fieldInfo.SetValue(instance, value.ToString());
                        break;
                    case VariableType.Byte:
                        fieldInfo.SetValue(instance, (byte)value);
                        break;
                    case VariableType.Sbyte:
                        fieldInfo.SetValue(instance, (sbyte)value);
                        break;
                    case VariableType.Short:
                        fieldInfo.SetValue(instance, (short)value);
                        break;
                    case VariableType.Int:
                        fieldInfo.SetValue(instance, (int)value);
                        break;
                    case VariableType.Long:
                        fieldInfo.SetValue(instance, (long)value);
                        break;
                    case VariableType.Ushort:
                        fieldInfo.SetValue(instance, (ushort)value);
                        break;
                    case VariableType.Uint:
                        fieldInfo.SetValue(instance, (uint)value);
                        break;
                    case VariableType.Ulong:
                        fieldInfo.SetValue(instance, (ulong)value);
                        break;
                    case VariableType.Float:
                        fieldInfo.SetValue(instance, value);
                        break;
                    case VariableType.Double:
                        fieldInfo.SetValue(instance, (double)value);
                        break;
                    case VariableType.Decimal:
                        fieldInfo.SetValue(instance, (decimal)value);
                        break;
                    case VariableType.BigInteger:
                        fieldInfo.SetValue(instance, (BigInteger)value);
                        break;
                    case VariableType.BigDecimal:
                        fieldInfo.SetValue(instance, (BigDecimal)value);
                        break;
                    default:
                        break;
                }
            }
        }

        public virtual void SaveStringValue(string value)
        {
            object number = 0;
            try
            {
                switch (variableType)
                {
                    case VariableType.Char:
                        break;
                    case VariableType.String:
                        break;
                    case VariableType.Bool:
                        number = FleeManager.Calculate<bool>(value);
                        break;
                    case VariableType.Byte:
                        number = (byte)FleeManager.Calculate<double>(value).RepeatWhile(byte.MinValue, byte.MaxValue);
                        break;
                    case VariableType.Sbyte:
                        number = (sbyte)FleeManager.Calculate<double>(value).RepeatWhile(sbyte.MinValue, sbyte.MaxValue);
                        break;
                    case VariableType.Short:
                        number = (short)FleeManager.Calculate<double>(value).RepeatWhile(short.MinValue, short.MaxValue);
                        break;
                    case VariableType.Int:
                        number = (int)FleeManager.Calculate<double>(value).RepeatWhile(int.MinValue, int.MaxValue);
                        break;
                    case VariableType.Long:
                        number = (long)FleeManager.Calculate<double>(value).RepeatWhile(long.MinValue, long.MaxValue);
                        break;
                    case VariableType.Ushort:
                        number = (ushort)FleeManager.Calculate<double>(value).RepeatWhile(ushort.MinValue, ushort.MaxValue);
                        break;
                    case VariableType.Uint:
                        number = (uint)FleeManager.Calculate<double>(value).RepeatWhile(uint.MinValue, uint.MaxValue);
                        break;
                    case VariableType.Ulong:
                        number = (ulong)FleeManager.Calculate<double>(value).RepeatWhile(ulong.MinValue, ulong.MaxValue);
                        break;
                    case VariableType.Float:
                        number = (float)FleeManager.Calculate<double>(value).RepeatWhile(float.MinValue, float.MaxValue);
                        break;
                    case VariableType.Double:
                        number = FleeManager.Calculate<double>(value).Round(roundingDigits);
                        break;
                    case VariableType.Decimal:
                        number = FleeManager.Calculate<decimal>(value).Round(roundingDigits);
                        break;
                    case VariableType.JColor:
                        break;
                    case VariableType.JColor32:
                        break;
                    case VariableType.BigInteger:
                        number = BigInteger.Parse(value);
                        break;
                    case VariableType.BigDecimal:
                        number = BigDecimal.Parse(value).Round(roundingDigits);
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                return;
            }

            try
            {
                if (propertyInfo != null)
                {
                    switch (variableType)
                    {
                        case VariableType.Char:
                            if (value.Length > 0)
                                propertyInfo.SetValue(instance, value[0]);
                            else
                                propertyInfo.SetValue(instance, char.MinValue);
                            break;
                        case VariableType.String:
                            propertyInfo.SetValue(instance, value);
                            break;
                        case VariableType.Bool or VariableType.Byte or VariableType.Sbyte or VariableType.Short or VariableType.Int or VariableType.Long or VariableType.Ushort or VariableType.Uint or VariableType.Ulong or VariableType.Float or VariableType.Double or VariableType.Decimal or VariableType.BigInteger or VariableType.BigDecimal:
                            propertyInfo.SetValue(instance, number);
                            break;
                        default:
                            break;
                    }
                }
                else if (fieldInfo != null)
                {
                    switch (variableType)
                    {
                        case VariableType.Char:
                            if (value.Length > 0)
                                fieldInfo.SetValue(instance, value[0]);
                            else
                                fieldInfo.SetValue(instance, char.MinValue);
                            break;
                        case VariableType.String:
                            fieldInfo.SetValue(instance, value);
                            break;
                        case VariableType.Bool or VariableType.Byte or VariableType.Sbyte or VariableType.Short or VariableType.Int or VariableType.Long or VariableType.Ushort or VariableType.Uint or VariableType.Ulong or VariableType.Float or VariableType.Double or VariableType.Decimal or VariableType.BigInteger or VariableType.BigDecimal:
                            propertyInfo.SetValue(instance, number);
                            break;
                        default:
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
            JColor32,
            BigInteger,
            BigDecimal,
        }
    }

    public abstract class SettingDrag : SaveLoadUIBase
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
                object value = propertyInfo.GetValue(instance);

                switch (variableType)
                {
                    case VariableType.Byte:
                        propertyInfo.SetValue(instance, (byte)((byte)value + mouseDelta));
                        break;
                    case VariableType.Sbyte:
                        propertyInfo.SetValue(instance, (sbyte)((sbyte)value + mouseDelta));
                        break;
                    case VariableType.Short:
                        propertyInfo.SetValue(instance, (short)((short)value + mouseDelta));
                        break;
                    case VariableType.Int:
                        propertyInfo.SetValue(instance, (int)((int)value + mouseDelta));
                        break;
                    case VariableType.Long:
                        propertyInfo.SetValue(instance, (long)((long)value + mouseDelta));
                        break;
                    case VariableType.Ushort:
                        propertyInfo.SetValue(instance, (ushort)((ushort)value + mouseDelta));
                        break;
                    case VariableType.Uint:
                        propertyInfo.SetValue(instance, (uint)((uint)value + mouseDelta));
                        break;
                    case VariableType.Ulong:
                        propertyInfo.SetValue(instance, (ulong)((ulong)value + mouseDelta));
                        break;
                    case VariableType.Float:
                        propertyInfo.SetValue(instance, ((float)value + mouseDelta).Round(roundingDigits));
                        break;
                    case VariableType.Double:
                        propertyInfo.SetValue(instance, ((double)value + mouseDelta).Round(roundingDigits));
                        break;
                    case VariableType.Decimal:
                        propertyInfo.SetValue(instance, ((decimal)value + (decimal)mouseDelta).Round(roundingDigits));
                        break;
                    case VariableType.BigInteger:
                        propertyInfo.SetValue(instance, (BigInteger)value + (BigInteger)mouseDelta);
                        break;
                    case VariableType.BigDecimal:
                        propertyInfo.SetValue(instance, ((BigDecimal)value + (BigDecimal)mouseDelta).Round(roundingDigits));
                        break;
                    default:
                        break;
                }
            }
            else if (fieldInfo != null)
            {
                object value = fieldInfo.GetValue(instance);

                switch (variableType)
                {
                    case VariableType.Byte:
                        fieldInfo.SetValue(instance, (byte)((byte)value + mouseDelta));
                        break;
                    case VariableType.Sbyte:
                        fieldInfo.SetValue(instance, (sbyte)((sbyte)value + mouseDelta));
                        break;
                    case VariableType.Short:
                        fieldInfo.SetValue(instance, (short)((short)value + mouseDelta));
                        break;
                    case VariableType.Int:
                        fieldInfo.SetValue(instance, (int)((int)value + mouseDelta));
                        break;
                    case VariableType.Long:
                        fieldInfo.SetValue(instance, (long)((long)value + mouseDelta));
                        break;
                    case VariableType.Ushort:
                        fieldInfo.SetValue(instance, (ushort)((ushort)value + mouseDelta));
                        break;
                    case VariableType.Uint:
                        fieldInfo.SetValue(instance, (uint)((uint)value + mouseDelta));
                        break;
                    case VariableType.Ulong:
                        fieldInfo.SetValue(instance, (ulong)((ulong)value + mouseDelta));
                        break;
                    case VariableType.Float:
                        fieldInfo.SetValue(instance, ((float)value + mouseDelta).Round(roundingDigits));
                        break;
                    case VariableType.Double:
                        fieldInfo.SetValue(instance, ((double)value + mouseDelta).Round(roundingDigits));
                        break;
                    case VariableType.Decimal:
                        fieldInfo.SetValue(instance, ((decimal)value + (decimal)mouseDelta).Round(roundingDigits));
                        break;
                    case VariableType.BigInteger:
                        fieldInfo.SetValue(instance, (BigInteger)value + (BigInteger)mouseDelta);
                        break;
                    case VariableType.BigDecimal:
                        fieldInfo.SetValue(instance, ((BigDecimal)value + (BigDecimal)mouseDelta).Round(roundingDigits));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}