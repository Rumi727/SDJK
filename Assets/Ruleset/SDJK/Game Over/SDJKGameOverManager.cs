using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
using SDJK.Mode;
using SDJK.Mode.Difficulty;
using SDJK.Ruleset.UI.PauseScreen;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.GameOver
{
    public sealed class SDJKGameOverManager : ManagerBase<SDJKGameOverManager>
    {
        [SerializeField] SDJKManager _manager; public SDJKManager manager => _manager;
        [SerializeField] PauseScreenUI _pauseScreen; public PauseScreenUI gameOverScreen => _pauseScreen;

        [SerializeField] float speed = 0;
        [SerializeField] float screenShowTime = 3;

        [SerializeField] bool _invincibility; public bool invincibility { get => _invincibility; set => _invincibility = value; }

        public IMode[] modes => manager.modes;

        public bool isGameOver { get; private set; } = false;

        public void Refresh() => SingletonCheck(this);

        float timer = 0;
        void Update()
        {
            if (instance != null && RhythmManager.isPlaying && isGameOver)
            {
                if (Kernel.gameSpeed > 0.001f)
                    Kernel.gameSpeed = Kernel.gameSpeed.Lerp(0, speed * Kernel.fpsUnscaledSmoothDeltaTime);
                else
                {
                    Kernel.gameSpeed = 0;
                    RhythmManager.isPaused = true;
                }

                if (timer >= screenShowTime)
                    gameOverScreen.Show(false);
                else
                    timer += Kernel.unscaledDeltaTime;
            }
        }

        public void GameOver()
        {
            if (invincibility || modes.FindMode<NoFailModeBase>() != null)
                return;

            if (!manager.isReplay)
            {
                manager.createdReplay.gameOverBeat = RhythmManager.currentBeatSound;
                manager.createdReplay.isGameOver = true;
            }

            isGameOver = true;
            InputManager.SetInputLock("ruleset.sdjk.gameover", true);
        }

        void OnDestroy() => InputManager.SetInputLock("ruleset.sdjk.gameover", false);
    }
}
