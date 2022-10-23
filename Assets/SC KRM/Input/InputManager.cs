using Newtonsoft.Json;
using SCKRM.Cursor;
using SCKRM.Loading;
using SCKRM.ProjectSetting;
using SCKRM.SaveLoad;
using SCKRM.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace SCKRM.Input
{
    [WikiDescription("조작을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Input/Input Manager", 0)]
    public sealed class InputManager : MonoBehaviour
    {
        [ProjectSettingSaveLoad]
        public sealed class Data
        {
            [JsonProperty] public static Dictionary<string, List<KeyCode>> controlSettingList { get; set; } = new Dictionary<string, List<KeyCode>>();
            [JsonProperty] public static Dictionary<string, bool> inputLockList { get; set; } = new Dictionary<string, bool>();
        }

        [GeneralSaveLoad]
        public sealed class SaveData
        {
            [JsonProperty] public static Dictionary<string, List<KeyCode>> controlSettingList { get; set; } = new Dictionary<string, List<KeyCode>>();
            [JsonProperty] public static bool mouseUpsideDown { get; set; } = false;
        }



        /// <summary>
        /// KeyCode의 모든 값
        /// Any value of KeyCode
        /// </summary>
        [WikiDescription("KeyCode의 모든 값")]
        public static KeyCode[] unityKeyCodeList { get; } = (KeyCode[])Enum.GetValues(typeof(KeyCode));



        static Dictionary<string, List<KeyCode>> _controlSettingList;
        /// <summary>
        /// 조작 설정 리스트 (세이브 포함)
        /// Control setting list (including save)
        /// </summary>
        [WikiDescription("조작 설정 리스트 (세이브 포함)")]
        public static Dictionary<string, List<KeyCode>> controlSettingList
        {
            get
            {
                if (!Kernel.isPlaying)
                    throw new NotPlayModeMethodException();

                if (_controlSettingList == null)
                    _controlSettingList = InputListMerge();

                return _controlSettingList;
            }
        }

        static Dictionary<string, List<KeyCode>> InputListMerge()
        {
            Dictionary<string, List<KeyCode>> mergeList = new Dictionary<string, List<KeyCode>>();
            foreach (var item in Data.controlSettingList)
            {
                if (SaveData.controlSettingList.TryGetValue(item.Key, out List<KeyCode> value))
                    mergeList.Add(item.Key, value);
                else
                    mergeList.Add(item.Key, item.Value);
            }

            return mergeList;
        }



        /// <summary>
        /// 강제 인풋 락
        /// </summary>
        [WikiDescription("강제 인풋 락")]
        public static bool forceInputLock { get; set; } = false;
        /// <summary>
        /// 로딩 인풋 락
        /// </summary>
        [WikiDescription("로딩 인풋 락")]
        public static bool loadingInputLock => LoadingAniManager.isLoading;



        public static string[] inputLockDenyEmpty { get; } = new string[0];
        public static string[] inputLockDenyAll { get; } = new string[] { "all" };
        public static string[] inputLockDenyForce { get; } = new string[] { "force" };
        public static string[] inputLockDenyInput { get; } = new string[] { "input" };
        public static string[] inputLockDenyAllForce { get; } = new string[] { "all", "force" };
        public static string[] inputLockDenyAllInput { get; } = new string[] { "all", "input" };
        public static string[] inputLockDenyForceInput { get; } = new string[] { "force", "input" };
        public static string[] inputLockDenyAllForceInput { get; } = new string[] { "all", "force", "input" };



        [WikiDescription("조작 설정 리셋 이벤트")]
        public static event Action controlSaveDataResetEvent;



        /// <summary>
        /// 픽셀 좌표의 현재 마우스 위치
        /// The current mouse position in pixel coordinates
        /// </summary>
        [WikiDescription("픽셀 좌표의 현재 마우스 위치\nThe current mouse position in pixel coordinates")]
        public static Vector2 mousePosition { get; private set; } = Vector2.zero;



#if UNITY_EDITOR_LINUX || (!UNITY_EDITOR && (UNITY_STANDALONE_LINUX || UNITY_WEBGL))
        public static bool mousePresent => true;
#elif !UNITY_EDITOR && (UNITY_IOS || UNITY_WII)
        public static bool mousePresent => false;
#else
        [WikiDescription("마우스 장치가 감지되었는지 여부를 나타냅니다\nIndicates if a mouse device is detected")]
        public static bool mousePresent => UnityEngine.Input.mousePresent;
#endif



        [WikiDescription("모든 텍스트 메쉬 프로의 인풋 필드를 가져옵니다")] public static TMP_InputField[] allInputFields { get; private set; }
        [WikiDescription("텍스트 메쉬 프로의 인풋 필드가 하나라도 포커스 되어있다면 true를 반환합니다")] public static bool isInputFieldFocused { get; set; }



        #region Input Check
        static readonly Func<KeyCode, bool> down = UnityEngine.Input.GetKeyDown;
        static readonly Func<KeyCode, bool> alway = UnityEngine.Input.GetKey;
        static readonly Func<KeyCode, bool> up = UnityEngine.Input.GetKeyUp;



        static bool TryInputCheck(string key, Func<KeyCode, bool> func)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(GetKey));
#if UNITY_EDITOR
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(GetKey));
#endif
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(GetKey));

            if (controlSettingList.TryGetValue(key, out List<KeyCode> list))
            {
                if (list == null)
                    return false;
                else if (list.Count <= 0)
                    return false;

                for (int i = 0; i < list.Count; i++)
                {
                    if (i != list.Count - 1)
                    {
                        if (!UnityEngine.Input.GetKey(list[i]))
                            return false;
                    }
                    else if (!func.Invoke(list[i]))
                        return false;
                }

                return true;
            }

            return false;
        }
        static bool InputCheck(string key, Func<KeyCode, bool> func)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(GetKey));
