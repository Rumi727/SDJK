using Newtonsoft.Json;
using SCKRM.Input;
using SCKRM.SaveLoad;
using SCKRM.UI;
using SCKRM.UI.Overlay.MessageBox;
using SCKRM.UI.Setting;
using SCKRM.Window;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SCKRM.Cursor
{
    [WikiDescription("마우스 포인터를 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Cursor/UI/Cursor Manager")]
    public sealed class CursorManager : UIManager<CursorManager>
    {
        [GeneralSaveLoad]
        public sealed class SaveData
        {
            [JsonProperty] public static bool ignoreMouseAcceleration { get; set; } = false;
            [JsonProperty] public static float mouseSensitivity { get; set; } = 1;
        }

        /// <summary>
        /// 마우스 포인터를 화면에 표시하고 있는지에 대한 여부입니다
        /// </summary>
        [WikiDescription("마우스 포인터를 화면에 표시하고 있는지에 대한 여부입니다")]
        public static bool visible
        {
            get => _visible;
            set
            {
                if (value && InputManager.mousePresent)
                    instance.canvasGroup.alpha = 1;
                else
                    instance.canvasGroup.alpha = 0;

                _visible = value;
            }
        }
        static bool _visible = true;

        /// <summary>
        /// 현재 마우스 포인터가 창 안에 있으며 이 프로그램을 포커스하고 있는지에 대한 여부입니다
        /// </summary>
        [WikiDescription("현재 마우스 포인터가 창 안에 있으며 이 프로그램을 포커스하고 있는지에 대한 여부입니다")]
        public static bool isFocused { get; private set; } = false;
        /// <summary>
        /// 드래그 하고 있는지에 대한 여부입니다
        /// </summary>
        [WikiDescription("드래그 하고 있는지에 대한 여부입니다")]
        public static bool isDragged { get; private set; } = false;



        [WikiDescription("붙어있는 캔버스 그룹을 가져옵니다")]
        public CanvasGroup canvasGroup => _canvasGroup = this.GetComponentFieldSave(_canvasGroup); [SerializeField, NotNull] CanvasGroup _canvasGroup;



        protected override void OnEnable() => SingletonCheck(this);

#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
        static Vector2 ignoreMouseAccelerationPos = Vector2.zero;
        protected override void Awake() => ignoreMouseAccelerationPos = GetCursorPosition(0, 0);
#endif

        Vector2 dragStartMousePosition = Vector2.zero;
        bool dragStart = false;
        bool mousePresent = false;
        void LateUpdate()
        {
            if (InputManager.mousePresent != mousePresent)
            {
                visible = visible;
                mousePresent = InputManager.mousePresent;
            }

            isFocused = InputManager.mousePosition.x >= 0 && InputManager.mousePosition.x <= ScreenManager.width && InputManager.mousePosition.y >= 0 && InputManager.mousePosition.y <= ScreenManager.height;
            UnityEngine.Cursor.visible = !isFocused;

            if (InitialLoadManager.isInitialLoadEnd)
            {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
                if (SaveData.ignoreMouseAcceleration && Application.isFocused && InputManager.mousePosition.x >= 0 && InputManager.mousePosition.x <= ScreenManager.width && InputManager.mousePosition.y >= 0 && InputManager.mousePosition.y <= ScreenManager.height)
                {
                    setCursorPosition((ignoreMouseAccelerationPos.x).RoundToInt(), (ignoreMouseAccelerationPos.y).RoundToInt(), 0, 0, true);

                    Vector2 delta = InputManager.GetMouseDelta(false, "all", "force");
                    delta.y = -delta.y;
                    ignoreMouseAccelerationPos += delta;
                }
                else
                    ignoreMouseAccelerationPos = GetCursorPosition(0, 0);
#endif

                #region Pos Move
                Vector2 pos = InputManager.mousePosition / UIManager.currentGuiSize;

                if (graphic.enabled != visible)
                {
                    graphic.enabled = visible;
                    rectTransform.anchoredPosition = Vector3.zero;
                }

                isDragged = false;

                if (UnityEngine.Input.GetMouseButtonDown(0))
                {
                    dragStart = false;
                    dragStartMousePosition = pos;
                }
                else if (UnityEngine.Input.GetMouseButton(0))
                {
                    graphic.color = graphic.color.Lerp(UIManager.SaveData.systemColor * new Color(1, 1, 1, 0.5f), 0.2f * Kernel.fpsUnscaledDeltaTime);
                    rectTransform.localScale = rectTransform.localScale.Lerp(Vector3.one * 0.2f, 0.075f * Kernel.fpsUnscaledDeltaTime);

                    if (!dragStart && Vector2.Distance(pos, dragStartMousePosition) >= EventSystem.current.pixelDragThreshold / UIManager.currentGuiSize)
                        dragStart = true;
                    else if (dragStart)
                    {
                        Vector3 dir = pos - dragStartMousePosition;
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, Quaternion.Euler(new Vector3(0, 0, angle + 67.5f)), 0.2f * Kernel.fpsUnscaledDeltaTime);

                        isDragged = true;
                    }
                    else
                        rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, Quaternion.Euler(Vector3.zero), 0.2f * Kernel.fpsUnscaledDeltaTime);
                }
                else
                {
                    graphic.color = graphic.color.Lerp(new Color(0, 0, 0, 0.5f), 0.2f * Kernel.fpsUnscaledDeltaTime);

                    rectTransform.localScale = transform.localScale.Lerp(Vector3.one * 0.25f, 0.3f * Kernel.fpsUnscaledDeltaTime);
                    rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, Quaternion.Euler(Vector3.zero), 0.2f * Kernel.fpsUnscaledDeltaTime);
                }

                rectTransform.anchoredPosition3D = pos;
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.zero;
                rectTransform.pivot = Vector2.up;
                #endregion
            }
        }



        static bool highPrecisionMouseWarningLock = false;
        [WikiDescription("시스템 마우스 가속 무시 설정을 변경할때 경고창을 띄우는 메소드입니다")]
        public static async void HighPrecisionMouseWarning(Setting setting)
        {
            if (highPrecisionMouseWarningLock)
                return;

            if (InitialLoadManager.isInitialLoadEnd && SaveData.ignoreMouseAcceleration)
            {
                highPrecisionMouseWarningLock = true;

                try
                {
                    SaveData.ignoreMouseAcceleration = false;
                    setting.ScriptOnValueChanged();

#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
                    (bool IsCanceled, int Result) = await MessageBoxManager.Show(new Renderer.NameSpacePathReplacePair[] { "sc-krm:gui.yes", "sc-krm:gui.no" }, 1, "sc-krm:options.input.highPrecisionMouse.warning", "sc-krm:gui/icon/exclamation_mark").SuppressCancellationThrow();
                    if (IsCanceled)
                        return;
                    else if (Result == 0)
                    {
                        SaveData.ignoreMouseAcceleration = true;
                        setting.ScriptOnValueChanged();
                    }
#else
                    await MessageBoxManager.Show("sc-krm:gui.yes", 0, "sc-krm:options.input.highPrecisionMouse.os_warning", "sc-krm:gui/exclamation_mark");
#endif
                }
                finally
                {
                    highPrecisionMouseWarningLock = false;
                }
            }
        }



        #region Cursor Pos
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);



        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);



        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);
