using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Object;
using SCKRM.UI;
using SDJK.Map;
using SDJK.Replay;
using SDJK.Ruleset;
using SDJK.Ruleset.UI.ReplayResult;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SDJK.MainMenu
{
    public sealed class ReplayList : UIBase
    {
        [SerializeField] RectTransformTool viewport;
        [SerializeField] Transform content;
        [SerializeField] ReplayResultUI lastReplayResultUI;
        [SerializeField] string replayResultUIPrefab = "ruleset.replay_result_ui";

        MapFile lastMap;
        List<ReplayResultUI> replayResultUIs = new List<ReplayResultUI>();
        void Update()
        {
            MapFile map = MapManager.selectedMap;
            if (lastMap != map)
            {
                Refresh();
                lastMap = MapManager.selectedMap;
            }

            for (int i = 0; i < replayResultUIs.Count; i++)
            {
                ReplayResultUI ui = replayResultUIs[i];

                bool top = viewport.rectTransformTool.worldCorners.topLeft.y < ui.rectTransformTool.worldCorners.bottomLeft.y;
                bool bottom = viewport.rectTransformTool.worldCorners.bottomLeft.y > ui.rectTransformTool.worldCorners.topLeft.y;

                bool active = !(top || bottom);
                if (active != ui.gameObject.activeSelf)
                    ui.gameObject.SetActive(active);
            }
        }

        protected override void OnEnable()
        {
            RulesetManager.rulesetChanged += Refresh;

            ReplayLoader.replaySaveEvent += Refresh;
            ReplayLoader.replayDeleteEvent += Refresh;
        }

        protected override void OnDisable()
        {
            RulesetManager.rulesetChanged -= Refresh;

            ReplayLoader.replaySaveEvent -= Refresh;
            ReplayLoader.replayDeleteEvent -= Refresh;
        }

        protected override void OnDestroy() => cancel.Cancel();

        public void Refresh() => refresh().Forget();
        void Refresh(ReplayFile replay) => refresh().Forget();

        CancellationTokenSource cancel;
        async UniTaskVoid refresh()
        {
            cancel?.Cancel();
            cancel = new CancellationTokenSource();

            IRuleset ruleset = RulesetManager.selectedRuleset;
            MapFile map = MapManager.selectedMap;
            for (int i = 0; i < replayResultUIs.Count; i++)
            {
                ReplayResultUI ui = replayResultUIs[i];
                if (ui != null)
                    ui.Remove();
            }

            replayResultUIs.Clear();

            lastReplayResultUI.ObjectReset();

            int lastReplayRanking = 1;
            ReplayFile lastReplay = null;

            List<ReplayFile> replays = await ReplayLoader.ReplaysLoad(map);
            if (replays == null || !Kernel.isPlaying || this == null || ruleset != RulesetManager.selectedRuleset || map != MapManager.selectedMap)
                return;

            for (int i = 0; i < replays.Count; i++)
            {
                ReplayFile replay = replays[i];
                if (replay.ruleset != ruleset.name)
                    continue;

                //마지막 리플레이 계산
                if (lastReplay == null)
                {
                    lastReplay = replay;
                    lastReplayRanking = i + 1;
                }
                else
                {
                    TimeSpan replayClearTime = DateTime.UtcNow - replay.clearUTCTime;
                    TimeSpan lastClearTime = DateTime.UtcNow - lastReplay.clearUTCTime;

                    if (replayClearTime <= lastClearTime)
                    {
                        lastReplay = replay;
                        lastReplayRanking = i + 1;
                    }
                }
            }

            if (lastReplay != null)
                lastReplayResultUI.Refresh(ruleset, map, lastReplay, lastReplayRanking);

            int loopCount = 0;
            for (int i = 0; i < replays.Count; i++)
            {
                ReplayFile replay = replays[i];
                if (replay.ruleset != ruleset.name)
                    continue;

                ReplayResultUI ui = (ReplayResultUI)ObjectPoolingSystem.ObjectCreate(replayResultUIPrefab, content).monoBehaviour;
                replayResultUIs.Add(ui);

                ui.Refresh(ruleset, map, replay, i + 1);

                if (loopCount >= 10)
                {
                    if (await UniTask.DelayFrame(1, PlayerLoopTiming.Update, cancel.Token).SuppressCancellationThrow() || ruleset != RulesetManager.selectedRuleset || map != MapManager.selectedMap)
                        return;

                    loopCount = 0;
                }

                loopCount++;
            }
        }
    }
}
