using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK
{
    public sealed class SDJKMode : IMode
    {
        public string modeName => "SDJK";
        public string[] compatibleMode => null;

        public void GameStart(string mapFilePath)
        {
            
        }
    }
}
