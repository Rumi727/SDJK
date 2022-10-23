using Cysharp.Threading.Tasks;
using SCKRM.Renderer;
using SCKRM.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Ruleset.SDJK
{
    public sealed class SDJKRuleset : Ruleset
    {
        public override NameSpaceIndexTypePathPair icon => new NameSpaceIndexTypePathPair("sdjk", "ruleset/sdjk", "icon");

        public override void GameStart(string mapFilePath)
        {
            SceneManager.LoadScene(3).Forget();
        }
    }
}
