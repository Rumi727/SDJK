using SCKRM.Renderer;
using SCKRM.UI;
using UnityEngine;

namespace SCKRM
{
    public class SaveLoadUITitle : UIObjectPooling
    {
        [SerializeField, NotNull] CustomTextMeshProRenderer _customTextMeshProRenderer; public CustomTextMeshProRenderer customTextMeshProRenderer => _customTextMeshProRenderer;

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            customTextMeshProRenderer.nameSpacePathReplacePair = null;
            return true;
        }
    }
}
