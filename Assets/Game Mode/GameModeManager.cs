using SCKRM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK
{
    public sealed class GameModeManager : Manager<GameModeManager>
    {
        public static List<IGameMode> gameModeList { get; } = new List<IGameMode>();
        public static IGameMode selectedGameMode { get; private set; } = null;

        void Awake()
        {
            if (SingletonCheck(this))
            {
                gameModeList.Add(new SDJKGameMode());

                selectedGameMode = gameModeList[0];
            }
        }
    }

    public interface IGameMode
    {
        public string gameModeName { get; }
        public string[] compatibleMode { get; }

        public void GameStart(string mapFilePath);
    }
}
