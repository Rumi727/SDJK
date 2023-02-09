using Cysharp.Threading.Tasks;
using SCKRM;
using SCKRM.Object;
using SDJK.Map;
using SDJK.Replay;
using SDJK.Ruleset;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SDJK.MainMenu
{
    public sealed class ReplayList : SCKRM.UI.UI
    {
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
        }

        protected override void OnEnable() => ReplayManager.replayLoadingEnd += Refresh;
        protected override void OnDisable() => ReplayManager.replayLoadingEnd -= Refresh;

        protected override void OnDestroy() => cancel.Cancel();

        public void Refresh() => refresh().Forget();

        CancellationTokenSource cancel;
        async UniTaskVoid refresh()
        {
            cancel?.Cancel();
            cancel = new CancellationTokenSource();

            MapFile map = MapManager.selectedMap;
            for (int i = 0; i < replayResultUIs.Count; i++)
            {
                ReplayResultUI ui = replayResultUIs[i];
                if (ui != null)
                    ui.Remove();
            }

            replayResultUIs.Clear();

            int lastReplayRanking = 1;
            ReplayFile lastReplay = null;
            if (ReplayManager.currentReplayFiles.TryGetValue(map.info.id, out List<ReplayFile> replays))
            {
                for (int i = 0; i < replays.Count; i++)
                {
                    ReplayFile replay = replays[i];

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

                    ReplayResultUI ui = (ReplayResultUI)ObjectPoolingSystem.ObjectCreate(replayResultUIPrefab, content).monoBehaviour;
                    replayResultUIs.Add(ui);

                    ui.Refresh(RulesetManager.selectedRuleset, map, replay, i + 1);
                    await UniTask.NextFrame(cancel.Token);
                }
            }

            lastReplayResultUI.ObjectReset();

            if (lastReplay != null)
                lastReplayResultUI.Refresh(RulesetManager.selectedRuleset, MapManager.selectedMap, lastReplay, lastReplayRanking);
        }
    }
}
