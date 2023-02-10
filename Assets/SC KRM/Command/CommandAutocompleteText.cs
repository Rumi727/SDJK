using SCKRM.UI;
using TMPro;
using UnityEngine;

namespace SCKRM
{
    public class CommandAutocompleteText : UIObjectPoolingBase
    {
        [SerializeField] TMP_Text _text; public TMP_Text text => _text;
        [SerializeField] BetterContentSizeFitter _betterContentSizeFitter; public BetterContentSizeFitter betterContentSizeFitter => _betterContentSizeFitter;

        public override void OnCreate()
        {
            base.OnCreate();
            rectTransform.anchoredPosition = new Vector2(8, 0);
        }
    }
}
