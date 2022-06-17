using SCKRM.Language;
using SCKRM.Resource;
using System.Threading;
using UnityEngine;

namespace SCKRM.Renderer
{
    public abstract class CustomAllTextRenderer : CustomAllRenderer
    {
        public NameSpacePathReplacePair nameSpacePathReplacePair
        {
            get => new NameSpacePathReplacePair(nameSpace, path, replace);
            set
            {
                nameSpace = value.nameSpace;
                path = value.path;

                replace = value.replace;
            }
        }

        int replaceLock = 0;
        [SerializeField] ReplaceOldNewPair[] _replace = new ReplaceOldNewPair[0];
        public ReplaceOldNewPair[] replace
        {
            get
            {
                while (Interlocked.CompareExchange(ref replaceLock, 1, 0) != 0)
                    Thread.Sleep(1);

                ReplaceOldNewPair[] replace = _replace;

                Interlocked.Decrement(ref replaceLock);
                return replace;
            }
            set
            {
                while (Interlocked.CompareExchange(ref replaceLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _replace = value;

                Interlocked.Decrement(ref replaceLock);
            }
        }


        public string GetText()
        {
            string text;
            if (Kernel.isPlaying)
                text = ResourceManager.SearchLanguage(path, nameSpace);
            else
                text = LanguageManager.LanguageLoad(path, nameSpace, "en_us");

            if (replace != null)
            {
                for (int i = 0; i < replace.Length; i++)
                    text = text.Replace(replace[i].replaceOld, replace[i].replaceNew);
            }
            
            if (text != "")
                return text;
            else
                return path;
        }
    }

    public struct NameSpacePathReplacePair
    {
        public string path;
        public string nameSpace;

        public ReplaceOldNewPair[] replace;

        public NameSpacePathReplacePair(string path)
        {
            nameSpace = "";
            this.path = path;

            replace = new ReplaceOldNewPair[0];
        }

        public NameSpacePathReplacePair(string nameSpace, string path)
        {
            this.nameSpace = nameSpace;
            this.path = path;

            replace = new ReplaceOldNewPair[0];
        }

        public NameSpacePathReplacePair(string nameSpace, string path, params ReplaceOldNewPair[] replace)
        {
            this.nameSpace = nameSpace;
            this.path = path;

            this.replace = replace;
        }

        public static implicit operator string(NameSpacePathReplacePair value) => value.ToString();

        public static implicit operator NameSpacePathReplacePair(string value)
        {
            string nameSpace = ResourceManager.GetNameSpace(value, out value);
            return new NameSpacePathReplacePair(nameSpace, value);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(nameSpace))
                return path;
            else
                return nameSpace + ":" + path;
        }
    }

    public struct ReplaceOldNewPair
    {
        public string replaceOld;
        public string replaceNew;

        public ReplaceOldNewPair(string replaceOld, string replaceNew)
        {
            this.replaceOld = replaceOld;
            this.replaceNew = replaceNew;
        }
    }
}