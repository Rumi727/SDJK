using SDJK.Map.Ruleset.SDJK.Map;
using System.Collections.Generic;

namespace SDJK.Replay.Ruleset.SDJK
{
    public sealed class SDJKReplayFile : ReplayFile
    {
        public List<List<double>> pressBeat { get; set; } = new();
        public List<List<double>> pressUpBeat { get; set; } = new();

        public int PressBinarySearch(double beat, int keyIndex) => InternalBinarySearch(pressBeat, beat, keyIndex);
        public int PressUpBinarySearch(double beat, int keyIndex) => InternalBinarySearch(pressUpBeat, beat, keyIndex);

        int InternalBinarySearch(List<List<double>> list, double beat, int keyIndex)
        {
            List<double> beats = list[keyIndex];

            if (beats.Count <= 0)
                return 0;
            else if (beat < beats[0])
                return 0;
            else if (beat >= beats[beats.Count - 1])
                return beats.Count - 1;

            int low = 0;
            int high = beats.Count - 1;

            while (low < high)
            {
                int index = (low + high) / 2;
                if (beats[index] <= beat)
                    low = index + 1;
                else
                    high = index;
            }

            return low - 1;
        }
    }
}
