using SCKRM;
using SCKRM.NTP;
using SCKRM.Renderer;
using SCKRM.Tooltip;
using System;
using UnityEngine;

namespace SDJK.Ruleset.ReplayResult
{
    public sealed class ReplayResultUIClearTime : ReplayResultUIBase
    {
        [SerializeField, NotNull] CustomTextRendererBase text;
        [SerializeField, NotNull] Tooltip tooltip;

        public override void RealUpdate(float lerpValue)
        {
            TimeSpan time = replay.clearUTCTime - NTPDateTime.utcNow;
            string utcOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).ToString(@"\U\T\C\+hh\:mm");


            text.nameSpacePathReplacePair = time.ToRelativeString();
            text.Refresh();
            time.ToString(true);

            tooltip.nameSpacePathPair = new NameSpacePathPair(replay.clearUTCTime.ToLocalTime().ToString(@"yyyy\-MM\-dd H\:mm\:ss\.ff") + " " + utcOffset);
        }

        public override void ObjectReset()
        {
            base.ObjectReset();

            text.nameSpacePathReplacePair = "";
            text.Refresh();
        }
    }
}
