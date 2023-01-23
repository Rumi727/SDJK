using Cysharp.Threading.Tasks;
using SCKRM.Renderer;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SCKRM.UI.Setting
{
    [AddComponentMenu("SC KRM/UI/Setting/Input Field (Save file linkage)")]
    public class SettingInputField : SettingDrag
    {
        [SerializeField] string _textPlaceHolderNameSpace = ""; public string textPlaceHolderNameSpace { get => _textPlaceHolderNameSpace; set => _textPlaceHolderNameSpace = value; }
        [SerializeField] string _numberPlaceHolderNameSpace = ""; public string numberPlaceHolderNameSpace { get => _numberPlaceHolderNameSpace; set => _numberPlaceHolderNameSpace = value; }

        [SerializeField] string _textPlaceHolderPath = ""; public string textPlaceHolderPath { get => _textPlaceHolderPath; set => _textPlaceHolderPath = value; }
        [SerializeField] string _numberPlaceHolderPath = ""; public string numberPlaceHolderPath { get => _numberPlaceHolderPath; set => _numberPlaceHolderPath = value; }



        [SerializeField] TMP_InputField _inputField; public TMP_InputField inputField { get => _inputField; set => _inputField = value; }
        [SerializeField] CustomTextMeshProRenderer _placeholder; public CustomTextMeshProRenderer placeholder { get => _placeholder; set => _placeholder = value; }


        [SerializeField] UnityEvent _onEndEdit = new UnityEvent(); public UnityEvent onEndEdit { get => _onEndEdit; set => _onEndEdit = value; }



        protected override async UniTask<bool> Awake()
        {
            if (await base.Awake())
                return true;

            if (variableType == VariableType.Char || variableType == VariableType.String)
            {
                placeholder.nameSpace = textPlaceHolderNameSpace;
                placeholder.path = textPlaceHolderPath;
            }
            else
            {
                placeholder.nameSpace = numberPlaceHolderNameSpace;
                placeholder.path = numberPlaceHolderPath;
            }

            placeholder.Refresh();
            return false;
        }

        public override void SetDefault()
        {
            base.SetDefault();
            ScriptOnValueChanged();
        }

        public virtual void OnEndEdit()
        {
            if (invokeLock)
                return;
            else if (variableType != VariableType.String && string.IsNullOrEmpty(inputField.text))
                inputField.text = "0";

            SaveStringValue(inputField.text);
            ScriptOnValueChanged();
        }

        public override void ScriptOnValueChanged(bool settingInfoInvoke = true)
        {
            Update();

            if (settingInfoInvoke)
                SettingInfoInvoke(inputField.text);

            onEndEdit.Invoke();
        }

        [NonSerialized] bool invokeLock = false;
        protected override void Update()
        {
            base.Update();

            if (InitialLoadManager.isInitialLoadEnd && isLoad && !inputField.isFocused)
            {
                string value = GetValue().ToString();

                invokeLock = true;
                inputField.text = value;
                invokeLock = false;

                isDefault = defaultValue.ToString() == value;
            }
        }
    }
}