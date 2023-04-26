using SCKRM.Json;
using SCKRM.Rhythm;
using System.Collections.Generic;

namespace SDJK.Map
{
    public class NoteConfigFile
    {
        public BeatValuePairAniListDouble noteSpeed { get; set; } = new BeatValuePairAniListDouble(1);
        public BeatValuePairAniListColor noteColor { get; set; } = new BeatValuePairAniListColor(new JColor(1, 1, 1));

        public TypeList<string> hitsoundFile { get; set; } = new TypeList<string>();
    }
}
