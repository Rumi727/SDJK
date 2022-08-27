using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.Resource.UI
{
    [WikiDescription("리소스팩 설정 버튼을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Resource/Resource Pack List/Resource Pack Setting Button")]
    public sealed class ResourcePackSettingButton : MonoBehaviour
    {
        [SerializeField] Button button;

        void Update() => button.interactable = !ResourceManager.isResourceRefesh;
    }
}
