using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
using SDJK.Effect;
using SDJK.Ruleset.SDJK.Effect;
using SDJK.Ruleset.SDJK.Input;
using SDJK.Ruleset.SDJK.Map;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.Judgement
{
    public sealed class SDJKJudgementManager : Manager<SDJKJudgementManager>
    {
        [SerializeField] SDJKManager _sdjkManager; public SDJKManager sdjkManager => _sdjkManager;
        [SerializeField] SDJKInputManager _inputManager; public SDJKInputManager inputManager => _inputManager;
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;
        [SerializeField] SDJKGameOverManager _gameOverManager; public SDJKGameOverManager gameOverManager => _gameOverManager;

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
        /// lastJudgementIndex[keyIndex]
        /// </summary>
        public List<int> lastJudgementIndex { get; } = new List<int>();
        /// <summary>
        /// lastAutoJudgementIndex[keyIndex]
        /// </summary>
        public List<int> lastAutoJudgementIndex { get; } = new List<int>();

        /// <summary>
        /// lastJudgementBeat[keyIndex]
        /// </summary>
        public List<double> lastJudgementBeat { get; } = new List<double>();
        /// <summary>
        /// lastAutoJudgementBeat[keyIndex]
        /// </summary>
        public List<double> lastAutoJudgementBeat { get; } = new List<double>();



        public event JudgementAction judgementAction;
        public delegate void JudgementAction(double disSecond, bool isMiss, JudgementMetaData metaData);



        List<JudgementObject> judgements = new List<JudgementObject>();
        public void Refresh()
        {
            if (SingletonCheck(this))
            {
                judgements.Clear();

                lastJudgementIndex.Clear();
                lastAutoJudgementIndex.Clear();

                lastJudgementBeat.Clear();
                lastAutoJudgementBeat.Clear();

                for (int i = 0; i < map.notes.Count; i++)
                {
                    judgements.Add(new JudgementObject(inputManager, map, i, lastJudgementIndex, lastJudgementBeat, false));
                    judgements.Add(new JudgementObject(inputManager, map, i, lastAutoJudgementIndex, lastAutoJudgementBeat, true));

                    lastJudgementIndex.Add(int.MinValue);
                    lastAutoJudgementIndex.Add(int.MinValue);

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
                //게임 오버 상태에선 판정하면 안됩니다
                if (!gameOverManager.isGameOver)
                {
                    for (int i = 0; i < judgements.Count; i++)
                        judgements[i].Update();
                }

                if (RhythmManager.currentBeat >= 0)
                    instance.health -= map.globalEffect.hpRemoveValue.GetValue() * RhythmManager.bpmDeltaTime;
            }
        }

        class JudgementObject
        {
            public JudgementObject(SDJKInputManager inputManager, SDJKMapFile map, int keyIndex, List<int> lastJudgementIndex, List<double> lastJudgementBeat, bool autoNote)
            {
                this.inputManager = inputManager;
                this.map = map;
                this.keyIndex = keyIndex;
                this.lastJudgementIndex = lastJudgementIndex;
                this.lastJudgementBeat = lastJudgementBeat;
                this.autoNote = autoNote;

                NextNote();

                List<NoteFile> notes = map.notes[keyIndex];
                if (currentNoteIndex < notes.Count)
                    currentNote = notes[currentNoteIndex];
            }

            SDJKInputManager inputManager;
            SDJKMapFile map;
            int keyIndex = 0;

            bool autoNote;
            NoteFile currentNote;
            int currentNoteIndex = -1;
            double currentHoldBeat = 0;
            bool isHold = false;

            /// <summary>
            /// lastJudgementBeat[keyIndex]
            /// </summary>
            List<int> lastJudgementIndex;
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
                    double disSecond = GetDisSecond(currentNote.beat, true);

                    if (autoNote)
                    {
                        input = currentBeat >= currentNote.beat;
                        disSecond = 0;
                    }
                    else
                        input = inputManager.GetKey(keyIndex, InputType.Down);

                    for (int i = currentNoteIndex; (disSecond >= missSecond || input) && i < notes.Count; i++)
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

                            lastJudgementIndex[keyIndex] = currentNoteIndex;
                            NextNote();
                        }

                        disSecond = GetDisSecond(currentNote.beat, true);
                        input = false;
                    }
                }

                if (isHold)
                {
                    double holdDisSecond = GetDisSecond(currentHoldBeat, true);
                    bool holdInput;

                    if (autoNote)
                    {
                        holdInput = currentBeat <= currentHoldBeat;
                        holdDisSecond = 0;
                    }
                    else
                        holdInput = inputManager.GetKey(keyIndex, InputType.Alway);

                    if (holdDisSecond >= missSecond || !holdInput)
                    {
                        isHold = false;
                        Judgement(currentHoldBeat, holdDisSecond, true, out _);
                    }
                }

                bool Judgement(double beat, double disSecond, bool forceFastMiss, out JudgementMetaData metaData)
                {
                    if (instance.sdjkManager.ruleset.Judgement(disSecond, forceFastMiss, out metaData))
                    {
                        lastJudgementBeat[keyIndex] = beat;

                        bool isMiss = metaData.nameKey == SDJKRuleset.miss;
                        if (!isMiss)
                        {
                            instance.combo++;
                            instance.health += map.globalEffect.hpAddValue.GetValue();
                        }
                        else
                        {
                            instance.combo = 0;
                            instance.health -= map.globalEffect.hpMissValue.GetValue();
                        }

                        if (instance.health <= 0)
                            instance.gameOverManager.GameOver();

                        instance.judgementAction?.Invoke(disSecond, isMiss, metaData);
                        return true;
                    }
                    else //노트를 치지 않았을때
                    {
                        //가장 가까운 즉사 노트 감지
                        double instantDeathNoteBeat = notes.CloseValue(currentBeat, x => x.beat, x => x.type == NoteTypeFile.instantDeath);
                        double dis = GetDisSecond(instantDeathNoteBeat, false);
                        
                        if (dis.Abs() <= missSecond)
                        {
                            instance.health = 0;
                            instance.gameOverManager.GameOver();

                            instance.judgementAction?.Invoke(dis, true, ruleset.missJudgementMetaData);
                        }
                    }

                    return false;
                }

                //beat 인자랑 currentBeat 변수간의 거리를 계산하고, 계산된 결과를 초로 변환하여 반환합니다
                double GetDisSecond(double beat, bool maxClamp)
                {
                    double value = (currentBeat - beat) / bpmDivide60 / (RhythmManager.currentSpeed * Kernel.gameSpeed);
                    if (maxClamp)
                        return value.Clamp(double.MinValue, missSecond);
                    else
                        return value;
                }
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
