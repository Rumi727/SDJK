using SCKRM.Rhythm;

namespace SDJK.Map
{
    public class NoteConfigFile
    {
        public NoteConfigFile() { }

        public NoteConfigFile(BeatValuePairAniListDouble noteSpeed, params string[] hitsoundFile)
        {
            this.noteSpeed = noteSpeed;
            this.hitsoundFile = hitsoundFile;
        }

        public BeatValuePairAniListDouble noteSpeed { get; set; } = new BeatValuePairAniListDouble(1);
        public string[] hitsoundFile { get; set; } = null;
    }
}
