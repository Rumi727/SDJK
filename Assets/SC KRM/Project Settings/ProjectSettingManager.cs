using SCKRM.SaveLoad;
using System;

namespace SCKRM.ProjectSetting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ProjectSettingSaveLoadAttribute : SaveLoadAttribute
    {

    }

    public static class ProjectSettingManager
    {
        public static SaveLoadClass[] projectSettingSLCList { get; [Obsolete("It is managed by the Kernel class. Please do not touch it.", false)] internal set; } = new SaveLoadClass[0];
    }
}