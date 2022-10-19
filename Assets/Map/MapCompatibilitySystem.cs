using Newtonsoft.Json.Linq;
using SCKRM;
using SCKRM.Easing;
using SCKRM.Json;
using SDJK.Map;
using SDJK.Ruleset;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Version = SCKRM.Version;

namespace SDJK.Map
{
    public static class MapCompatibilitySystem
    {
        public static string[] compatibleMapExtensions { get; } = new string[] { "sdjk", "adofai" };
        public static event GlobalMapCompatibilityAction globalMapCompatibilityAction;

        /// <summary>
        ///
        /// </summary>
        /// <param name="type">맵의 타입</param>
        /// <param name="mapFilePath">맵 파일의 경로</param>
        /// <param name="extension">맵 파일의 확장자</param>
        /// <returns>
        /// 맵 인스턴스
        /// </returns>
        public delegate object GlobalMapCompatibilityAction(Type type, string mapFilePath, string extension);

        public static T GlobalMapCompatibility<T>(string mapFilePath) where T : Map, new()
        {
            Delegate[] delegates = globalMapCompatibilityAction.GetInvocationList();
            for (int i = 0; i < delegates.Length; i++)
            {
                GlobalMapCompatibilityAction action = (GlobalMapCompatibilityAction)delegates[i];
                object map = action.Invoke(typeof(T), mapFilePath, Path.GetExtension(mapFilePath));
                if (map != null)
                    return (T)map;
            }

            return null;
        }
    }
}
