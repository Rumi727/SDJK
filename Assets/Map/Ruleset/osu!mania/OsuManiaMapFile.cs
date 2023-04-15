using System.Collections.Generic;

namespace SDJK.Map.Ruleset.OsuMania
{
    public sealed class OsuManiaMapFile : MapFile
    {
        public OsuManiaMapFile(string mapFilePath) : base(mapFilePath) { }

        public TypeList<TypeList<OsuManiaNoteFile>> notes { get; } = new();
    }

    public struct OsuManiaNoteFile
    {
        public OsuManiaNoteFile(double beat, double holdLength)
        {
            this.beat = beat;
            this.holdLength = holdLength;
        }

        public double beat { get; set; }
        public double holdLength { get; set; }
    }
}
