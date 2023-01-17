using SCKRM.Rhythm;
using System.Collections.Generic;

namespace SDJK.Map.Ruleset.ADOFAI
{
    public sealed class ADOFAIMapFile : MapFile
    {
        public List<double> tiles { get; } = new List<double>();
        public List<ADOFAITileEffectFile<double>> holds { get; } = new List<ADOFAITileEffectFile<double>>();

        public List<ADOFAITileEffectFile<bool>> twirls { get; } = new List<ADOFAITileEffectFile<bool>>();
    }

    public struct ADOFAITileEffectFile<T>
    {
        public int targetTileIndex { get; }
        public T value { get; }

        public ADOFAITileEffectFile(int targetTileIndex, T value)
        {
            this.targetTileIndex = targetTileIndex;
            this.value = value;
        }
    }
}
