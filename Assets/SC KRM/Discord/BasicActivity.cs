using System;
using UnityEngine;

namespace SCKRM.Discord
{
    [Serializable]
    public class BasicActivity
    {
        public string details = "";
        public string state = "";

        public string largeImage = null;
        public string largeText = "";

        public string smallImage = null;
        public string smallText = "";

        public long startTime = 0;
        public long endTime = 0;
    }
}
