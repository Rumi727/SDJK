using SCKRM.SaveLoad;
using System;

namespace SCKRM.ProjectSetting
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ProjectSettingSaveLoadAttribute : SaveLoadBaseAttribute
    {

    }

    [WikiDescription("프로젝트 설정을 관리하는 클래스 입니다")]
    public static class ProjectSettingManager
    {
        public static SaveLoadClass[] projectSettingSLCList { get; [Obsolete("It is managed by the Kernel class. Please do not touch it.", false)] internal set; } = new SaveLoadClass[0];
    }
}