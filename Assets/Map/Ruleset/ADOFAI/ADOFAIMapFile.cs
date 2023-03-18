using System.Collections.Generic;

namespace SDJK.Map.Ruleset.ADOFAI
{
    public sealed class ADOFAIMapFile : MapFile
    {
        public ADOFAIMapFile(string mapFilePath) : base(mapFilePath) { }

        public TypeList<double> tiles { get; } = new();
        public TypeList<ADOFAITileEffectFile<double>> holds { get; } = new();

        public TypeList<ADOFAITileEffectFile<bool>> twirls { get; } = new();

        public TypeList<ADOFAITileEffectFile<bool>> autoTiles { get; } = new();

        public override void SetVisualizerEffect()
        {
            visualizerEffect.leftMove.Clear();
            visualizerEffect.leftMove.Add(double.MinValue, false, true);

            for (int i = 0; i < twirls.Count; i++)
            {
                ADOFAITileEffectFile<bool> twirl = twirls[i];

                if (twirl.targetTileIndex < tiles.Count)
                {
                    double beat = tiles[twirl.targetTileIndex];
                    visualizerEffect.leftMove.Add(beat, twirl.value, true);
                }
            }
        }
    }

    public struct ADOFAITileEffectFile<T>
    {
        public int targetTileIndex { get; set; }
        public T value { get; set; }

        public ADOFAITileEffectFile(int targetTileIndex, T value)
        {
            this.targetTileIndex = targetTileIndex;
            this.value = value;
        }
    }
}
