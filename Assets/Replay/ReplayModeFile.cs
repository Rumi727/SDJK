using Newtonsoft.Json.Linq;
using SDJK.Mode;
using System;

namespace SDJK.Replay
{
    public struct ReplayModeFile
    {
        public Type modeType { get; set; }
        public Type modeConfigType { get; set; }

        public JObject modeConfig { get; set; }

        public ReplayModeFile(Type modeType, IModeConfig modeConfig)
        {
            this.modeType = modeType;

            if (modeConfig != null)
            {
                modeConfigType = modeConfig.GetType();
                this.modeConfig = JObject.FromObject(modeConfig);
            }
            else
            {
                modeConfigType = null;
                this.modeConfig = null;
            }
        }
    }
}