#if UNITY_EDITOR
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(GetKey));
#endif
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(GetKey));

            List<KeyCode> list = controlSettingList[key];
            if (list == null)
                return false;
            else if (list.Count <= 0)
                return false;

            for (int i = 0; i < list.Count; i++)
            {
                if (i != list.Count - 1)
                {
                    if (!UnityEngine.Input.GetKey(list[i]))
                        return false;
                }
                else if (!func.Invoke(list[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 사용자가 키 KeyCode 열거형 매개변수로 식별되는 키를 누르고 있는 동안 true를 반환합니다
        /// Returns true while the user holds down the key identified by the key KeyCode enum parameter
        /// </summary>
        /// <param name="keyCode">
        /// 키 코드
        /// Key Code
        /// </param>
        /// <param name="inputType">
        /// 인풋 타입
        /// Input Type
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        [WikiDescription(
@"사용자가 키 KeyCode 열거형 매개변수로 식별되는 키를 누르고 있는 동안 true를 반환합니다
Returns true while the user holds down the key identified by the key KeyCode enum parameter"
)]
        public static bool GetKey(KeyCode keyCode, InputType inputType = InputType.Down) => GetKey(keyCode, inputType, inputLockDenyEmpty);

        /// <summary>
        /// 사용자가 키 KeyCode 열거형 매개변수로 식별되는 키를 누르고 있는 동안 true를 반환합니다
        /// Returns true while the user holds down the key identified by the key KeyCode enum parameter
        /// </summary>
        /// <param name="keyCode">
        /// 키 코드
        /// Key Code
        /// </param>
        /// <param name="inputType">
        /// 인풋 타입
        /// Input Type
        /// </param>
        /// <param name="inputLockDeny">
        /// 무시할 인풋 락
        /// input lock to ignore
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        [WikiIgnore]
        public static bool GetKey(KeyCode keyCode, InputType inputType = InputType.Down, params string[] inputLockDeny)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(GetKey));
#if UNITY_EDITOR
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(GetKey));
#endif
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(GetKey));

            if (inputLockDeny == null)
                inputLockDeny = new string[0];

            if (!InputLockCheck(inputLockDeny))
            {
                if (inputType == InputType.Down && UnityEngine.Input.GetKeyDown(keyCode))
                    return true;
                else if (inputType == InputType.Alway && UnityEngine.Input.GetKey(keyCode))
                    return true;
                else if (inputType == InputType.Up && UnityEngine.Input.GetKeyUp(keyCode))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 사용자가 딕셔너리로 식별된 키를 누르고 있는 동안 true를 반환합니다.
        /// Returns true while the user is holding down the key identified by the dictionary.
        /// </summary>
        /// <param name="key">
        /// 키
        /// Key
        /// </param>
        /// <param name="inputType">
        /// 인풋 타입
        /// Input Type
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        [WikiDescription(
@"사용자가 딕셔너리로 식별된 키를 누르고 있는 동안 true를 반환합니다.
Returns true while the user is holding down the key identified by the dictionary."
)]
        public static bool TryGetKey(string key, InputType inputType = InputType.Down) => TryGetKey(key, inputType, inputLockDenyEmpty);

        /// <summary>
        /// 사용자가 딕셔너리로 식별된 키를 누르고 있는 동안 true를 반환합니다.
        /// Returns true while the user is holding down the key identified by the dictionary.
        /// </summary>
        /// <param name="key">
        /// 키
        /// Key
        /// </param>
        /// <param name="inputType">
        /// 인풋 타입
        /// Input Type
        /// </param>
        /// <param name="inputLockDeny">
        /// 무시할 인풋 락
        /// input lock to ignore
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        [WikiIgnore]
        public static bool TryGetKey(string key, InputType inputType = InputType.Down, params string[] inputLockDeny)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(GetKey));
#if UNITY_EDITOR
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(GetKey));
#endif
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(GetKey));

            if (inputLockDeny == null)
                inputLockDeny = new string[0];

            if (!InputLockCheck(inputLockDeny))
            {
                if (inputType == InputType.Down && TryInputCheck(key, down))
                    return true;
                else if (inputType == InputType.Alway && TryInputCheck(key, alway))
                    return true;
                else if (inputType == InputType.Up && TryInputCheck(key, up))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 사용자가 딕셔너리로 식별된 키를 누르고 있는 동안 true를 반환합니다.
        /// Returns true while the user is holding down the key identified by the dictionary.
        /// </summary>
        /// <param name="key">
        /// 키
        /// Key
        /// </param>
        /// <param name="inputType">
        /// 인풋 타입
        /// Input Type
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        [WikiDescription(
@"사용자가 딕셔너리로 식별된 키를 누르고 있는 동안 true를 반환합니다.
Returns true while the user is holding down the key identified by the dictionary."
)]
        public static bool GetKey(string key, InputType inputType = InputType.Down) => GetKey(key, inputType, inputLockDenyEmpty);

        /// <summary>
        /// 사용자가 딕셔너리로 식별된 키를 누르고 있는 동안 true를 반환합니다.
        /// Returns true while the user is holding down the key identified by the dictionary.
        /// </summary>
        /// <param name="key">
        /// 키
        /// Key
        /// </param>
        /// <param name="inputType">
        /// 인풋 타입
        /// Input Type
        /// </param>
        /// <param name="inputLockDeny">
        /// 무시할 인풋 락
        /// input lock to ignore
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        [WikiIgnore]
        public static bool GetKey(string key, InputType inputType = InputType.Down, params string[] inputLockDeny)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(GetKey));
