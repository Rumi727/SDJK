using SCKRM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SDJK
{
    public static class GameModeManager
    {
        public static List<IGameMode> gameModeList { get; } = new List<IGameMode>();
        public static IGameMode selectedGameMode { get; private set; } = null;

        [Awaken]
        public static void GameModeListRefresh()
        {
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int assemblysIndex = 0; assemblysIndex < assemblys.Length; assemblysIndex++)
            {
                Type[] types = assemblys[assemblysIndex].GetTypes();
                for (int typesIndex = 0; typesIndex < types.Length; typesIndex++)
                {
                    Type type = types[typesIndex];
                    if (type.IsPublic && type.IsClass && !type.IsSpecialName)
                    {
                        Type[] interfaces = type.GetInterfaces();
                        for (int interfaceIndex = 0; interfaceIndex < interfaces.Length; interfaceIndex++)
                        {
                            Type interfaceType = interfaces[interfaceIndex];
                            if (interfaceType == typeof(IGameMode))
                            {
                                gameModeList.Add((IGameMode)Activator.CreateInstance(type));
                                break;
                            }
                        }
                    }
                }
            }

            selectedGameMode = gameModeList[0];
        }

        /// <summary>
        /// 현제 선택된 모드랑 호환되는 모드인지 확인합니다
        /// </summary>
        /// <param name="gameMode"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [WikiDescription("현제 선택된 모드랑 호환되는 모드인지 확인합니다")]
        public static bool IsCompatibleMode(this IGameMode gameMode, string mode)
        {
            if (selectedGameMode.gameModeName == mode)
                return true;

            if (gameMode.compatibleMode == null)
                return false;

            for (int i = 0; i < gameMode.compatibleMode.Length; i++)
            {
                if (selectedGameMode.gameModeName == gameMode.compatibleMode[i])
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    /// 이 인터페이스를 상속하면 SDJK가 게임 모드를 자동으로 감지합니다
    /// </summary>
    [WikiDescription("이 인터페이스를 상속하면 SDJK가 게임 모드를 자동으로 감지합니다")]
    public interface IGameMode
    {
        public string gameModeName { get; }
        public string[] compatibleMode { get; }

        public void GameStart(string mapFilePath);
    }
}
