using Cysharp.Threading.Tasks;
using SCKRM.Renderer;
using SCKRM.Scene;
using SDJK.Map;
using SDJK.Ruleset.SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class SDJKRuleset : Ruleset
    {
        public override NameSpaceIndexTypePathPair icon => new NameSpaceIndexTypePathPair("sdjk", "ruleset/sdjk", "icon");

        public override async void GameStart(string mapFilePath)
        {
            await SceneManager.LoadScene(3);
            await UniTask.NextFrame();

            SDJKMapFile map = MapLoader.MapLoad<SDJKMapFile>(mapFilePath);
            Object.FindObjectOfType<SDJKManager>().Refresh(map);
        }
    }
}
