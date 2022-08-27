using System;
using System.Reflection;

namespace SCKRM
{
    /// <summary>
    /// 초기 로딩이 끝나고 씬이 이동되기 전에 메소드를 호출 시켜주는 어트리뷰트 입니다
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [WikiDescription("초기 로딩이 끝나고 씬이 이동되기 전에 메소드를 호출 시켜주는 어트리뷰트 입니다")]
    public class StartenAttribute : Attribute
    {

    }

    public static class StartenManager
    {
        /// <summary>
        /// Startable 어트리뷰트가 붙어있는 모든 메소드를 호출합니다
        /// 기본적으로 초기 로딩이 끝나고 씬이 이동되기 전에 자동으로 호출됩니다
        /// </summary>
        [WikiDescription("Startable 어트리뷰트가 붙어있는 모든 메소드를 호출합니다\n(기본적으로 초기 로딩이 끝나고 씬이 이동되기 전에 자동으로 호출됩니다)")]
        public static void AllStartableMethodAwaken()
        {
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            for (int assemblysIndex = 0; assemblysIndex < assemblys.Length; assemblysIndex++)
            {
                Type[] types = assemblys[assemblysIndex].GetTypes();
                for (int typesIndex = 0; typesIndex < types.Length; typesIndex++)
                {
                    MethodInfo[] methodInfos = types[typesIndex].GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    for (int methodInfoIndex = 0; methodInfoIndex < methodInfos.Length; methodInfoIndex++)
                    {
                        MethodInfo methodInfo = methodInfos[methodInfoIndex];
                        if (Attribute.GetCustomAttributes(methodInfo, typeof(AwakenAttribute)).Length < 0 && methodInfo.GetParameters().Length <= 0)
                            methodInfo.Invoke(null, null);
                    }
                }
            }
        }
    }
}
