using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SCKRM;
using SCKRM.Easing;
using SCKRM.Input;
using SCKRM.Resource;
using SCKRM.Rhythm;
using SCKRM.SaveLoad;
using SCKRM.Scene;
using SCKRM.Sound;
using SCKRM.UI;
using SCKRM.UI.Layout;
using SCKRM.UI.SideBar;
using SCKRM.UI.StatusBar;
using SDJK.Mode;
using SDJK.Replay;
using SDJK.Ruleset;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SDJK.MainMenu
{
    public sealed class MainMenu : ManagerBase<MainMenu>
    {
        [GeneralSaveLoad]
        public class SaveData
        {
            [JsonProperty] public static bool logoVisualizerShow { get; set; } = true;
            [JsonProperty] public static bool logoMapHitsoundEnable { get; set; } = false;
            [JsonProperty] public static bool logoEffectEnable { get; set; } = true;
        }

        public static ScreenMode currentScreenMode { get; private set; } = ScreenMode.esc;



        [SerializeField, NotNull] RectTransform logo;
        [SerializeField, NotNull] RectTransform bar;
        [SerializeField, NotNull] CanvasGroup barCanvasGroup;
        [SerializeField, NotNull] RectTransform barLayout;
        [SerializeField, NotNull] HorizontalLayout barLayoutHorizontalLayout;
        [SerializeField, NotNull] CanvasGroup mapSelectScreen;
        [SerializeField, NotNull] SuperBlur.SuperBlur superBulr;
        [SerializeField, NotNull] GameObject logoVisualizer;
        [SerializeField, NotNull] LogoEffect logoEffect;



        void Awake()
        {
            if (SingletonCheck(this))
                StatusBarManager.allowStatusBarShow = false;
        }

        void OnEnable() => UIManager.homeEvent += EscScreen;
        void OnDisable() => UIManager.homeEvent -= EscScreen;

        static float screenNormalAniT = 0;
        static Vector2 screenNormalStartPos = Vector2.zero;
        static Vector2 screenNormalStartSize = Vector2.zero;
        static Vector2 screenEscStartPos = Vector2.zero;

        public static float barAlpha = 0;
        void Update()
        {
            SongSelect();

            if (InputManager.GetKey(KeyCode.Space) || InputManager.GetKey(KeyCode.Return))
                NextScreen();

            if (currentScreenMode == ScreenMode.esc)
            {
                DefaultLogoAni(Vector2.zero, new Vector2(logo.sizeDelta.x, 0));

                if (mapSelectScreen.alpha > 0)
                    mapSelectScreen.alpha = mapSelectScreen.alpha.MoveTowards(0, 0.1f * Kernel.fpsUnscaledSmoothDeltaTime);
                else if (mapSelectScreen.gameObject.activeSelf)
                    mapSelectScreen.gameObject.SetActive(false);
            }
            else if (currentScreenMode == ScreenMode.normal)
            {
                #region Logo Ani
                if (screenNormalAniT < 1)
                    screenNormalAniT = (screenNormalAniT + 0.1f * Kernel.fpsUnscaledSmoothDeltaTime).Clamp01();
                else
                {
                    if (barAlpha < 1)
                        barAlpha = (barAlpha += 0.1f * Kernel.fpsUnscaledSmoothDeltaTime).Clamp01();

                    barCanvasGroup.alpha = barAlpha;
                    barCanvasGroup.blocksRaycasts = true;
                    barCanvasGroup.interactable = true;

                    bar.sizeDelta = new Vector2(0, (float)EasingFunction.EaseOutCubic(0, 135, barAlpha));

                    Vector2 offsetMin = new Vector2(-240, 0);
                    if (Vector2.Distance(barLayout.offsetMin, offsetMin) > 0.01f)
                        barLayout.offsetMin = barLayout.offsetMin.Lerp(offsetMin, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                    else
                        barLayout.offsetMin = offsetMin;

                    barLayoutHorizontalLayout.spacing = barLayoutHorizontalLayout.spacing.Lerp(-25, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);

                    if (!bar.gameObject.activeSelf)
                        bar.gameObject.SetActive(true);
                }

                if (logo.anchoredPosition != new Vector2(-300, 0))
                {
                    float x = (float)EasingFunction.EaseInQuad(screenNormalStartPos.x, -300, screenNormalAniT);
                    float y = (float)EasingFunction.EaseInQuad(screenNormalStartPos.y, 0, screenNormalAniT);
                    logo.anchoredPosition = new Vector2(x, y);
                }

                if (logo.sizeDelta != new Vector2(320, 320))
                {
                    float x = (float)EasingFunction.EaseInQuad(screenNormalStartSize.x, 320, screenNormalAniT);
                    float y = (float)EasingFunction.EaseInQuad(screenNormalStartSize.y, 320, screenNormalAniT);
                    logo.sizeDelta = new Vector2(x, y);
                }
                #endregion

                if (mapSelectScreen.alpha > 0)
                    mapSelectScreen.alpha = mapSelectScreen.alpha.MoveTowards(0, 0.1f * Kernel.fpsUnscaledSmoothDeltaTime);
                else if (mapSelectScreen.gameObject.activeSelf)
                    mapSelectScreen.gameObject.SetActive(false);
            }
            else if (currentScreenMode == ScreenMode.mapPackSelect)
            {
                if (DefaultLogoAni(new Vector2(-92, 50), new Vector2(250, 250)))
                {
                    if (mapSelectScreen.alpha < 1)
                    {
                        mapSelectScreen.alpha = mapSelectScreen.alpha.MoveTowards(1, 0.1f * Kernel.fpsUnscaledSmoothDeltaTime);

                        if (!mapSelectScreen.gameObject.activeSelf)
                            mapSelectScreen.gameObject.SetActive(true);
                    }
                }
            }
            else if (currentScreenMode == ScreenMode.mapSelect)
            {
                if (DefaultLogoAni(new Vector2(-92, 50), new Vector2(250, 250)))
                {
                    if (mapSelectScreen.alpha < 1)
                    {
                        mapSelectScreen.alpha = mapSelectScreen.alpha.MoveTowards(1, 0.1f * Kernel.fpsUnscaledSmoothDeltaTime);

                        if (!mapSelectScreen.gameObject.activeSelf)
                            mapSelectScreen.gameObject.SetActive(true);
                    }
                }
            }

            superBulr.interpolation = mapSelectScreen.alpha;

            if (logoVisualizer.activeSelf != SaveData.logoVisualizerShow)
                logoVisualizer.SetActive(SaveData.logoVisualizerShow);

            bool DefaultLogoAni(Vector2 anchoredPosition, Vector2 sizeDelta)
            {
                if (barAlpha <= 0)
                {
                    if (currentScreenMode == ScreenMode.esc)
                    {
                        if (Vector2.Distance(screenEscStartPos, anchoredPosition) > 0.01f)
                            screenEscStartPos = screenEscStartPos.Lerp(anchoredPosition, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                        else
                            screenEscStartPos = anchoredPosition;

                        logo.anchoredPosition = logoEffect.pos + screenEscStartPos;
                    }
                    else
                    {
                        if (Vector2.Distance(logo.anchoredPosition, anchoredPosition) > 0.01f)
                            logo.anchoredPosition = logo.anchoredPosition.Lerp(anchoredPosition, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                        else
                            logo.anchoredPosition = anchoredPosition;
                    }

                    if (Vector2.Distance(logo.sizeDelta, sizeDelta) > 0.01f)
                        logo.sizeDelta = logo.sizeDelta.Lerp(sizeDelta, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                    else
                        logo.sizeDelta = sizeDelta;

                    barLayoutHorizontalLayout.spacing = -230;
                    barLayout.offsetMin = new Vector2(-440, 0);
                    screenNormalAniT = 0;

                    if (bar.gameObject.activeSelf)
                        bar.gameObject.SetActive(false);

                    return true;
                }
                else
                {
                    if (barAlpha > 0)
                        barAlpha = (barAlpha -= 0.1f * Kernel.fpsUnscaledSmoothDeltaTime).Clamp01();

                    barCanvasGroup.alpha = barAlpha;
                    barCanvasGroup.blocksRaycasts = false;
                    barCanvasGroup.interactable = false;

                    bar.sizeDelta = new Vector2(0, (float)EasingFunction.EaseOutCubic(0, 135, barAlpha));

                    barLayout.offsetMin = barLayout.offsetMin.Lerp(new Vector2(-440, 0), 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);
                    barLayoutHorizontalLayout.spacing = barLayoutHorizontalLayout.spacing.Lerp(-230, 0.2f * Kernel.fpsUnscaledSmoothDeltaTime);

                    return false;
                }
            }
        }

        void SongSelect()
        {
            if (currentScreenMode == ScreenMode.esc || currentScreenMode == ScreenMode.normal)
            {
                if (InputManager.GetKey("map_manager.pause_music"))
                {
                    if (BGMManager.bgm != null && BGMManager.bgm.soundPlayer != null && !BGMManager.bgm.soundPlayer.isRemoved)
                    {
                        if (!BGMManager.bgm.soundPlayer.isPaused)
                        {
                            BGMManager.bgm.soundPlayer.isPaused = true;
                            SettingInfoManager.Show("sdjk:map_manager.music", "sdjk:map_manager.pause_music", "map_manager.pause_music");
                        }
                        else
                        {
                            BGMManager.bgm.soundPlayer.isPaused = false;
                            SettingInfoManager.Show("sdjk:map_manager.music", "sdjk:map_manager.play_music", "map_manager.pause_music");
                        }
                    }
                }
                else if (InputManager.GetKey("map_manager.previous_music"))
                {
                    if (BGMManager.bgm != null && BGMManager.bgm.soundPlayer != null && !BGMManager.bgm.soundPlayer.isRemoved && BGMManager.bgm.soundPlayer.time > 10)
                    {
                        BGMManager.bgm.soundPlayer.time = 0;
                        SettingInfoManager.Show("sdjk:map_manager.music", "sdjk:map_manager.restart_music", "map_manager.previous_music");
                    }
                    else
                    {
                        if (MapManager.selectedMapPackIndex - 1 < 0)
                            MapManager.selectedMapPackIndex = MapManager.currentMapPacks.Count - 1;
                        else
                            MapManager.selectedMapPackIndex--;

                        SettingInfoManager.Show("sdjk:map_manager.music", "sdjk:map_manager.previous_music", "map_manager.previous_music");
                    }
                }
                if (InputManager.GetKey("map_manager.next_music"))
                {
                    if (MapManager.selectedMapPackIndex + 1 >= MapManager.currentMapPacks.Count)
                        MapManager.selectedMapPackIndex = 0;
                    else
                        MapManager.selectedMapPackIndex++;

                    SettingInfoManager.Show("sdjk:map_manager.music", "sdjk:map_manager.next_music", "map_manager.next_music");
                }
            }

            if (currentScreenMode == ScreenMode.mapPackSelect || currentScreenMode == ScreenMode.mapSelect)
            {
                if (MapManager.currentRulesetMapCount <= 0)
                    NormalScreen();
                else if (!RulesetManager.selectedRuleset.IsCompatibleRuleset(MapManager.selectedMapInfo.ruleset))
                {
                    MapManager.RulesetNextMap();

                    if (!RulesetManager.selectedRuleset.IsCompatibleRuleset(MapManager.selectedMapInfo.ruleset))
                    {
                        RandomMapPack();
                        MapPackSelectScreen();
                    }

                    void RandomMapPack()
                    {
                        MapManager.selectedMapPackIndex = UnityEngine.Random.Range(0, MapManager.currentMapPacks.Count);
                        MapManager.RulesetNextMapPack();
                    }
                }
            }
        }

        public static void NextScreen()
        {
            if (currentScreenMode == ScreenMode.esc)
                NormalScreen();
            else if (currentScreenMode == ScreenMode.normal)
                MapPackSelectScreen();
            else if (currentScreenMode == ScreenMode.mapPackSelect)
                MapSelectScreen();
            else if (currentScreenMode == ScreenMode.mapSelect)
                RulesetManager.GameStart(MapManager.selectedMap.mapFilePath, null, false, ModeManager.selectedModeList.ToArray());
        }

        public static void EscScreen()
        {
            currentScreenMode = ScreenMode.esc;
            StatusBarManager.allowStatusBarShow = false;

            ScreenChange(new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.8f));
            screenEscStartPos = instance.logo.anchoredPosition - instance.logoEffect.pos;
        }

        public static void NormalScreen()
        {
            currentScreenMode = ScreenMode.normal;
            StatusBarManager.allowStatusBarShow = true;

            ScreenChange(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            UIManager.BackEventAdd(EscScreen);

            screenNormalStartPos = instance.logo.anchoredPosition;
            screenNormalStartSize = instance.logo.rect.size;
        }

        public static void MapPackSelectScreen()
        {
            currentScreenMode = ScreenMode.mapPackSelect;
            StatusBarManager.allowStatusBarShow = true;

            ScreenChange(Vector2.right, Vector2.right);
            UIManager.BackEventAdd(NormalScreen);
        }

        public static void MapSelectScreen()
        {
            currentScreenMode = ScreenMode.mapSelect;
            StatusBarManager.allowStatusBarShow = true;

            ScreenChange(Vector2.right, Vector2.right);
            UIManager.BackEventAdd(MapPackSelectScreen);
        }

        static void ScreenChange(Vector2 anchorMin, Vector2 anchorMax)
        {
            Vector2 pos = instance.logo.localPosition;
            Vector2 size = instance.logo.rect.size;

            instance.logo.anchorMin = anchorMin;
            instance.logo.anchorMax = anchorMax;

            instance.logo.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            instance.logo.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

            instance.logo.localPosition = pos;

            UIManager.BackEventRemove(EscScreen);
            UIManager.BackEventRemove(NormalScreen);
            UIManager.BackEventRemove(MapPackSelectScreen);
            UIManager.BackEventRemove(MapSelectScreen);
        }

        public static void ApplicationQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static async UniTaskVoid MainMenuLoad()
        {
            StatusBarManager.statusBarForceHide = false;
            SideBarManager.sideBarForceHide = false;
            ResourceManager.audioResetProhibition = false;

            EventSystem.current.SetSelectedGameObject(null);
            SideBarManager.AllHide();

            UIManager.BackEventAllRemove();

            RhythmManager.Stop();

            SoundManager.StopSoundAll(true);
            SoundManager.StopNBSAll(true);

            SceneManager.LoadScene(2).Forget();

            Kernel.gameSpeed = 1;

            await UniTask.WaitUntil(() => instance != null);

            MapSelectScreen();
        }
    }

    public enum ScreenMode
    {
        esc,
        normal,
        mapPackSelect,
        mapSelect
    }
}