#if UNITY_EDITOR
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(GetKey));
#endif
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(GetKey));

            if (inputLockDeny == null)
                inputLockDeny = new string[0];

            if (!InputLockCheck(inputLockDeny))
            {
                if (inputType == InputType.Down && InputCheck(key, down))
                    return true;
                else if (inputType == InputType.Alway && InputCheck(key, alway))
                    return true;
                else if (inputType == InputType.Up && InputCheck(key, up))
                    return true;
            }

            return false;
        }
        #endregion

        #region Mouse Input Check
        static Vector2 mouseDelta = Vector2.zero;

        /// <summary>
        /// 현재 마우스 델타
        /// The current mouse delta
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        [WikiDescription("현재 마우스 델타\nThe current mouse delta")]
        public static Vector2 GetMouseDelta(bool ignoreMouseSensitivity = false) => GetMouseDelta(ignoreMouseSensitivity, inputLockDenyEmpty);

        /// <summary>
        /// 현재 마우스 델타
        /// The current mouse delta
        /// </summary>
        /// <param name="inputLockDeny">
        /// 무시할 인풋 락
        /// input lock to ignore
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        [WikiIgnore]
        public static Vector2 GetMouseDelta(bool ignoreMouseSensitivity = false, params string[] inputLockDeny)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(GetMouseDelta));

