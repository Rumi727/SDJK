using HSVPicker;
using SCKRM.Json;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SCKRM.SaveLoad.UI
{
    [AddComponentMenu("SC KRM/Save Load/UI/Color Picker (Save file linkage)")]
    public class SaveLoadUIColorPicker : SaveLoadUIBase
    {
        [SerializeField, NotNull] ColorPicker _colorPicker; public ColorPicker colorPicker { get => _colorPicker; set => _colorPicker = value; }
        [SerializeField] UnityEvent _onValueChanged = new UnityEvent(); public UnityEvent onValueChanged { get => _onValueChanged; set => _onValueChanged = value; }

        public virtual void OnValueChanged()
        {
            if (invokeLock)
                return;
            else if (variableType == VariableType.JColor)
                SaveValue((JColor)colorPicker.CurrentColor);
            else if (variableType == VariableType.JColor32)
                SaveValue((JColor32)colorPicker.CurrentColor);

            ScriptOnValueChanged();
        }

        public override void VarReset()
        {
            base.VarReset();
            onValueChanged.RemoveAllListeners();
        }

        public override void SetDefault()
        {
            base.SetDefault();
            ScriptOnValueChanged();
        }

        public override void ScriptOnValueChanged(bool settingInfoInvoke = true)
        {
            Update();

            if (settingInfoInvoke)
            {
                if (colorPicker.Setup.ShowAlpha)
                    SettingInfoInvoke("#" + ColorUtility.ToHtmlStringRGBA(colorPicker.CurrentColor));
                else
                    SettingInfoInvoke("#" + ColorUtility.ToHtmlStringRGB(colorPicker.CurrentColor));
            }

            onValueChanged.Invoke();
        }

        protected override void Update()
        {
            if (!InitialLoadManager.isInitialLoadEnd || !isLoad)
                return;

            base.Update();

            if (variableType == VariableType.JColor)
            {
                Color value = (Color)(JColor)GetValue();

                invokeLock = true;
                colorPicker.CurrentColor = value;
                invokeLock = false;

                isDefault = (Color)(JColor)defaultValue == value;
            }
            else if (variableType == VariableType.JColor32)
            {
                Color value = (Color)(JColor32)GetValue();

                invokeLock = true;
                colorPicker.CurrentColor = value;
                invokeLock = false;

                isDefault = (Color)(JColor32)defaultValue == value;
            }
        }
    }
}