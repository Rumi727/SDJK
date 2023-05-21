using UnityEngine;

namespace SCKRM.SaveLoad.UI
{
    [AddComponentMenu("SC KRM/Save Load/UI/Disable Condition (Save file linkage)")]
    public sealed class SaveLoadUIDisableCondition : MonoBehaviour
    {
        [SerializeField, FieldNotNull] SaveLoadUIBase _setting; public SaveLoadUIBase setting { get => _setting; set => _setting = value; }
        [SerializeField, FieldNotNull] GameObject _disableGameObject; public GameObject disableGameObject { get => _disableGameObject; set => _disableGameObject = value; }
        [SerializeField] bool _reversal = false; public bool reversal { get => _reversal; set => _reversal = value; }



        void Update()
        {
            if (setting == null)
                return;

            if (InitialLoadManager.isInitialLoadEnd && setting.isLoad && setting.variableType == SaveLoadUIBase.VariableType.Bool)
            {
                if ((bool)setting.GetValue())
                {
                    if (reversal)
                    {
                        if (disableGameObject.activeSelf)
                            disableGameObject.SetActive(false);
                    }
                    else if (!disableGameObject.activeSelf)
                        disableGameObject.SetActive(true);
                }
                else
                {
                    if (reversal)
                    {
                        if (!disableGameObject.activeSelf)
                            disableGameObject.SetActive(true);
                    }
                    else if (disableGameObject.activeSelf)
                        disableGameObject.SetActive(false);
                }
            }
        }
    }
}