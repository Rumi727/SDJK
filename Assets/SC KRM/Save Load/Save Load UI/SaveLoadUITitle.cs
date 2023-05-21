using SCKRM.Renderer;
using SCKRM.UI;
using UnityEngine;

namespace SCKRM
{
    public class SaveLoadUITitle : UIObjectPoolingBase
    {
        [SerializeField, FieldNotNull] CustomTextMeshProRenderer _customTextMeshProRenderer; public CustomTextMeshProRenderer customTextMeshProRenderer => _customTextMeshProRenderer;

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            customTextMeshProRenderer.nameSpacePathReplacePair = null;
            return true;
        }
    }
}