#if UNITY_EDITOR
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(GetMouseDelta));
#endif
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(GetMouseDelta));

            if (inputLockDeny == null)
                inputLockDeny = new string[0];

            if (!InputLockCheck(inputLockDeny))
            {
                if (!ignoreMouseSensitivity)
                    return mouseDelta * CursorManager.SaveData.mouseSensitivity;
                else
                    return mouseDelta;
            }
            else
                return Vector2.zero;
        }

        /// <summary>
        /// 주어진 마우스 버튼을 누르고 있는지 여부를 반환합니다
        /// Returns whether the given mouse button is held down
        /// </summary>
        /// <param name="button">
        /// 버튼
        /// Button
        /// </param>
        /// <param name="inputType">
        /// 인풋 타입
        /// Input Type
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        [WikiDescription(
@"주어진 마우스 버튼을 누르고 있는지 여부를 반환합니다
Returns whether the given mouse button is held down"
)]
        public static bool GetMouseButton(int button, InputType inputType = InputType.Down) => GetMouseButton(button, inputType, inputLockDenyEmpty);

        /// <summary>
        /// 주어진 마우스 버튼을 누르고 있는지 여부를 반환합니다
        /// Returns whether the given mouse button is held down
        /// </summary>
        /// <param name="button">
        /// 버튼
        /// Button
        /// </param>
        /// <param name="inputType">
        /// 인풋 타입
        /// Input Type
        /// </param>
        /// <param name="inputLockDeny">
        /// 무시할 인풋 락
        /// input lock to ignore
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        [WikiIgnore]
        public static bool GetMouseButton(int button, InputType inputType = InputType.Down, params string[] inputLockDeny)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(GetMouseButton));

#if UNITY_EDITOR
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(GetMouseButton));
#endif
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(GetMouseButton));

            if (inputLockDeny == null)
                inputLockDeny = new string[0];

            if (!InputLockCheck(inputLockDeny))
            {
                if (inputType == InputType.Down)
                    return UnityEngine.Input.GetMouseButtonDown(button);
                else if (inputType == InputType.Alway)
                    return UnityEngine.Input.GetMouseButton(button);
                else if (inputType == InputType.Up)
                    return UnityEngine.Input.GetMouseButtonUp(button);

                return false;
            }
            else
                return false;
        }

        static Vector2 mouseScrollDelta = Vector2.zero;
        /// <summary>
        /// 현재 마우스 스크롤 델타
        /// The current mouse delta
        /// </summary>
        /// <param name="inputLockDeny">
        /// 무시할 인풋 락
        /// input lock to ignore
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        [WikiDescription(
@"현재 마우스 스크롤 델타
The current mouse delta"
)]
        public static Vector2 GetMouseScrollDelta(params string[] inputLockDeny)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(GetMouseScrollDelta));

#if UNITY_EDITOR
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(GetMouseScrollDelta));
#endif
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(GetMouseScrollDelta));

            if (inputLockDeny == null)
                inputLockDeny = new string[0];

            if (!InputLockCheck(inputLockDeny))
                return mouseScrollDelta;
            else
                return Vector2.zero;
        }



        /// <summary>
        /// 사용자가 키나 마우스 버튼을 누르는 첫 번째 프레임에 true를 반환합니다
        /// Returns true the first frame the user hits any key or mouse button
        /// </summary>
        /// <param name="inputLockDeny">
        /// 무시할 인풋 락
        /// input lock to ignore
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        [WikiDescription(
@"사용자가 키나 마우스 버튼을 누르는 첫 번째 프레임에 true를 반환합니다
Returns true the first frame the user hits any key or mouse button"
)]
        public static bool GetAnyKeyDown(params string[] inputLockDeny)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(GetAnyKeyDown));

#if UNITY_EDITOR
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(GetAnyKeyDown));
#endif
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(GetAnyKeyDown));

            if (inputLockDeny == null)
                inputLockDeny = new string[0];

            return !InputLockCheck(inputLockDeny) && UnityEngine.Input.anyKeyDown;
        }

        /// <summary>
        /// 현재 누르고 있는 키나 마우스 버튼이 있습니까?
        /// Is any key or mouse button currently held down?
        /// </summary>
        /// <param name="inputLockDeny">
        /// 무시할 인풋 락
        /// input lock to ignore
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        [WikiDescription(
@"현재 누르고 있는 키나 마우스 버튼이 있습니까?
Is any key or mouse button currently held down?"
)]
        public static bool GetAnyKey(params string[] inputLockDeny)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(GetAnyKey));

