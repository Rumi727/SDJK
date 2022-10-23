using SCKRM;
using SCKRM.Input;
using SCKRM.Object;
using SCKRM.UI;
using SDJK.Map;
using SDJK.Ruleset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.MapSelectScreen
{
    public class MapSelectScreen : SCKRM.UI.UI
    {
        [SerializeField, NotNull] CanvasScaler canvasScaler;

        float upTimer = 0;
        float upTimer2 = 0;
        float downTimer = 0;
        float downTimer2 = 0;
        float leftTimer = 0;
        float leftTimer2 = 0;
        float rightTimer = 0;
        float rightTimer2 = 0;
        void Update()
        {
            canvasScaler.referenceResolution = new Vector2((ScreenManager.width / UIManager.currentGuiSize).Clamp(1920), (ScreenManager.height / UIManager.currentGuiSize).Clamp(1080));

            if (MainMenu.currentScreenMode == ScreenMode.mapPackSelect || MainMenu.currentScreenMode == ScreenMode.mapSelect)
            {
                bool up = ReapeatInput(KeyCode.UpArrow, ref upTimer, ref upTimer2);
                bool down = ReapeatInput(KeyCode.DownArrow, ref downTimer, ref downTimer2);
                bool left = ReapeatInput(KeyCode.LeftArrow, ref leftTimer, ref leftTimer2);
                bool right = ReapeatInput(KeyCode.RightArrow, ref rightTimer, ref rightTimer2);

                bool ReapeatInput(KeyCode keyCode, ref float timer, ref float timer2)
                {
                    if (InputManager.GetKey(keyCode))
                        return true;
                    else if (InputManager.GetKey(keyCode, InputType.Alway))
                    {
                        if (timer >= 0.25f)
                        {
                            if (timer2 >= 0.05f)
                            {
                                timer2 = 0;
                                return true;
                            }
                            else
                                timer2 += Kernel.unscaledDeltaTime;

                            return false;
                        }
                        else
                            timer += Kernel.unscaledDeltaTime;

                        return false;
                    }
                    else
                    {
                        timer2 = 0;
                        timer = 0;

                        return false;
                    }
                }

                if (up || left)
                {
                    if (MainMenu.currentScreenMode == ScreenMode.mapPackSelect)
                        MapManager.RulesetBackMapPack();
                    else if (MainMenu.currentScreenMode == ScreenMode.mapSelect)
                        MapManager.RulesetBackMap();
                }
                if (down || right)
                {
                    if (MainMenu.currentScreenMode == ScreenMode.mapPackSelect)
                        MapManager.RulesetNextMapPack();
                    else if (MainMenu.currentScreenMode == ScreenMode.mapSelect)
                        MapManager.RulesetNextMap();
                }
            }
        }
    }
}
