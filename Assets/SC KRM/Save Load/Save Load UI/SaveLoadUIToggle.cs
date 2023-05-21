using SCKRM.Renderer;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SCKRM.SaveLoad.UI
{
    [AddComponentMenu("SC KRM/Save Load/UI/Toggle (Save file linkage)")]
    public class SaveLoadUIToggle : SaveLoadUIBase
    {
        [SerializeField, FieldNotNull] Toggle _toggle; public Toggle toggle { get => _toggle; set => _toggle = value; }
        [SerializeField] UnityEvent _onValueChanged = new UnityEvent(); public UnityEvent onValueChanged { get => _onValueChanged; set => _onValueChanged = value; }

        public virtual void OnValueChanged()
        {
            if (invokeLock)
                return;
            else if (variableType != VariableType.Bool)
                return;

            SaveValue(toggle.isOn);
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
                if (toggle.isOn)
                    SettingInfoInvoke(new NameSpacePathPair("sc-krm", "gui.on"));
                else
                    SettingInfoInvoke(new NameSpacePathPair("sc-krm", "gui.off"));
            }

            onValueChanged.Invoke();
        }

        protected override void Update()
        {
            base.Update();

            if (!InitialLoadManager.isInitialLoadEnd || !isLoad || variableType != VariableType.Bool)
                return;

            bool value = (bool)GetValue();

            invokeLock = true;
            toggle.isOn = value;
            invokeLock = false;

            isDefault = (bool)defaultValue == value;
        }
    }
}