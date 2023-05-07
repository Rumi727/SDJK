using SCKRM;
using SCKRM.Input;
using SCKRM.Object;
using SCKRM.UI;
using SDJK.Map;
using SDJK.Replay;
using SDJK.Ruleset.UI.ReplayResult;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Ruleset
{
    public sealed class ResultScreen : UIObjectPoolingBase
    {
        public static ResultScreen Show(IRuleset ruleset, MapFile map, ReplayFile replay, Action backEvent, string prefab = "ruleset.result_screen")
        {
            ResultScreen resultScreen = (ResultScreen)ObjectPoolingSystem.ObjectCreate(prefab).monoBehaviour;
            resultScreen.Refresh(ruleset, map, replay, backEvent);
            
            return resultScreen;
        }

        [SerializeField, NotNull] Graphic background;
        [SerializeField, NotNull] ReplayResultUI replayResultUI;
        [SerializeField] float alphaAni = 0.15f;
        [SerializeField, NotNull] CanvasGroup canvasGroup;

        IRuleset ruleset;
        MapFile map = null;
        ReplayFile replay = null;
        Action backEvent;

        bool isShow = false;

        public void Refresh(IRuleset ruleset, MapFile map, ReplayFile replay, Action backEvent)
        {
            this.ruleset = ruleset;
            this.map = map;
            this.replay = replay;
            this.backEvent = backEvent;

            replayResultUI.Refresh(ruleset, map, replay, 0);

            InputManager.SetInputLock("ruleset.result_screen_" + GetInstanceID(), true);
            UIManager.BackEventAdd(Hide);
            isShow = true;
        }

        public void Hide()
        {
            if (!isShow)
                return;

            InputManager.SetInputLock("ruleset.result_screen_" + GetInstanceID(), false);
            UIManager.BackEventRemove(Hide);

            backEvent?.Invoke();

            background.raycastTarget = false;
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
            if (ruleset == null || map == null || replay == null || !isShow)
                return;

            ruleset.GameStart(map.mapFilePath, replay.replayFilePath, false);

            InputManager.SetInputLock("ruleset.result_screen_" + GetInstanceID(), false);
            UIManager.BackEventRemove(Hide);

            background.raycastTarget = false;
            isShow = false;
        }

        public void ReplayRemove()
        {
            if (ruleset == null || map == null || replay == null || !isShow)
                return;

            ReplayLoader.ReplayDelete(replay);
            Hide();
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

            background.raycastTarget = true;
            replayResultUI.ObjectReset();

            return true;
        }
    }
}
