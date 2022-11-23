using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
using SDJK.Effect;
using SDJK.Ruleset.SDJK.Input;
using SDJK.Ruleset.SDJK.Map;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace SDJK.Ruleset.SDJK.Judgement
{
    public sealed class SDJKJudgementManager : Manager<SDJKJudgementManager>
    {
        [SerializeField] SDJKInputManager _inputManager; public SDJKInputManager inputManager => _inputManager;
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;
        public SDJKMapFile map => (SDJKMapFile)effectManager.selectedMap;

        /// <summary>
        /// lastJudgementBeat[keyIndex]
        /// </summary>
        public List<double> lastJudgementBeat = new List<double>();

        List<JudgementObject> judgements = new List<JudgementObject>();
        public void Refresh()
        {
            if (SingletonCheck(this))
            {
                lastJudgementBeat.Clear();
                judgements.Clear();
                for (int i = 0; i < map.notes.Count; i++)
                {
                    judgements.Add(new JudgementObject(inputManager, map, i));
                    lastJudgementBeat.Add(double.MinValue);
                }
            }
        }

        void Update()
        {
            if (instance != null)
            {
                for (int i = 0; i < judgements.Count; i++)
                    judgements[i].Update();
            }
        }

        class JudgementObject
        {
            public JudgementObject(SDJKInputManager inputManager, SDJKMapFile map, int keyIndex)
            {
                this.inputManager = inputManager;
                this.map = map;
                this.keyIndex = keyIndex;

                List<NoteFile> notes = map.notes[keyIndex];
                if (currentNoteIndex < notes.Count)
                    currentNote = notes[currentNoteIndex];
            }

            SDJKInputManager inputManager;
            SDJKMapFile map;
            int keyIndex = 0;

            NoteFile currentNote;
            int currentNoteIndex = 0;
            NoteFile currentHoldNote;
            double currentHoldBeat = 0;
            bool isHold = false;

            public void Update()
            {
                List<NoteFile> notes = map.notes[keyIndex];
                if (currentNoteIndex < notes.Count)
                {
                    double currentBeat = RhythmManager.currentBeatSound;
                    double bpmDivide60 = RhythmManager.bpm / 60d;

                    bool input = inputManager.GetKey(keyIndex, InputType.Down);
                    double disSecond = getDis(currentNote.beat);

                    for (int i = 0; (disSecond >= 0.1f || input) && (i < notes.Count); i++)
                    {
                        if (Judgement(currentNote.beat, disSecond, false))
                        {
                            if (currentNote.holdLength > 0)
                            {
                                isHold = true;
                                currentHoldBeat = currentNote.beat + currentNote.holdLength;
                                currentHoldNote = currentNote;
                            }

                            NextNote();
                        }

                        disSecond = getDis(currentNote.beat);
                        input = false;
                    }

                    if (isHold)
                    {
                        double holdDisSecond = ((currentBeat - currentHoldBeat) / bpmDivide60).Clamp(double.MinValue, 0.1f);
                        if (holdDisSecond >= 0.1f || !inputManager.GetKey(keyIndex, InputType.Alway))
                        {
                            isHold = false;
                            Judgement(currentHoldBeat, holdDisSecond, true);
                        }
                    }


                    double getDis(double beat) => ((currentBeat - beat) / bpmDivide60).Clamp(double.MinValue, 0.1f);
                }
            }

            bool Judgement(double beat, double disSecond, bool forceFastMiss)
            {
                if (disSecond >= -0.1f || forceFastMiss)
                {
                    Debug.Log(disSecond);
                    instance.lastJudgementBeat[keyIndex] = beat;
                    return true;
                }

                return false;
            }

            void NextNote()
            {
                List<NoteFile> notes = map.notes[keyIndex];

                for (int i = currentNoteIndex; i <= notes.Count; i++)
                {
                    currentNoteIndex++;

                    if (currentNoteIndex < notes.Count)
                    {
                        currentNote = notes[currentNoteIndex];

                        if (currentNote.type != NoteTypeFile.instantDeath)
                            break;
                    }
                }
            }
        }
    }
}
