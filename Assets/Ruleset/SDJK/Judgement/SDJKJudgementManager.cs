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

        public double health 
        { 
            get => _health; 
            private set => _health = value.Clamp(0, maxHealth); 
        }
        private double _health = maxHealth;

        public const double maxHealth = 100;



        /// <summary>
        /// lastJudgementBeat[keyIndex]
        /// </summary>
        public List<double> lastJudgementBeat { get; } = new List<double>();
        public List<double> lastAutoJudgementBeat { get; } = new List<double>();



        public event JudgementAction judgementAction;
        public delegate void JudgementAction(double disSecond, bool isMiss, JudgementMetaData metaData);



        List<JudgementObject> judgements = new List<JudgementObject>();
        public void Refresh()
        {
            if (SingletonCheck(this))
            {
                judgements.Clear();

                lastJudgementBeat.Clear();
                lastAutoJudgementBeat.Clear();

                for (int i = 0; i < map.notes.Count; i++)
                {
                    judgements.Add(new JudgementObject(inputManager, map, i, lastJudgementBeat, false));
                    judgements.Add(new JudgementObject(inputManager, map, i, lastAutoJudgementBeat, true));

                    lastJudgementBeat.Add(double.MinValue);
                    lastAutoJudgementBeat.Add(double.MinValue);
                }
            }
        }

        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            if (instance != null)
            {
                for (int i = 0; i < judgements.Count; i++)
                    judgements[i].Update();

                if (RhythmManager.currentBeat >= 0)
                    instance.health -= map.effect.hpRemoveValue.GetValue() * RhythmManager.bpmDeltaTime;
            }
        }

        class JudgementObject
        {
            public JudgementObject(SDJKInputManager inputManager, SDJKMapFile map, int keyIndex, List<double> lastJudgementBeat, bool autoNote)
            {
                this.inputManager = inputManager;
                this.map = map;
                this.keyIndex = keyIndex;
                this.lastJudgementBeat = lastJudgementBeat;
                this.autoNote = autoNote;

                List<NoteFile> notes = map.notes[keyIndex];
                if (currentNoteIndex < notes.Count)
                    currentNote = notes[currentNoteIndex];
            }

            SDJKInputManager inputManager;
            SDJKMapFile map;
            int keyIndex = 0;

            bool autoNote;
            NoteFile currentNote;
            int currentNoteIndex = 0;
            double currentHoldBeat = 0;
            bool isHold = false;

            /// <summary>
            /// lastJudgementBeat[keyIndex]
            /// </summary>
            List<double> lastJudgementBeat;

            public void Update()
            {
                SDJKRuleset ruleset = instance.sdjkManager.ruleset;
                List<NoteFile> notes = map.notes[keyIndex];
                double missSecond = ruleset.judgementMetaDatas.Last().sizeSecond;

                double currentBeat = RhythmManager.currentBeatSound;
                double bpmDivide60 = RhythmManager.bpm / 60d;

                if (currentNoteIndex < notes.Count)
                {
                    bool input;
                    double disSecond = getDisSecond(currentNote.beat);

                    if (autoNote)
                        input = currentBeat >= currentNote.beat;
                    else
                        input = inputManager.GetKey(keyIndex, InputType.Down);

                    for (int i = 0; (disSecond >= missSecond || input) && i < notes.Count; i++)
                    {
                        if (Judgement(currentNote.beat, disSecond, false, out JudgementMetaData metaData))
                        {
                            bool isMiss = metaData.nameKey == SDJKRuleset.miss;
                            if (currentNote.holdLength > 0)
                            {
                                if (!isMiss) //미스가 아닐경우 홀드 노트 진행
                                {
                                    isHold = true;
                                    currentHoldBeat = currentNote.beat + currentNote.holdLength;
                                }
                                else //미스 일경우 홀드 노트 패스
                                    lastJudgementBeat[keyIndex] = currentNote.beat + currentNote.holdLength;
                            }

                            NextNote();
                        }

                        disSecond = getDisSecond(currentNote.beat);
                        input = false;
                    }

                    //beat 인자랑 currentBeat 변수간의 거리를 계산하고, 계산된 결과를 초로 변환하여 반환합니다
                    double getDisSecond(double beat) => ((currentBeat - beat) / bpmDivide60).Clamp(double.MinValue, missSecond);
                }

                if (isHold)
                {
                    double holdDisSecond = ((currentBeat - currentHoldBeat) / bpmDivide60).Clamp(double.MinValue, missSecond);
                    bool holdInput;

                    if (autoNote)
                        holdInput = currentBeat <= currentHoldBeat;
                    else
                        holdInput = inputManager.GetKey(keyIndex, InputType.Alway);

                    if (holdDisSecond >= missSecond || !holdInput)
                    {
                        isHold = false;
                        Judgement(currentHoldBeat, holdDisSecond, true, out _);
                    }
                }
            }

            bool Judgement(double beat, double disSecond, bool forceFastMiss, out JudgementMetaData metaData)
            {
                if (autoNote)
                    disSecond = 0;

                if (instance.sdjkManager.ruleset.Judgement(disSecond, forceFastMiss, out metaData))
                {
                    lastJudgementBeat[keyIndex] = beat;

                    bool isMiss = metaData.nameKey == SDJKRuleset.miss;
                    if (!isMiss)
                    {
                        instance.combo++;
                        instance.health += map.effect.hpAddValue.GetValue();
                    }
                    else
                    {
                        instance.combo = 0;
                        instance.health -= map.effect.hpMissValue.GetValue();
                    }

                    instance.judgementAction?.Invoke(disSecond, isMiss, metaData);
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

                        if (autoNote)
                        {
                            if (currentNote.type == NoteTypeFile.auto)
                                break;
                        }
                        else
                        {
                            if (currentNote.type != NoteTypeFile.instantDeath && currentNote.type != NoteTypeFile.auto)
                                break;
                        }
                    }
                }
            }
        }
    }
}
