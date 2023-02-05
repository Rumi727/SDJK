using System.Collections.Generic;

namespace SDJK.Mode
{
    public static class ModeUtility
    {
        public static IMode FindMode<T>(this IList<IMode> modes) where T : IMode
        {
            for (int i = 0; i < modes.Count; i++)
            {
                IMode mode = modes[i];
                if (mode is T)
                    return mode;
            }

            return null;
        }
    }
}
