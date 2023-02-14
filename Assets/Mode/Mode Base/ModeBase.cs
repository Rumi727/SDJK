using SCKRM.Renderer;
using SCKRM.SaveLoad;
using SCKRM;
using System;

namespace SDJK.Mode
{
    /// <summary>
    /// 이 인터페이스를 상속하면 SDJK가 모드를 자동으로 감지합니다
    /// </summary>
    [WikiDescription("이 인터페이스를 상속하면 SDJK가 모드를 자동으로 감지합니다")]
    public interface IMode
    {
        public NameSpacePathReplacePair title { get; }
        public int order { get; }

        public NameSpacePathReplacePair displayName { get; }
        public NameSpacePathPair info { get; }

        public NameSpaceIndexTypePathPair icon { get; }

        public string targetRuleset { get; }
        public Type[] incompatibleModes { get; }

        public IModeConfig modeConfig { get; set; }
        public SaveLoadClass modeConfigSlc { get; }

        public IModeConfig CreateModeConfig();

        public static IMode[] emptyModes { get; } = new IMode[0];
    }

    public interface IModeConfig { }

    public sealed class ModeConfigSaveLoadAttribute : SaveLoadBaseAttribute { }

    /// <summary>
    /// <see cref="IMode"/> 인터페이스를 사용할때 커스텀하지 않을경우 권장하는 부모 클래스 입니다
    /// </summary>
    [WikiDescription("IMode 인터페이스를 사용할때 커스텀하지 않을경우 권장하는 부모 클래스 입니다")]
    public abstract class ModeBase : IMode
    {
        public abstract NameSpacePathReplacePair title { get; }
        public abstract int order { get; }

        public abstract NameSpacePathReplacePair displayName { get; }
        public virtual NameSpacePathPair info => "";

        public abstract NameSpaceIndexTypePathPair icon { get; }

        public abstract string targetRuleset { get; }
        public virtual Type[] incompatibleModes => null;

        public virtual IModeConfig modeConfig
        {
            get
            {
                if (_modeConfig == null)
                    _modeConfig = CreateModeConfig();

                return _modeConfig;
            }
            set
            {
                _modeConfig = value;
                _modeConfigSlc = null;
            }
        }
        IModeConfig _modeConfig = null;

        public SaveLoadClass modeConfigSlc
        {
            get
            {
                if (_modeConfigSlc == null)
                {
                    if (modeConfig == null)
                        return null;

                    SaveLoadManager.Initialize(modeConfig.GetType(), typeof(ModeConfigSaveLoadAttribute), out _modeConfigSlc, modeConfig);
                }

                return _modeConfigSlc;
            }
        }
        SaveLoadClass _modeConfigSlc;

        IModeConfig IMode.CreateModeConfig() => CreateModeConfig();
        protected virtual IModeConfig CreateModeConfig() => null;
    }
}
