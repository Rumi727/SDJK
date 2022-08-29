using PolyAndCode.UI;
using SCKRM;
using SCKRM.Input;
using SCKRM.Object;
using SCKRM.UI;
using SDJK.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.MapSelectScreen
{
    public class MapSelectScreen : UI
    {
        [SerializeField, NotNull] CanvasScaler canvasScaler;

        float upTimer = 0;
        float upTimer2 = 0;
        float downTimer = 0;
        float downTimer2 = 0;
        void Update()
        {
            canvasScaler.referenceResolution = new Vector2((ScreenManager.width / UIManager.currentGuiSize).Clamp(1920), (ScreenManager.height / UIManager.currentGuiSize).Clamp(1080));

            if (MainMenu.currentScreenMode == ScreenMode.mapPackSelect || MainMenu.currentScreenMode == ScreenMode.mapSelect)
            {
                bool up = ReapeatInput(KeyCode.UpArrow, ref upTimer, ref upTimer2);
                bool down = ReapeatInput(KeyCode.DownArrow, ref downTimer, ref downTimer2);

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

                if (up)
                {
                    if (MainMenu.currentScreenMode == ScreenMode.mapPackSelect)
                        Left();
                    else if (MainMenu.currentScreenMode == ScreenMode.mapSelect)
                    {
                        if (MapManager.selectedMapIndex - 1 < 0)
                        {
                            //Left();
                            MapManager.selectedMapIndex = MapManager.selectedMapPack.maps.Count - 1;
                        }
                        else
                            MapManager.selectedMapIndex--;
                    }
                }
                if (down)
                {
                    if (MainMenu.currentScreenMode == ScreenMode.mapPackSelect)
                        Right();
                    else if (MainMenu.currentScreenMode == ScreenMode.mapSelect)
                    {
                        if (MapManager.selectedMapIndex + 1 >= MapManager.selectedMapPack.maps.Count)
                        {
                            //Right();
                            MapManager.selectedMapIndex = 0;
                        }
                        else
                            MapManager.selectedMapIndex++;
                    }
                }

                void Left()
                {
                    if (MapManager.selectedMapPackIndex - 1 < 0)
                        MapManager.selectedMapPackIndex = MapManager.currentMapPacks.Count - 1;
                    else
                        MapManager.selectedMapPackIndex--;
                }

                void Right()
                {
                    if (MapManager.selectedMapPackIndex + 1 >= MapManager.currentMapPacks.Count)
                        MapManager.selectedMapPackIndex = 0;
                    else
                        MapManager.selectedMapPackIndex++;
                }
            }
        }
    }
}
