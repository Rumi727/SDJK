using SCKRM;
using SCKRM.Input;
using SCKRM.Object;
using SCKRM.UI;
using SDJK.Map;
using SDJK.Replay;
using System;
using UnityEngine;

namespace SDJK.Ruleset
{
    public sealed class ResultScreen : UIObjectPooling
    {
        public static ResultScreen Show(IRuleset ruleset, MapFile map, ReplayFile replay, Action backEvent, string prefab = "ruleset.result_screen")
        {
            if (isShow)
                return null;

            ResultScreen resultScreen = (ResultScreen)ObjectPoolingSystem.ObjectCreate(prefab).monoBehaviour;
            resultScreen.Refresh(ruleset, map, replay, backEvent);
            
            return resultScreen;
        }

        [SerializeField] ReplayResultUI replayResultUI;
        [SerializeField] float alphaAni = 0.1f;
        [SerializeField, NotNull] CanvasGroup canvasGroup;

        IRuleset ruleset;
        MapFile map = null;
        ReplayFile replay = null;
        Action backEvent;

        static bool isShow = false;

        public void Refresh(IRuleset ruleset, MapFile map, ReplayFile replay, Action backEvent)
        {
            this.ruleset = ruleset;
            this.map = map;
            this.replay = replay;
            this.backEvent = backEvent;

            replayResultUI.Refresh(ruleset, map, replay, 0);

            InputManager.SetInputLock("ruleset.result_screen", true);
            UIManager.BackEventAdd(Hide);
            isShow = true;
        }

        public void Hide()
        {
            InputManager.SetInputLock("ruleset.result_screen", false);
            UIManager.BackEventRemove(Hide);

            backEvent?.Invoke();
            isShow = false;
        }

        float alphaValue = 0;
        void Update()
        {
            if (ruleset == null || map == null || replay == null)
                return;

            if (isShow)
                alphaValue = alphaValue.MoveTowards(1, alphaAni * Kernel.fpsUnscaledSmoothDeltaTime);
            else
            {
                alphaValue = alphaValue.MoveTowards(0, alphaAni * Kernel.fpsUnscaledSmoothDeltaTime);
                if (alphaValue <= 0)
                    Remove();
            }

            canvasGroup.alpha = alphaValue;
        }

        public void ReplayPlay()
        {
            if (ruleset == null || map == null || replay == null)
                return;

            ruleset.GameStart(map.mapFilePath, replay.replayFilePath, false);
        }

        protected override void OnDestroy()
        {
            isShow = false;
            UIManager.BackEventRemove(Hide);
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            ruleset = null;
            map = null;
            replay = null;

            alphaValue = 0;
            isShow = false;

            replayResultUI.ObjectReset();
            return true;
        }
    }
}
