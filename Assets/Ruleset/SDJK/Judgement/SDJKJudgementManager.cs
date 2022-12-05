using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
using SDJK.Effect;
using SDJK.Ruleset.SDJK.Input;
using SDJK.Ruleset.SDJK.Map;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace SDJK.Ruleset.SDJK.Judgement
{
    public sealed class SDJKJudgementManager : Manager<SDJKJudgementManager>
    {
        [SerializeField] SDJKManager _sdjkManager; public SDJKManager sdjkManager => _sdjkManager;
        [SerializeField] SDJKInputManager _inputManager; public SDJKInputManager inputManager => _inputManager;
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;
        public SDJKMapFile map => (SDJKMapFile)effectManager.selectedMap;
        public int combo { get; private set; }

        /// <summary>
        /// lastJudgementBeat[keyIndex]
        /// </summary>
        public List<double> lastJudgementBeat { get; } = new List<double>();



        public event JudgementAction judgementAction;
        public delegate void JudgementAction(double disSecond, bool isMiss, JudgementMetaData metaData);



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
                SDJKRuleset ruleset = instance.sdjkManager.ruleset;
                List<NoteFile> notes = map.notes[keyIndex];
                double missSecond = ruleset.judgementMetaDatas.Last().sizeSecond;
                if (currentNoteIndex < notes.Count)
                {
                    double currentBeat = RhythmManager.currentBeatSound;
                    double bpmDivide60 = RhythmManager.bpm / 60d;

                    bool input = inputManager.GetKey(keyIndex, InputType.Down);
                    double disSecond = getDisSecond(currentNote.beat);

                    for (int i = 0; (disSecond >= missSecond || input) && (i < notes.Count); i++)
                    {
                        if (Judgement(currentNote.beat, disSecond, false, out JudgementMetaData metaData))
                        {
                            bool isMiss = metaData.nameKey == SDJKRuleset.miss;
                            if (!isMiss)
                            {
                                if (currentNote.holdLength > 0)
                                {
                                    isHold = true;
                                    currentHoldBeat = currentNote.beat + currentNote.holdLength;
                                    currentHoldNote = currentNote;
                                }

                                instance.combo++;
                            }

                            instance.judgementAction?.Invoke(disSecond, isMiss, metaData);

                            NextNote();
                        }

                        disSecond = getDisSecond(currentNote.beat);
                        input = false;
                    }

                    if (isHold)
                    {
                        double holdDisSecond = ((currentBeat - currentHoldBeat) / bpmDivide60).Clamp(double.MinValue, missSecond);
                        if (holdDisSecond >= missSecond || !inputManager.GetKey(keyIndex, InputType.Alway))
                        {
                            isHold = false;
                            Judgement(currentHoldBeat, holdDisSecond, true, out JudgementMetaData metaData);

                            bool isMiss = metaData.nameKey == SDJKRuleset.miss;
                            if (!isMiss)
                                instance.combo++;

                            instance.judgementAction?.Invoke(disSecond, isMiss, metaData);
                        }
                    }


                    //beat 인자랑 currentBeat 변수간의 거리를 계산하고, 계산된 결과를 초로 변환하여 반환합니다
                    double getDisSecond(double beat) => ((currentBeat - beat) / bpmDivide60).Clamp(double.MinValue, missSecond);
                }
            }

            bool Judgement(double beat, double disSecond, bool forceFastMiss, out JudgementMetaData metaData)
            {
                if (instance.sdjkManager.ruleset.Judgement(disSecond, forceFastMiss, out metaData))
                {
                    Debug.Log(disSecond);
                    Debug.Log(metaData.nameKey);
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