#endif



        [WikiDescription("커서 위치를 가져옵니다")] public static Vector2Int GetCursorPosition() => GetCursorPosition(0, 0);
        [WikiIgnore] public static Vector2Int GetCursorPosition(Vector2 datumPoint) => GetCursorPosition(datumPoint.x, datumPoint.y);
        [WikiIgnore]
        public static Vector2Int GetCursorPosition(float xDatumPoint, float yDatumPoint)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            bool success = GetCursorPos(out POINT lpPoint);
            if (!success)
                return Vector2Int.zero;

            return new Vector2Int((lpPoint.X - (ScreenManager.currentResolution.width * xDatumPoint)).RoundToInt(), (lpPoint.Y - (ScreenManager.currentResolution.height * yDatumPoint)).RoundToInt());
#else
            throw new NotSupportedException();
#endif
        }


        [WikiDescription("커서 위치를 클라이언트에 맞춰서 가져옵니다")] public static Vector2Int GetClientCursorPosition() => GetClientCursorPosition(0, 0);
        [WikiIgnore] public static Vector2Int GetClientCursorPosition(Vector2 datumPoint) => GetClientCursorPosition(datumPoint.x, datumPoint.y);
        [WikiIgnore]
        public static Vector2Int GetClientCursorPosition(float xDatumPoint, float yDatumPoint)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            bool success = GetCursorPos(out POINT lpPoint);
            if (!success)
                return Vector2Int.zero;

            Vector2Int clientSize = WindowManager.GetClientSize();
            Vector2 border = (Vector2)(WindowManager.GetWindowSize() - clientSize) * 0.5f;
            Vector2Int offset = WindowManager.GetWindowPos(Vector2.zero, Vector2.zero) + new Vector2Int((border.x).RoundToInt(), (border.y).RoundToInt());
            return new Vector2Int(lpPoint.X - (clientSize.x * xDatumPoint).RoundToInt() - offset.x, (lpPoint.Y - offset.y - (clientSize.y * yDatumPoint)).RoundToInt() - offset.y);
