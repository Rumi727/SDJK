using SCKRM;
using SCKRM.Rhythm;
using SCKRM.Sound;
using SDJK.Effect;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SDJK.Mode;
using SDJK.Mode.Difficulty;
using SDJK.Map.Ruleset.SuperHexagon.Map;
using SDJK.Ruleset.SuperHexagon.GameOver;
using SDJK.Replay.Ruleset.SuperHexagon;

namespace SDJK.Ruleset.SuperHexagon.Judgement
{
    public sealed class SuperHexagonJudgementManager : ManagerBase<SuperHexagonJudgementManager>
    {
        [SerializeField] SuperHexagonManager _manager; public SuperHexagonManager manager => _manager;
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;
        [SerializeField] SuperHexagonGameOverManager _gameOverManager; public SuperHexagonGameOverManager gameOverManager => _gameOverManager;

        public SuperHexagonMapFile map => manager.map;
        public SuperHexagonRuleset ruleset => manager.ruleset;



        public int combo { get; private set; }
        public int maxCombo { get; private set; }

        public double score { get; private set; }

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

        public const double maxHealth = 1000;



        public event JudgementAction judgementAction;
        public delegate void JudgementAction(bool isMiss);

        public void Refresh() => SingletonCheck(this);

        int currentNoteIndex = 0;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            if (instance != null)
            {
                //게임 오버 상태 또는 일시정지 상태에선 판정하면 안됩니다
                if (!gameOverManager.isGameOver && !manager.isPaused && Kernel.gameSpeed != 0)
                {
                    double beat = map.allJudgmentBeat[currentNoteIndex];
                    for (int i = currentNoteIndex; beat < RhythmManager.currentBeatSound && i < map.allJudgmentBeat.Count; i++)
                    {
                        Perfect(beat);
                        HitsoundPlay();
                        currentNoteIndex++;
                    }
                }

                if (RhythmManager.currentBeatSound >= 0)
                    health -= map.globalEffect.hpRemoveValue.GetValue() * RhythmManager.bpmDeltaTime;
            }
        }

        public void Miss()
        {
            combo = 0;

            accuracys.Add(1);
            accuracy = accuracys.Average();

            if (!manager.isReplay)
                CreatedReplayFileAdd(RhythmManager.currentBeatSound);
            else
                GetReplayFileValue(RhythmManager.currentBeatSound);

            judgementAction?.Invoke(true);
        }

        public void Perfect(double beat)
        {
            combo += 1;

            double comboMultiplier = 0.25;
            {
                IMode comboMultiplierMode;
                if ((comboMultiplierMode = manager.modes.FindMode<ComboMultiplierModeBase>()) != null)
                    comboMultiplier = (float)((ComboMultiplierModeBase.Data)comboMultiplierMode.modeConfig).multiplier;
            }

            score += ruleset.GetScoreAddValue(0, map.allJudgmentBeat.Count, combo, comboMultiplier);

            if (maxCombo < combo)
            {
                maxCombo = combo;

                if (!manager.isReplay)
                    manager.createdReplay.maxCombo.Add(beat, maxCombo);
            }

            health += map.globalEffect.hpAddValue.GetValue(beat);

            accuracys.Add(0);
            accuracy = accuracys.Average();

            if (!manager.isReplay)
                CreatedReplayFileAdd(beat);
            else
                GetReplayFileValue(beat);

            judgementAction?.Invoke(true);
        }

        void CreatedReplayFileAdd(double beat)
        {
            manager.createdReplay.combos.Add(beat, combo);
            manager.createdReplay.scores.Add(beat, score);
            manager.createdReplay.healths.Add(beat, health);
            manager.createdReplay.accuracyAbses.Add(beat, accuracy);
            manager.createdReplay.accuracys.Add(beat, accuracy);
        }

        void GetReplayFileValue(double beat)
        {
            SuperHexagonReplayFile replay = manager.currentReplay;

            if (replay.combos.Count > 0)
                combo = replay.combos.GetValue(beat);
            if (replay.maxCombo.Count > 0)
                maxCombo = replay.maxCombo.GetValue(beat);
            if (replay.scores.Count > 0)
                score = replay.scores.GetValue(beat);
            if (replay.healths.Count > 0)
                health = replay.healths.GetValue(beat);
            if (replay.accuracyAbses.Count > 0)
                accuracy = replay.accuracyAbses.GetValue(beat);
        }

        public static void HitsoundPlay() => SoundManager.PlaySound("hitsound.normal", "sdjk", 0.5f, false, 0.95f);
    }
}
