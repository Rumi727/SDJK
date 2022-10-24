using SCKRM.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.MainMenu
{
    public class LogoVisualizer : SCKRM.UI.UI
    {
        [SerializeField] RectTransform logo;

        void Update()
        {
            Vector2 sizeOffset = logo.rect.size / new Vector2(648, 648);
            transform.localScale = new Vector3(sizeOffset.x, sizeOffset.y, 1);
        }
    }
}
