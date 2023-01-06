using SDJK.Map.Ruleset.SDJK.Map;
using System.Collections.Generic;

namespace SDJK.Map.Ruleset.ADOFAI
{
    public sealed class ADOFAIMapFile : MapFile
    {
        public List<double> tiles { get; } = new List<double>();
        public List<HoldFile> holds { get; } = new List<HoldFile>();
    }

    public struct HoldFile
    {
        public int targetTileIndex { get; }
        public double length { get; }

        public HoldFile(int targetTileIndex, double length = 0)
        {
            this.targetTileIndex = targetTileIndex;
            this.length = length;
        }
    }
}
