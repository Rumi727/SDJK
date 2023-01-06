using SCKRM;
using SCKRM.Object;
using SCKRM.Rhythm;
using SDJK.Effect;
using SDJK.Ruleset.SDJK.Judgement;
using SDJK.Map.Ruleset.SDJK.Map;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class Note : ObjectPooling
    {
        public const float noteYSize = 1;

        public Bar bar { get; private set; }
        public int barIndex => bar.barIndex;

        public NoteFile noteFile { get; private set; }

        public double beat => noteFile.beat;
        public double holdLength => noteFile.holdLength;
        public NoteTypeFile type => noteFile.type;

        public int index { get; private set; }

        public SDJKMapFile map => bar.map;
        public EffectManager effectManager => SDJKManager.instance.effectManager;
        public PlayField playField => bar.playField;

        [SerializeField] Transform _holdNote; public Transform holdNote => _holdNote;

        public double GetNoteDis()
        {
            double globalNoteSpeed = map.effect.globalNoteSpeed.GetValue(beat);
            double fieldNoteSpeed = playField.fieldEffectFile.noteSpeed.GetValue(beat);
            double localNoteSpeed = bar.barEffectFile.noteSpeed.GetValue(beat);
            double noteDis = bar.noteDistance * globalNoteSpeed * fieldNoteSpeed * localNoteSpeed;

            return noteDis;
        }

        public double GetYPos(double noteDis, out double holdYSize, out bool allowRemove)
        {
            double y = beat;
            holdYSize = holdLength;

            int lastJudgementIndex;
            double lastJudgementBeat;
            if (type == NoteTypeFile.auto)
            {
                lastJudgementIndex = SDJKJudgementManager.instance.lastAutoJudgementIndex[bar.barIndex];
                lastJudgementBeat = SDJKJudgementManager.instance.lastAutoJudgementBeat[bar.barIndex];
            }
            else
            {
                lastJudgementIndex = SDJKJudgementManager.instance.lastJudgementIndex[bar.barIndex];
                lastJudgementBeat = SDJKJudgementManager.instance.lastJudgementBeat[bar.barIndex];
            }

            if (index <= lastJudgementIndex)
            {
                if (beat + holdLength <= lastJudgementBeat)
                {
                    allowRemove = true;
                    return 0;
                }
                else if (beat <= lastJudgementBeat)
                {
                    float cutY = (float)(RhythmManager.currentBeatScreen - beat);

                    y += cutY;
                    holdYSize -= cutY;
                }
            }

            double currentBeat = RhythmManager.currentBeatScreen;
            bool noteStop = bar.barEffectFile.noteStop.GetValue(currentBeat, out double noteStopBeat);
            double noteOffset = bar.barEffectFile.noteOffset.GetValue(currentBeat);

            if (!noteStop)
                y -= currentBeat;
            else
                y -= noteStopBeat;

            y *= noteDis;
            holdYSize *= noteDis;

            y -= noteOffset;
            y -= playField.fieldHeight * 0.5;

            allowRemove = false;
            return y;
        }

        public void PosAndHoldScaleUpdate(double y, double holdYSize)
        {
            transform.localPosition = new Vector3(0, (float)y, 0);
            holdNote.localScale = new Vector3(1, ((float)holdYSize).Clamp(0), 1);
        }

        public void Refresh(Bar bar, NoteFile noteFile, int index)
        {
            this.bar = bar;
            this.noteFile = noteFile;

            this.index = index;
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
