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
                                gameModeList[0].GameStart("asdf");

                                break;
                            }
                        }
                    }
                }
            }

            selectedGameMode = gameModeList[0];
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