#else
            throw new NotSupportedException();
#endif
        }



        [WikiDescription("커서 위치를 변경합니다")] public static void SetCursorPosition(Vector2Int pos) => SetCursorPosition(pos.x, pos.y);
        [WikiIgnore] public static void SetCursorPosition(Vector2Int pos, Vector2 datumPoint) => SetCursorPosition(pos.x, pos.y, datumPoint.x, datumPoint.y);
        [WikiIgnore] public static void SetCursorPosition(int x, int y, float xDatumPoint = 0, float yDatumPoint = 0) => setCursorPosition(x, y, xDatumPoint, yDatumPoint, false);

        static void setCursorPosition(int x, int y, float xDatumPoint, float yDatumPoint, bool force)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            if (!SaveData.ignoreMouseAcceleration || force)
                SetCursorPos((x + ((ScreenManager.currentResolution.width - 1) * xDatumPoint)).RoundToInt(), (y + ((ScreenManager.currentResolution.height - 1) * yDatumPoint)).RoundToInt());
            else
                ignoreMouseAccelerationPos = new Vector2(x, y);
#else
            throw new NotSupportedException();
#endif
        }

        [WikiDescription("커서 위치를 클라이언트에 맞춰서 변경합니다")] public static void SetClientCursorPosition(Vector2Int pos) => SetCursorPosition(pos.x, pos.y);
        [WikiIgnore] public static void SetClientCursorPosition(Vector2Int pos, Vector2 datumPoint) => SetCursorPosition(pos.x, pos.y, datumPoint.x, datumPoint.y);
        [WikiIgnore] public static void SetClientCursorPosition(int x, int y, float xDatumPoint = 0, float yDatumPoint = 0) => setClientCursorPosition(x, y, xDatumPoint, yDatumPoint, false);
        static void setClientCursorPosition(int x, int y, float xDatumPoint, float yDatumPoint, bool force)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            Vector2Int clientSize = WindowManager.GetClientSize();
            Vector2 border = (Vector2)(WindowManager.GetWindowSize() - clientSize) * 0.5f;
            Vector2Int offset = WindowManager.GetWindowPos(Vector2.zero, Vector2.zero) + new Vector2Int((border.x).RoundToInt(), (border.y).RoundToInt());
            int x2 = (x + ((clientSize.x - 1) * xDatumPoint)).RoundToInt() - offset.x;
            int y2 = (y + ((clientSize.y - 1) * yDatumPoint)).RoundToInt() - offset.y;

            if (!SaveData.ignoreMouseAcceleration || force)
                SetCursorPos(x, y);
            else
                ignoreMouseAccelerationPos = new Vector2(x, y);
#else
            throw new NotSupportedException();
#endif
        }
        #endregion



        [WikiIgnore] public Vector2Int ClientPosToScreenPos(Vector2Int pos) => ClientPosToScreenPos(pos.x, pos.y);

        [WikiDescription("클라이언트 좌표를 스크린 좌표로 변경합니다")]
        public Vector2Int ClientPosToScreenPos(int x, int y)
        {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
            POINT point = new POINT() { X = x, Y = y };
            ClientToScreen(WindowManager.currentHandle, ref point);

            return new Vector2Int(point.X, point.Y); 
#else
            throw new NotSupportedException();
#endif
        }
    }
}
