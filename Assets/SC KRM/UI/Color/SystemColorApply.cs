using UnityEngine;

namespace SCKRM.UI
{
    [AddComponentMenu("SC KRM/UI/System Color Apply")]
    public sealed class SystemColorApply : UI
    {
        [SerializeField] Color _offset = Color.white; public Color offset { get => _offset; set => _offset = value; }

        void Update() => graphic.color = UIManager.SaveData.systemColor * offset;
    }
}
