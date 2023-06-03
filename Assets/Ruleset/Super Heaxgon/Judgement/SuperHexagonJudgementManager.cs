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
using SDJK.Mode.Fun;

namespace SDJK.Ruleset.SuperHexagon.Judgement
{
    public sealed class SuperHexagonJudgementManager : JudgementManagerBase
    {
        public static new SuperHexagonJudgementManager instance => (SuperHexagonJudgementManager)JudgementManagerBase.instance;

        [SerializeField] SuperHexagonManager _manager; public SuperHexagonManager manager => _manager;
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;
        [SerializeField] SuperHexagonGameOverManager _gameOverManager; public SuperHexagonGameOverManager gameOverManager => _gameOverManager;

        public SuperHexagonMapFile map => manager.map;
        public SuperHexagonRuleset ruleset => manager.ruleset;



        public event JudgementAction judgementAction;
        public delegate void JudgementAction(bool isMiss);



        int currentNoteIndex = 0;
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            if (instance != null)
            {
                //게임 오버 상태 또는 일시정지 상태에선 판정하면 안됩니다
                if (!gameOverManager.isGameOver && !manager.isPaused && Kernel.gameSpeed != 0 && currentNoteIndex < map.allJudgmentBeat.Count)
                {
                    bool hitsoundPlay = false;
                    double beat = map.allJudgmentBeat[currentNoteIndex];
                    for (int i = currentNoteIndex; beat < RhythmManager.currentBeatSound && i < map.allJudgmentBeat.Count; i++)
                    {
                        Perfect(beat);
                        hitsoundPlay = true;

                        currentNoteIndex++;
                        if (currentNoteIndex < map.allJudgmentBeat.Count)
                            beat = map.allJudgmentBeat[currentNoteIndex];
                    }

                    if (hitsoundPlay)
                        HitsoundPlay();

                    if (RhythmManager.currentBeatSound >= 0)
                        health += map.globalEffect.hpRemoveValue.GetValue(RhythmManager.currentBeatSound) * RhythmManager.bpmDeltaTime;
                }

                SetRankProgress();
            }
        }

        void SetRankProgress() => rankProgress = (1 - (RhythmManager.currentBeatSound / map.info.clearBeat)).Clamp01();

        public void Miss(double beat)
        {
            if (!gameObject.activeSelf)
                return;

            combo = 0;

            accuracys.Add(1);
            accuracy = accuracys.Average();

            health -= map.globalEffect.hpMissValue.GetValue(beat);

            SetRankProgress();

            if (!manager.isReplay)
                CreatedReplayFileAdd(RhythmManager.currentBeatSound);
            else
                GetReplayFileValue(RhythmManager.currentBeatSound);

            SoundManager.PlaySound("ruleset.super_hexagon.damage", "sdjk");
            judgementAction?.Invoke(true);

            //서든 데스 및 완벽주의자 모드
            {
                IMode mode;
                if ((mode = manager.modes.FindMode<SuddenDeathModeBase>()) != null)
                {
                    SuddenDeathModeBase.Config config = (SuddenDeathModeBase.Config)mode.modeConfig;
                    if (config.restartOnFail)
                        manager.Restart();
                    else
                        instance.health = 0;
                }
            }

            if (health <= 0)
                gameOverManager.GameOver();

            {
                IMode mode;
                if (((mode = manager.modes.FindMode<AccelerationModeBase>()) != null && ((AccelerationModeBase.Config)mode.modeConfig).resetIfMiss) ||
                    ((mode = manager.modes.FindMode<DecelerationModeBase>()) != null && ((DecelerationModeBase.Config)mode.modeConfig).resetIfMiss))
                    manager.accelerationDeceleration = 1;
            }
        }

        public void Perfect(double beat)
        {
            if (!gameObject.activeSelf)
                return;

            combo++;

            accuracys.Add(0);
            accuracy = accuracys.Average();

            double comboMultiplier = 1;
            {
                IMode comboMultiplierMode;
                if ((comboMultiplierMode = manager.modes.FindMode<ComboMultiplierModeBase>()) != null)
                    comboMultiplier = (float)((ComboMultiplierModeBase.Config)comboMultiplierMode.modeConfig).multiplier;
            }

            score += JudgementUtility.GetScoreAddValue(0, map.allJudgmentBeat.Count, combo, comboMultiplier) * (1 - accuracy);

            if (maxCombo < combo)
            {
                maxCombo = combo;

                if (!manager.isReplay)
                    manager.createdReplay.maxCombo.Add(beat, maxCombo);
            }

            SetRankProgress();

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

        public static void HitsoundPlay() => HitsoundEffect.DefaultHitsoundPlay();
    }
}
