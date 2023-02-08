using UnityEngine;
using UnityEngine.UI;

namespace SCKRM.Resource.UI
{
    [WikiDescription("오디오 리셋 버튼을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Resource/Audio Reset Button")]
    public sealed class AudioResetButton : MonoBehaviour
    {
        [SerializeField] Button button;

        void Update() => button.interactable = !ResourceManager.audioResetProhibition;
    }
}
