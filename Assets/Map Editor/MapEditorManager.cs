using SDJK.Ruleset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.MapEditor
{
    public sealed class MapEditorManager : MonoBehaviour
    {
        public static void MapEditor(string mapFilePath)
        {
            RulesetManager.GameStart(mapFilePath, true);
        }
    }
}
