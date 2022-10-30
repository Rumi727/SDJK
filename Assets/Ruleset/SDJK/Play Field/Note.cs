using SCKRM.Object;
using SDJK.Effect;
using SDJK.Ruleset.SDJK.Map;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class Note : ObjectPooling
    {
        public Bar bar { get; private set; }
        public int barIndex => bar.barIndex;

        public NoteFile noteFile { get; private set; }

        public double beat => noteFile.beat;
        public double holdLength => noteFile.holdLength;
        public NoteTypeFile type => noteFile.type;

        public SDJKMapFile map => bar.map;
        public EffectManager effectManager => bar.effectManager;

        public void Refresh(Bar bar, NoteFile noteFile)
        {
            this.bar = bar;
            this.noteFile = noteFile;
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            bar = null;
            noteFile = default;

            transform.localPosition = Vector3.zero;
            return true;
        }
    }
}
