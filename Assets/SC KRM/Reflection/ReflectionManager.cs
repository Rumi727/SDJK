using System.Reflection;
using System;
using System.Collections.Generic;

namespace SCKRM.Reflection
{
    public static class ReflectionManager
    {
        static ReflectionManager()
        {
            assemblys = AppDomain.CurrentDomain.GetAssemblies();

            {
                List<Type> result = new List<Type>();
                for (int assemblysIndex = 0; assemblysIndex < assemblys.Length; assemblysIndex++)
                {
                    Type[] types = assemblys[assemblysIndex].GetTypes();
                    for (int typesIndex = 0; typesIndex < types.Length; typesIndex++)
                    {
                        Type type = types[typesIndex];
                        result.Add(type);
                    }
                }

                types = result.ToArray();
            }
        }

        [WikiDescription("로드된 모든 어셈블리")] public static Assembly[] assemblys { get; }
        [WikiDescription("로드된 모든 타입")] public static Type[] types { get; }
    }
}
