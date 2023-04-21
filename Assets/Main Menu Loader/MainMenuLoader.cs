using SCKRM;
using SCKRM.Reflection;
using System.Reflection;
using System;

namespace SDJK.MainMenuLoader
{
    public static class MainMenuLoader
    {
        static MainMenuLoader()
        {
            Type[] types = ReflectionManager.types;
            for (int typesIndex = 0; typesIndex < types.Length; typesIndex++)
            {
                MethodInfo[] methodInfos = types[typesIndex].GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                for (int methodInfoIndex = 0; methodInfoIndex < methodInfos.Length; methodInfoIndex++)
                {
                    MethodInfo methodInfo = methodInfos[methodInfoIndex];
                    if (Attribute.GetCustomAttributes(methodInfo, typeof(MainMenuLoadMethodAttribute)).Length > 0 && methodInfo.GetParameters().Length <= 0)
                        loadMethod = methodInfo;
                }
            }
        }

        public static MethodInfo loadMethod { get; private set; }

        public static void Load() => loadMethod?.Invoke(null, null);
    }
}
