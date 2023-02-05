using System;
using System.Collections.Generic;

namespace SDJK.Mode
{
    public static class ModeUtility
    {
        public static IMode FindMode<T>(this IList<IMode> modes) where T : IMode => FindMode(modes, typeof(T));

        public static IMode FindMode(this IList<IMode> modes, Type targetType)
        {
            for (int i = 0; i < modes.Count; i++)
            {
                IMode mode = modes[i];
                Type modeType = mode.GetType();

                if (modeType == targetType || modeType.IsSubclassOf(targetType))
                    return mode;
            }

            return null;
        }
    }
}
