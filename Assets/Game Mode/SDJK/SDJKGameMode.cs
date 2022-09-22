using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK
{
    public sealed class SDJKGameMode : IGameMode
    {
        public string gameModeName => GetType().FullName;
        public string[] compatibleMode => null;

        public void GameStart(string mapFilePath)
        {

        }
    }
}
