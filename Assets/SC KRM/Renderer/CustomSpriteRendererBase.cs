using SCKRM.Resource;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

namespace SCKRM.Renderer
{
    [WikiDescription("모든 스프라이트 렌더러의 부모")]
    public abstract class CustomSpriteRendererBase : CustomRendererBase
    {
        int typeLock = 0;
        [SerializeField] string _type = "";
        public string type
        {
            get
            {
                while (Interlocked.CompareExchange(ref typeLock, 1, 0) != 0)
                    Thread.Sleep(1);

                string type = _type;

                Interlocked.Decrement(ref typeLock);
                return type;
            }
            set
            {
                while (Interlocked.CompareExchange(ref typeLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _type = value;

                Interlocked.Decrement(ref typeLock);
            }
        }

        int indexLock = 0;
        [SerializeField, Min(0)] int _index = 0;
        public int index
        {
            get
            {
                while (Interlocked.CompareExchange(ref indexLock, 1, 0) != 0)
                    Thread.Sleep(1);

                int index = _index;

                Interlocked.Decrement(ref indexLock);
                return index;
            }
            set
            {
                while (Interlocked.CompareExchange(ref indexLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _index = value;

                Interlocked.Decrement(ref indexLock);
            }
        }

        public NameSpaceIndexTypePathPair nameSpaceIndexTypePathPair
        {
            get => new NameSpaceIndexTypePathPair(nameSpace, index, type, path);
            set
            {
                nameSpace = value.nameSpace;
                index = value.index;

                type = value.type;
                path = value.path;
            }
        }

        /*protected virtual void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (!Kernel.isPlaying && UnityEditor.Selection.activeObject != null)
            {
                if (UnityEditor.Selection.activeGameObject.GetComponentInParent<CustomAllRenderer>())

            }
#endif
        }*/

        /*public override void ResourceReload()
        {
            base.ResourceReload();
            queue.Enqueue();
        }*/

        public static Sprite GetSprite(string type, string name, int index, string nameSpace = "")
        {
            if (Kernel.isPlaying && InitialLoadManager.isInitialLoadEnd)
            {
                Sprite[] sprites = ResourceManager.SearchSprites(type, name, nameSpace);
                if (sprites != null && sprites.Length > 0)
                    return sprites[index.Clamp(0, sprites.Length - 1)];

                return null;
            }
            else
            {
                Sprite[] sprites = ResourceManager.GetSprites(Kernel.streamingAssetsPath, type, name, nameSpace);
                if (sprites != null && sprites.Length > 0)
                    return sprites[index.Clamp(0, sprites.Length - 1)];
                else
                    return null;
            }
        }
    }

    public struct NameSpaceTypePathPair
    {
        public string type;
        public string path;
        public string nameSpace;

        public NameSpaceTypePathPair(string type, string path)
        {
            nameSpace = "";

            this.type = type;
            this.path = path;
        }

        public NameSpaceTypePathPair(string nameSpace, string type, string path)
        {
            this.nameSpace = nameSpace;

            this.type = type;
            this.path = path;
        }

        public static implicit operator string(NameSpaceTypePathPair value) => value.ToString();

        public static implicit operator NameSpaceTypePathPair(string value)
        {
            string nameSpace = ResourceManager.GetNameSpace(value, out value);
            string type = ResourceManager.GetTextureType(value, out value);

            return new NameSpaceTypePathPair(nameSpace, type, value);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(nameSpace))
                return PathUtility.Combine(type, path);
            else
                return nameSpace + ":" + PathUtility.Combine(type, path);
        }
    }

    public struct NameSpaceIndexTypePathPair
    {
        public string type;
        public string path;
        public string nameSpace;

        public int index;

        public NameSpaceIndexTypePathPair(string type, string path)
        {
            nameSpace = "";
            index = 0;

            this.type = type;
            this.path = path;
        }

        public NameSpaceIndexTypePathPair(string nameSpace, string type, string path)
        {
            this.nameSpace = nameSpace;
            index = 0;

            this.type = type;
            this.path = path;
        }

        public NameSpaceIndexTypePathPair(string nameSpace, int index, string type, string path)
        {
            this.nameSpace = nameSpace;
            this.index = index;

            this.type = type;
            this.path = path;
        }

        public static implicit operator string(NameSpaceIndexTypePathPair value) => value.ToString();

        public static implicit operator NameSpaceIndexTypePathPair(string value)
        {
            string nameSpace = ResourceManager.GetNameSpace(value, out value);

            int spriteIndex = 0;
            if (value.Contains(':') && !int.TryParse(ResourceManager.GetNameSpace(value, out value), out spriteIndex))
                spriteIndex = -1;

            string type = ResourceManager.GetTextureType(value, out value);
            return new NameSpaceIndexTypePathPair(nameSpace, spriteIndex, type, value);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(nameSpace))
                return ResourceManager.defaultNameSpace + ":" + index + ":" + PathUtility.Combine(type, path);
            else
                return nameSpace + ":" + index + ":" + PathUtility.Combine(type, path);
        }
    }
}