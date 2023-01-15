using SCKRM;
using SCKRM.Sound;
using TMPro;
using UnityEngine;

namespace SDJK.MainMenu.MapSelectScreen
{
    public sealed class LengthText : SCKRM.UI.UI
    {
        [SerializeField] TMP_Text text;

        void Update()
        {
            ISoundPlayer soundPlayer = BGMManager.bgm != null ? BGMManager.bgm.soundPlayer : null;
            if (soundPlayer != null && !BGMManager.bgm.padeOut)
                text.text = soundPlayer.length.ToTime();
            else
                text.text = "--:--";
        }
    }
}
