using System.Collections.Specialized;
using System;

namespace SCKRM
{
    public static class TypeUtility
    {
        private static HybridDictionary defaultValueMap = new HybridDictionary();

        public static object GetDefaultValue(this Type type)
        {
            if (!type.IsValueType)
                return null;

            if (defaultValueMap.Contains(type))
                return defaultValueMap[type];

            object defaultValue = Activator.CreateInstance(type);
            defaultValueMap[type] = defaultValue;

            return defaultValue;
        }
    }
}