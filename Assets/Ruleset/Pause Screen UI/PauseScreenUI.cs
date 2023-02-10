using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
using SCKRM.UI;
using System;
using UnityEngine;

namespace SDJK.Ruleset.PauseScreen
{
    public sealed class PauseScreenUI : UIBase
    {
        [SerializeField] string inputLockKey = "ruleset.pause_screen";
        [SerializeField] float alphaAni = 0.15f;
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] GameObject viewport;
        [SerializeField] bool cantHide = false;

        public bool isShow { get; private set; }

        bool pause;
        public void Show(bool pause = true)
        {
            if (isShow)
                return;

            isShow = true;
            this.pause = pause;

            canvasGroup.blocksRaycasts = true;
            InputManager.SetInputLock(inputLockKey, true);

            if (!cantHide)
                UIManager.BackEventAdd(Hide);

            if (pause)
            {
                RhythmManager.isPaused = true;
                Kernel.gameSpeed = 0;
            }
        }

        public void Hide()
        {
            if (cantHide || !isShow)
                return;

            isShow = false;
            UIManager.BackEventRemove(Hide);

            Disable();
        }

        public void Disable()
        {
            canvasGroup.blocksRaycasts = false;
            InputManager.SetInputLock(inputLockKey, false);

            if (pause)
            {
                RhythmManager.isPaused = false;
                Kernel.gameSpeed = 1;
            }
        }

        protected override void OnDestroy()
        {
            InputManager.SetInputLock(inputLockKey, false);

            if (!cantHide)
                UIManager.BackEventRemove(Hide);

            if (pause)
            {
                RhythmManager.isPaused = false;
                Kernel.gameSpeed = 1;
            }
        }

        void Update()
        {
            if (isShow)
                canvasGroup.alpha = canvasGroup.alpha.MoveTowards(1, alphaAni * Kernel.fpsUnscaledSmoothDeltaTime);
            else
                canvasGroup.alpha = canvasGroup.alpha.MoveTowards(0, alphaAni * Kernel.fpsUnscaledSmoothDeltaTime);

            if (canvasGroup.alpha <= 0)
            {
                if (viewport.activeSelf)
                    viewport.SetActive(false);
            }
            else if (!viewport.activeSelf)
                viewport.SetActive(true);
        }
    }
}
