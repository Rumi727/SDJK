using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SDJK.Effect;
using SDJK.Ruleset.SDJK.Effect;
using SDJK.Ruleset.SDJK.Input;
using SDJK.Map.Ruleset.SDJK.Map;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.Judgement
{
    public sealed class SDJKJudgementManager : Manager<SDJKJudgementManager>
    {
        [SerializeField] SDJKManager _sdjkManager; public SDJKManager sdjkManager => _sdjkManager;
        [SerializeField] SDJKInputManager _inputManager; public SDJKInputManager inputManager => _inputManager;
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;
        [SerializeField] SDJKGameOverManager _gameOverManager; public SDJKGameOverManager gameOverManager => _gameOverManager;
        [SerializeField] bool auto = false;

        public SDJKMapFile map => (SDJKMapFile)effectManager.selectedMap;



        public int combo { get; private set; }

        public double score { get; private set; }
        public const double scoreMultiplier = 10000000;

        /// <summary>
        /// 0 ~ 1 (0에 가까울수록 정확함)
        /// </summary>
        public double accuracy { get; private set; } = 0;
        public List<double> accuracys { get; } = new List<double>();

        public double health 
        { 
            get => _health; 
            private set => _health = value.Clamp(0, maxHealth); 
        }
        double _health = maxHealth;

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
        List<HoldJudgementObject> holdJudgements = new List<HoldJudgementObject>();
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

                    for (int i = 0; i < holdJudgements.Count; i++)
                        holdJudgements[i].Update();
                }

                if (RhythmManager.currentBeatSound >= 0)
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

            /// <summary>
            /// lastJudgementBeat[keyIndex]
            /// </summary>
            List<int> lastJudgementIndex;
            List<double> lastJudgementBeat;

            public void Update()
            {
                SDJKRuleset ruleset = instance.sdjkManager.ruleset;
                List<NoteFile> notes = map.notes[keyIndex];
                //SCKRM.Rhythm.BeatValuePairList<bool> yukiModes = map.globalEffect.yukiMode;
                double missSecond = ruleset.judgementMetaDatas.Last().sizeSecond;

                double currentBeat = RhythmManager.currentBeatSound;

                if (currentNoteIndex < notes.Count)
                {
                    bool input;
                    SetDisSecond(currentNote.beat, true, out double realDisSecond, out double judgementDisSecond);

                    if (autoNote || instance.auto)
                        input = currentBeat >= currentNote.beat;
                    else
                        input = inputManager.GetKey(keyIndex, InputType.Down);

                    if (input)
                        HitsoundPlay();

                    for (int i = currentNoteIndex; (realDisSecond >= missSecond || input) && i < notes.Count; i++)
                    {
                        if (Judgement(currentNote.beat, judgementDisSecond, false, out JudgementMetaData metaData))
                        {
                            bool isMiss = metaData.nameKey == SDJKRuleset.miss;
                            if (currentNote.holdLength > 0)
                            {
                                double holdBeat = currentNote.beat + currentNote.holdLength;

                                if (!isMiss) //미스가 아닐경우 홀드 노트 진행
                                {
                                    HoldJudgementObject holdJudgementObject = new HoldJudgementObject(this, inputManager, keyIndex, autoNote, holdBeat);
                                    instance.holdJudgements.Add(holdJudgementObject);

                                    holdJudgementObject.Update();
                                }
                                else //미스 일경우 홀드 노트 패스
                                    lastJudgementBeat[keyIndex] = lastJudgementBeat[keyIndex].Max(holdBeat);
                            }

                            lastJudgementIndex[keyIndex] = currentNoteIndex;
                            NextNote();
                        }

                        SetDisSecond(currentNote.beat, true, out realDisSecond, out judgementDisSecond);
                        input = false;
                    }
                }
            }

            public void SetDisSecond(double beat, bool maxClamp, out double realDisSecond, out double judgementDisSecond)
            {
                realDisSecond = GetDisSecond(beat, maxClamp);
                judgementDisSecond = realDisSecond;

                if (autoNote || instance.auto)
                    judgementDisSecond = 0;
            }

            public bool Judgement(double beat, double disSecond, bool forceFastMiss, out JudgementMetaData metaData)
            {
                SDJKRuleset ruleset = instance.sdjkManager.ruleset;
                List<NoteFile> notes = map.notes[keyIndex];
                double missSecond = ruleset.judgementMetaDatas.Last().sizeSecond;

                double currentBeat = RhythmManager.currentBeatSound;
                double bpmDivide60 = RhythmManager.bpm / 60d;

                if (instance.sdjkManager.ruleset.Judgement(disSecond, forceFastMiss, out metaData))
                {
                    lastJudgementBeat[keyIndex] = lastJudgementBeat[keyIndex].Max(beat);
                    bool isMiss = metaData.nameKey == SDJKRuleset.miss;

                    if (!isMiss)
                    {
                        instance.combo += 1;
                        instance.score += ruleset.GetScoreAddValue(disSecond, map.allJudgmentBeat.Count) * instance.combo * scoreMultiplier;
                    }
                    else
                        instance.combo = 0;

                    if (isMiss || metaData.missHp)
                        instance.health -= map.globalEffect.hpMissValue.GetValue() * metaData.hpMultiplier;
                    else
                        instance.health += map.globalEffect.hpAddValue.GetValue() * metaData.hpMultiplier;

                    if (instance.health <= 0)
                        instance.gameOverManager.GameOver();

                    instance.accuracys.Add(disSecond.Abs().Clamp(0, missSecond) / missSecond);
                    instance.accuracy = instance.accuracys.Average();

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
                        instance.combo = 0;
                        instance.health = 0;
                        instance.gameOverManager.GameOver();

                        instance.judgementAction?.Invoke(dis, true, ruleset.instantDeathJudgementMetaData);
                    }
                }

                return false;
            }

            public void HitsoundPlay() => SoundManager.PlaySound("hitsound.normal", "sdjk", 0.5f, false, 0.95f);

            public double GetDisSecond(double beat, bool maxClamp)
            {
                SDJKRuleset ruleset = instance.sdjkManager.ruleset;
                double missSecond = ruleset.judgementMetaDatas.Last().sizeSecond;

                double currentBeat = RhythmManager.currentBeatSound;
                double bpmDivide60 = RhythmManager.bpm / 60d;

                double value = (currentBeat - beat) / bpmDivide60 / (RhythmManager.currentSpeed * Kernel.gameSpeed);
                if (maxClamp)
                    return value.Clamp(double.MinValue, missSecond);
                else
                    return value;
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

        class HoldJudgementObject
        {
            public HoldJudgementObject(JudgementObject judgementObject, SDJKInputManager inputManager, int keyIndex, bool autoNote, double currentHoldNoteBeat)
            {
                this.judgementObject = judgementObject;
                this.inputManager = inputManager;
                this.keyIndex = keyIndex;

                this.autoNote = autoNote;
                this.currentHoldNoteBeat = currentHoldNoteBeat;
            }

            JudgementObject judgementObject;
            SDJKInputManager inputManager;
            int keyIndex = 0;

            bool autoNote;
            double currentHoldNoteBeat;

            public void Update()
            {
                SDJKRuleset ruleset = instance.sdjkManager.ruleset;
                double missSecond = ruleset.judgementMetaDatas.Last().sizeSecond;

                double currentBeat = RhythmManager.currentBeatSound;

                judgementObject.SetDisSecond(currentHoldNoteBeat, true, out double realDisSecond, out double judgementDisSecond);
                bool input;

                if (autoNote || instance.auto)
                    input = currentBeat <= currentHoldNoteBeat;
                else
                    input = inputManager.GetKey(keyIndex, InputType.Alway);

                if (realDisSecond >= missSecond || !input)
                {
                    judgementObject.Judgement(currentHoldNoteBeat, judgementDisSecond, true, out _);
                    judgementObject.HitsoundPlay();

                    instance.holdJudgements.Remove(this);
                }
            }
        }
    }
}
