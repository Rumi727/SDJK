using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityToolbarExtender
{
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(0, 3, 0, 0),
            };
        }
    }
}