#if UNITY_EDITOR
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(GetAnyKey));
#endif
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(GetAnyKey));

            if (inputLockDeny == null)
                inputLockDeny = new string[0];

            return !InputLockCheck(inputLockDeny) && UnityEngine.Input.anyKey;
        }
        #endregion

        #region Input Lock
        /// <summary>
        /// 인풋 락을 체크합니다 무시할 인풋 락을 제외한 락중에 하나라도 락이 걸려있다면 true를 반환합니다
        /// Checks input locks. Returns true if any of the locks except the input lock to ignore are locked.
        /// </summary>
        /// <param name="inputLockDeny">
        /// 무시할 인풋 락
        /// input lock to ignore
        /// </param>
        /// <returns></returns>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        [WikiDescription(
@"인풋 락을 체크합니다 무시할 인풋 락을 제외한 락중에 하나라도 락이 걸려있다면 true를 반환합니다
Checks input locks. Returns true if any of the locks except the input lock to ignore are locked."
)]
        public static bool InputLockCheck(params string[] inputLockDeny)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(InputLockCheck));

#if UNITY_EDITOR
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(InputLockCheck));
#endif
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(InputLockCheck));

            if (inputLockDeny == null)
                inputLockDeny = new string[0];

            if (loadingInputLock)
                return true;
            else if (!inputLockDeny.Contains("input") && isInputFieldFocused)
                return true;
            else if (!inputLockDeny.Contains("force") && forceInputLock)
                return true;
            else if (inputLockDeny.Contains("all"))
                return false;

            foreach (var item in Data.inputLockList)
            {
                if (item.Value && !inputLockDeny.Contains(item.Key))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 인풋 락을 설정합니다
        /// Set input lock
        /// </summary>
        /// <param name="key">
        /// 설정할 키
        /// key to set
        /// </param>
        /// <param name="value">
        /// 잠금?
        /// lock?
        /// </param>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public static void SetInputLock(string key, bool value)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(SetInputLock));
#if UNITY_EDITOR
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(SetInputLock));
#endif
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(SetInputLock));

            Data.inputLockList[key] = value;
        }

        /// <summary>
        /// 인풋 락을 설정합니다
        /// Set input lock
        /// </summary>
        /// <param name="key">
        /// 설정할 키
        /// key to set
        /// </param>
        /// <param name="value">
        /// 잠금?
        /// lock?
        /// </param>
        /// <exception cref="NotMainThreadMethodException"></exception>
        /// <exception cref="NotPlayModeMethodException"></exception>
        /// <exception cref="NotInitialLoadEndMethodException"></exception>
        public static void TrySetInputLock(string key, bool value)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(SetInputLock));
#if UNITY_EDITOR
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(SetInputLock));
#endif
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(SetInputLock));

            if (Data.inputLockList.ContainsKey(key))
                Data.inputLockList[key] = value;
        }
        #endregion



        public static void ControlListRefresh() => _controlSettingList = InputListMerge();
        public static void ControlSaveDataReset()
        {
            SaveData.controlSettingList.Clear();
            ControlListRefresh();

            controlSaveDataResetEvent?.Invoke();
        }



        [SerializeField] void OnDelta(InputAction.CallbackContext context) => mouseDelta = context.ReadValue<Vector2>();
        [SerializeField] void OnPosition(InputAction.CallbackContext context) => mousePosition = context.ReadValue<Vector2>();
        [SerializeField] void OnScroll(InputAction.CallbackContext context) => mouseScrollDelta = context.ReadValue<Vector2>();

        static int allSelectableCount;
        void Update()
        {
            if (allSelectableCount != Selectable.allSelectableCount)
            {
                allInputFields = Selectable.allSelectablesArray.OfType<TMP_InputField>().ToArray();
                allSelectableCount = Selectable.allSelectableCount;
            }

            isInputFieldFocused = false;
            for (int i = 0; i < allInputFields.Length; i++)
            {
                if (allInputFields[i].isFocused)
                {
                    isInputFieldFocused = true;
                    break;
                }
            }
        }
    }

    public enum InputType
    {
        Down,
        Alway,
        Up,
    }
}