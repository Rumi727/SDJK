using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
using SDJK.Effect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK.Effect
{
    public sealed class SDJKGameOverManager : Manager<SDJKGameOverManager>
    {
        [SerializeField] SDJKManager _manager; public SDJKManager manager => _manager;
        [SerializeField] float speed = 0;

        public bool isGameOver { get; private set; } = false;

        void Awake() => SingletonCheck(this);

        void Update()
        {
            if (RhythmManager.isPlaying && isGameOver)
            {
                if (Kernel.gameSpeed > 0.001f)
                    Kernel.gameSpeed = Kernel.gameSpeed.Lerp(0, speed * Kernel.fpsUnscaledDeltaTime);
                else
                {
                    Kernel.gameSpeed = 0;
                    manager.soundPlayer.isPaused = true;
                }
            }
        }

        public void GameOver()
        {
            isGameOver = true;
            InputManager.SetInputLock("ruleset.sdjk.gameover", true);
        }

        void OnDestroy() => InputManager.SetInputLock("ruleset.sdjk.gameover", false);
    }
}