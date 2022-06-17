using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SCKRM.UI.Setting
{
    [AddComponentMenu("SC KRM/UI/Setting/Slider (Save file linkage)")]
    public class SettingSlider : SettingInputField
    {
        [SerializeField] Slider _slider; public Slider slider { get => _slider; set => _slider = value; }
        [SerializeField] UnityEvent _onValueChanged = new UnityEvent(); public UnityEvent onValueChanged { get => _onValueChanged; set => _onValueChanged = value; }

        public void OnValueChanged()
        {
            if (invokeLock)
                return;

            SaveValueFloat(slider.value);
            ScriptOnValueChanged();
        }

        public override void SetDefault()
        {
            base.SetDefault();
            ScriptOnValueChanged();
        }

        public override void ScriptOnValueChanged(bool settingInfoInvoke = true)
        {
            base.ScriptOnValueChanged();
            onValueChanged.Invoke();
        }

        [NonSerialized] bool invokeLock = false;
        protected override void Update()
        {
            base.Update();

            if (InitialLoadManager.isInitialLoadEnd && isLoad && !(variableType == VariableType.Char || variableType == VariableType.String))
            {
                invokeLock = true;
                slider.value = GetValueFloat();
                invokeLock = false;

                isDefault = GetValue().ToString() == defaultValue.ToString();
            }
        }
    }
}