using SCKRM.Renderer;
using System;

namespace SCKRM
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SaveLoadUIAttribute : Attribute
    {
        public NameSpacePathPair name { get; } = "";

        /// <summary>
        /// </summary>
        /// <param name="name">type is NameSpacePathPair</param>
        public SaveLoadUIAttribute(string name) => this.name = name;
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SaveLoadUIIgnoreAttribute : Attribute
    {
        
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public abstract class SaveLoadUIConfigBaseAttribute : Attribute
    {
        public NameSpacePathPair name { get; } = "";
        public NameSpacePathPair tooltip { get; } = "";

        public int roundingDigits { get; } = 2;
        public string[] hotkeyToDisplays { get; } = new string[0];

        /// <param name="name">type is NameSpacePathPair</param>
        /// <param name="tooltip">type is NameSpacePathPair</param>
        public SaveLoadUIConfigBaseAttribute(NameSpacePathPair name, NameSpacePathPair tooltip, int roundingDigits = 2, params string[] hotkeyToDisplays)
        {
            this.name = name;
            this.tooltip = tooltip;

            this.roundingDigits = roundingDigits;
            this.hotkeyToDisplays = hotkeyToDisplays;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class SaveLoadUIColorPickerConfigAttribute : SaveLoadUIConfigBaseAttribute
    {
        public bool alphaShow { get; } = true;

        /// <param name="name">type is NameSpacePathPair</param>
        /// <param name="tooltip">type is NameSpacePathPair</param>
        public SaveLoadUIColorPickerConfigAttribute(string name, string tooltip, bool alphaShow = true, params string[] hotkeyToDisplays) : base(name, tooltip, 2, hotkeyToDisplays) => this.alphaShow = alphaShow;
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SaveLoadUIInputFieldConfigAttribute : SaveLoadUIConfigBaseAttribute
    {
        public float mouseSensitivity { get; } = 1;

        /// <param name="name">type is NameSpacePathPair</param>
        /// <param name="tooltip">type is NameSpacePathPair</param>
        public SaveLoadUIInputFieldConfigAttribute(string name, string tooltip, float mouseSensitivity = 1, int roundingDigits = 2, params string[] hotkeyToDisplays) : base(name, tooltip, roundingDigits, hotkeyToDisplays) => this.mouseSensitivity = mouseSensitivity;
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class SaveLoadUISliderConfigAttribute : SaveLoadUIInputFieldConfigAttribute
    {
        public float min { get; } = 0;
        public float max { get; } = 1;

        /// <param name="name">type is NameSpacePathPair</param>
        /// <param name="tooltip">type is NameSpacePathPair</param>
        public SaveLoadUISliderConfigAttribute(string name, string tooltip, float min, float max, float mouseSensitivity = 1, int roundingDigits = 2, params string[] hotkeyToDisplays) : base(name, tooltip, mouseSensitivity, roundingDigits, hotkeyToDisplays)
        {
            this.min = min;
            this.max = max;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class SaveLoadUIToggleConfigAttribute : SaveLoadUIConfigBaseAttribute
    {
        /// <param name="name">type is NameSpacePathPair</param>
        /// <param name="tooltip">type is NameSpacePathPair</param>
        public SaveLoadUIToggleConfigAttribute(string name, string tooltip, params string[] hotkeyToDisplays) : base(name, tooltip, 2, hotkeyToDisplays) { }
    }
}
