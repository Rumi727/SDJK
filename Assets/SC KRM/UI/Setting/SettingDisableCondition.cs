using UnityEngine;

namespace SCKRM.UI.Setting
{
    [AddComponentMenu("SC KRM/UI/Setting/Disable Condition (Save file linkage)")]
    public sealed class SettingDisableCondition : MonoBehaviour
    {
        [SerializeField] Setting _setting; public Setting setting { get => _setting; set => _setting = value; }
        [SerializeField] GameObject _disableGameObject; public GameObject disableGameObject { get => _disableGameObject; set => _disableGameObject = value; }
        [SerializeField] bool _reversal = false; public bool reversal { get => _reversal; set => _reversal = value; }



        void Update()
        {
            if (setting == null)
                return;

            if (InitialLoadManager.isInitialLoadEnd && setting.isLoad && setting.variableType == Setting.VariableType.Bool)
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