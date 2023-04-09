using SCKRM.Rhythm;

namespace SDJK.Map
{
    public struct NoteConfigFile
    {
        public NoteConfigFile(BeatValuePairAniListDouble noteSpeed, params string[] hitsoundFile)
        {
            this.noteSpeed = noteSpeed;
            this.hitsoundFile = hitsoundFile;
        }

        public BeatValuePairAniListDouble noteSpeed { get; set; }
        public string[] hitsoundFile { get; set; }
    }
}
