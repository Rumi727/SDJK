using SCKRM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK
{
    public sealed class ModeManager : Manager<ModeManager>
    {
        public static List<IMode> modeList { get; } = new List<IMode>();
        public static IMode selectedMode { get; private set; } = null;

        void Awake()
        {
            if (SingletonCheck(this))
            {
                modeList.Add(new SDJKMode());

                selectedMode = modeList[0];
            }
        }
    }

    public interface IMode
    {
        public string modeName { get; }
        public string[] compatibleMode { get; }

        public void GameStart(string mapFilePath);
    }
}
