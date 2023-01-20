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
using SDJK.Replay;
using static TreeEditor.TreeEditorHelper;

namespace SDJK.Ruleset.SDJK.Judgement
{
    public sealed class SDJKJudgementManager : ManagerBase<SDJKJudgementManager>
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
        List<double> accuracys { get; } = new List<double>();

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

                    if (!sdjkManager.isReplay)
                    {
                        sdjkManager.createdReplay.pressNoteBeat.Add(new List<double>());
                        sdjkManager.createdReplay.hitSoundBeat.Add(new List<double>());
                    }
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

                if (instance.sdjkManager.isReplay)
                    NextHitsoundReplay();

                List<SDJKNoteFile> notes = map.notes[keyIndex];
                if (currentNoteIndex < notes.Count)
                    currentNote = notes[currentNoteIndex];

                existsInstantDeathNote = notes.FindIndex(x => x.type == SDJKNoteTypeFile.instantDeath) != -1;
            }

            SDJKInputManager inputManager;
            SDJKMapFile map;
            int keyIndex = 0;

            bool autoNote;
            SDJKNoteFile currentNote;
            double currentNotePressBeatReplay;
            double currentHitsoundBeatReplay;
            int currentNoteIndex = -1;
            int currentNotePressBeatReplayIndex = -1;
            int currentHitsoundBeatReplayIndex = -1;

            /// <summary>
            /// lastJudgementBeat[keyIndex]
            /// </summary>
            List<int> lastJudgementIndex;
            List<double> lastJudgementBeat;

            bool existsInstantDeathNote = false;

            public void Update()
            {
                SDJKManager sdjkManager = instance.sdjkManager;
                SDJKRuleset ruleset = sdjkManager.ruleset;
                List<SDJKNoteFile> notes = map.notes[keyIndex];
                //SCKRM.Rhythm.BeatValuePairList<bool> yukiModes = map.globalEffect.yukiMode;
                double missSecond = ruleset.judgementMetaDatas.Last().sizeSecond;

                double currentBeat = RhythmManager.currentBeatSound;

                if (currentNoteIndex < notes.Count)
                {
                    bool input;
                    SetDisSecond(currentNote.beat, true, out double realDisSecond, out double judgementDisSecond, currentNotePressBeatReplay);

                    if (autoNote || instance.auto)
                        input = currentBeat >= currentNote.beat;
                    else if (sdjkManager.isReplay)
                    {
                        if (currentNotePressBeatReplayIndex < sdjkManager.currentReplay.pressNoteBeat[keyIndex].Count)
                            input = currentBeat >= currentNotePressBeatReplay;
                        else
                            input = false;

                        while (currentBeat >= currentHitsoundBeatReplay && currentHitsoundBeatReplayIndex < sdjkManager.currentReplay.hitSoundBeat[keyIndex].Count)
                        {
                            input = true;

                            HitsoundPlay();
                            NextHitsoundReplay();
                        }
                    }
                    else
                        input = inputManager.GetKey(keyIndex);

                    if (input && !sdjkManager.isReplay)
                    {
                        HitsoundPlay();
                        sdjkManager.createdReplay.hitSoundBeat[keyIndex].Add(currentBeat);
                    }

                    for (int i = currentNoteIndex; (realDisSecond >= missSecond || input) && i < notes.Count; i++)
                    {
                        if (Judgement(currentNote.beat, judgementDisSecond, false, out JudgementMetaData metaData, false, currentNote.type))
                        {
                            bool isMiss = metaData.nameKey == SDJKRuleset.miss;
                            if (currentNote.holdLength > 0)
                            {
                                double holdBeat = currentNote.beat + currentNote.holdLength;

                                if (!isMiss) //미스가 아닐경우 홀드 노트 진행
                                {
                                    HoldJudgementObject holdJudgementObject = new HoldJudgementObject(this, inputManager, keyIndex, autoNote, holdBeat, currentNote.type);
                                    instance.holdJudgements.Add(holdJudgementObject);

                                    holdJudgementObject.Update();
                                }
                                else //미스 일경우 홀드 노트 패스
                                    lastJudgementBeat[keyIndex] = lastJudgementBeat[keyIndex].Max(holdBeat);
                            }

                            lastJudgementIndex[keyIndex] = currentNoteIndex;
                            NextNote();
                        }

                        SetDisSecond(currentNote.beat, true, out realDisSecond, out judgementDisSecond, currentNotePressBeatReplay);
                        input = false;
                    }
                }
            }

            public void SetDisSecond(double beat, bool maxClamp, out double realDisSecond, out double judgementDisSecond, double notePressBeatReplay)
            {
                realDisSecond = GetDisSecond(beat, maxClamp, RhythmManager.currentBeatSound);
                judgementDisSecond = realDisSecond;

                if (autoNote || instance.auto)
                    judgementDisSecond = 0;
                else if (instance.sdjkManager.isReplay)
                    judgementDisSecond = GetDisSecond(beat, maxClamp, notePressBeatReplay);
            }

            public bool Judgement(double beat, double disSecond, bool forceFastMiss, out JudgementMetaData metaData, bool isHoldJudgement, SDJKNoteTypeFile noteType)
            {
                SDJKManager sdjkManager = instance.sdjkManager;
                SDJKRuleset ruleset = instance.sdjkManager.ruleset;
                List<SDJKNoteFile> notes = map.notes[keyIndex];
                double missSecond = ruleset.judgementMetaDatas.Last().sizeSecond;

                double currentBeat = RhythmManager.currentBeatSound;
                double bpmDivide60 = RhythmManager.bpm / 60d;

                if (ruleset.Judgement(disSecond, forceFastMiss, out metaData))
                {
                    //리플레이 파일에 노트 기록 저장
                    if (!sdjkManager.isReplay && !isHoldJudgement && noteType != SDJKNoteTypeFile.auto)
                        sdjkManager.createdReplay.pressNoteBeat[keyIndex].Add(currentBeat);

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
                        instance.health -= map.globalEffect.hpMissValue.GetValue(currentBeat) * metaData.hpMultiplier;
                    else
                        instance.health += map.globalEffect.hpAddValue.GetValue(currentBeat) * metaData.hpMultiplier;

                    if (!sdjkManager.isReplay)
                    {
                        if (instance.health <= 0)
                        {
                            instance.gameOverManager.GameOver();
                            sdjkManager.createdReplay.gameOverBeat = currentBeat;
                        }
                    }
                    else if (currentBeat >= sdjkManager.createdReplay.gameOverBeat)
                        instance.gameOverManager.GameOver();

                    instance.accuracys.Add(disSecond.Abs().Clamp(0, missSecond) / missSecond);
                    instance.accuracy = instance.accuracys.Average();

                    if (!sdjkManager.isReplay)
                        CreatedReplayFileAdd();
                    else
                        GetReplayComboToSet();

                    instance.judgementAction?.Invoke(disSecond, isMiss, metaData);
                    return true;
                }
                else if (existsInstantDeathNote) //즉사 노트가 존재하며 노트를 치지 않았을때
                {
                    //가장 가까운 즉사 노트 감지
                    double instantDeathNoteBeat = notes.CloseValue(currentBeat, x => x.beat, x => x.type == SDJKNoteTypeFile.instantDeath);
                    double dis = GetDisSecond(instantDeathNoteBeat, false, currentNotePressBeatReplay);

                    if (dis.Abs() <= missSecond)
                    {
                        instance.combo = 0;
                        instance.health = 0;
                        instance.gameOverManager.GameOver();

                        if (!sdjkManager.isReplay)
                            CreatedReplayFileAdd();

                        instance.judgementAction?.Invoke(dis, true, ruleset.instantDeathJudgementMetaData);
                    }
                }

                return false;
            }

            public void CreatedReplayFileAdd()
            {
                SDJKManager sdjkManager = instance.sdjkManager;
                double currentBeat = RhythmManager.currentBeatSound;

                sdjkManager.createdReplay.combos.Add(currentBeat, instance.combo);
                sdjkManager.createdReplay.scores.Add(currentBeat, instance.score);
                sdjkManager.createdReplay.healths.Add(currentBeat, instance.health);
                sdjkManager.createdReplay.accuracys.Add(currentBeat, instance.accuracy);
            }

            public void GetReplayComboToSet()
            {
                SDJKManager sdjkManager = instance.sdjkManager;
                double currentBeat = RhythmManager.currentBeatSound;

                instance.combo = sdjkManager.currentReplay.combos.GetValue(currentBeat);
                instance.score = sdjkManager.currentReplay.scores.GetValue(currentBeat);
                instance.health = sdjkManager.currentReplay.healths.GetValue(currentBeat);
                instance.accuracy = sdjkManager.currentReplay.accuracys.GetValue(currentBeat);
            }

            public void HitsoundPlay() => SoundManager.PlaySound("hitsound.normal", "sdjk", 0.5f, false, 0.95f);
            public void NextHitsoundReplay()
            {
                SDJKManager sdjkManager = instance.sdjkManager;
                List<double> replayHitsound = sdjkManager.currentReplay.hitSoundBeat[keyIndex];

                currentHitsoundBeatReplayIndex++;
                if (currentHitsoundBeatReplayIndex < replayHitsound.Count)
                    currentHitsoundBeatReplay = replayHitsound[currentHitsoundBeatReplayIndex];
            }

            public double GetDisSecond(double beat, bool maxClamp, double currentBeat)
            {
                SDJKManager sdjkManager = instance.sdjkManager;
                SDJKRuleset ruleset = sdjkManager.ruleset;
                double missSecond = ruleset.judgementMetaDatas.Last().sizeSecond;

                double bpmDivide60 = RhythmManager.bpm / 60d;
                double value = (currentBeat - beat) / bpmDivide60 / (RhythmManager.currentSpeed * Kernel.gameSpeed);
                if (maxClamp)
                    return value.Clamp(double.MinValue, missSecond);
                else
                    return value;
            }

            void NextNote()
            {
                SDJKManager sdjkManager = instance.sdjkManager;
                List<SDJKNoteFile> notes = map.notes[keyIndex];

                for (int i = currentNoteIndex; i <= notes.Count; i++)
                {
                    currentNoteIndex++;

                    if (currentNoteIndex < notes.Count)
                    {
                        currentNote = notes[currentNoteIndex];

                        if (autoNote)
                        {
                            if (currentNote.type == SDJKNoteTypeFile.auto)
                                break;
                        }
                        else
                        {
                            if (currentNote.type != SDJKNoteTypeFile.instantDeath && currentNote.type != SDJKNoteTypeFile.auto)
                                break;
                        }
                    }
                }

                if (sdjkManager.isReplay)
                {
                    List<double> replayNote = sdjkManager.currentReplay.pressNoteBeat[keyIndex];

                    currentNotePressBeatReplayIndex++;
                    if (currentNotePressBeatReplayIndex < replayNote.Count)
                        currentNotePressBeatReplay = replayNote[currentNotePressBeatReplayIndex];
                }
            }
        }

        class HoldJudgementObject
        {
            public HoldJudgementObject(JudgementObject judgementObject, SDJKInputManager inputManager, int keyIndex, bool autoNote, double currentHoldNoteBeat, SDJKNoteTypeFile noteType)
            {
                this.judgementObject = judgementObject;
                this.inputManager = inputManager;
                this.keyIndex = keyIndex;

                this.autoNote = autoNote;
                this.currentHoldNoteBeat = currentHoldNoteBeat;

                this.noteType = noteType;
            }

            JudgementObject judgementObject;
            SDJKInputManager inputManager;
            int keyIndex = 0;

            bool autoNote;
            double currentHoldNoteBeat;

            bool isRemove = false;

            SDJKNoteTypeFile noteType;
            public void Update()
            {
                if (isRemove)
                    return;

                SDJKManager sdjkManager = instance.sdjkManager;
                SDJKRuleset ruleset = sdjkManager.ruleset;
                double missSecond = ruleset.judgementMetaDatas.Last().sizeSecond;

                double currentBeat = RhythmManager.currentBeatSound;

                bool input;
                double replayInputBeat = 0;
                if (autoNote || instance.auto)
                    input = currentBeat <= currentHoldNoteBeat;
                else if (sdjkManager.isReplay)
                {
                    input = inputManager.ReplayGetKey(keyIndex, currentBeat, out double beat);
                    replayInputBeat = beat;
                }
                else
                    input = inputManager.GetKey(keyIndex, InputType.Alway);

                judgementObject.SetDisSecond(currentHoldNoteBeat, true, out double realDisSecond, out double judgementDisSecond, replayInputBeat);

                if (realDisSecond >= missSecond || !input)
                {
                    judgementObject.Judgement(currentHoldNoteBeat, judgementDisSecond, true, out _, true, noteType);
                    judgementObject.HitsoundPlay();

                    instance.holdJudgements.Remove(this);
                    isRemove = true;
                }
            }
        }
    }
}
