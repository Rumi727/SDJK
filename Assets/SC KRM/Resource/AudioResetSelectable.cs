using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.Resource.UI
{
    [WikiDescription("오디오 리셋 버튼을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Resource/Audio Reset Selectable")]
    public sealed class AudioResetSelectable : MonoBehaviour
    {
        [SerializeField] Selectable button;

        void Update() => button.interactable = !ResourceManager.audioResetProhibition;
    }
}
