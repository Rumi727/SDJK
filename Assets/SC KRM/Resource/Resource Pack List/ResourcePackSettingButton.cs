using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.Resource.UI
{
    [AddComponentMenu("SC KRM/Resource/Resource Pack List/Resource Pack Setting Button")]
    public sealed class ResourcePackSettingButton : MonoBehaviour
    {
        [SerializeField] Button button;

        void Update() => button.interactable = !ResourceManager.isResourceRefesh;
    }
}
