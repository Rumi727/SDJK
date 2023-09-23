using SCKRM.Resource;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SCKRM.Renderer
{
    [WikiDescription("모든 스프라이트 렌더러의 부모")]
    public abstract class CustomSpriteRendererBase : CustomRendererBase
    {
        /// <summary>
        /// cachedLocalSprites[nameSpace][type][name][tag] = Sprite[];
        /// </summary>
        protected static Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Sprite[]>>>> cachedLocalSprites = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, Sprite[]>>>>();

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

        int spriteTagLock = 0;
        [SerializeField] string _spriteTag = ResourceManager.spriteDefaultTag;
        public string spriteTag
        {
            get
            {
                while (Interlocked.CompareExchange(ref spriteTagLock, 1, 0) != 0)
                    Thread.Sleep(1);

                string spriteTag = _spriteTag;

                Interlocked.Decrement(ref spriteTagLock);
                return spriteTag;
            }
            set
            {
                while (Interlocked.CompareExchange(ref spriteTagLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _spriteTag = value;

                Interlocked.Decrement(ref spriteTagLock);
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

        int nameSpaceIndexTypePathPairLock = 0;
        public NameSpaceIndexTypePathPair nameSpaceIndexTypePathPair
        {
            get
            {
                while (Interlocked.CompareExchange(ref nameSpaceIndexTypePathPairLock, 1, 0) != 0)
                    Thread.Sleep(1);

                NameSpaceIndexTypePathPair result;

                try
                {
                    result = new NameSpaceIndexTypePathPair(nameSpace, index, type, path);
                }
                finally
                {
                    Interlocked.Decrement(ref nameSpaceIndexTypePathPairLock);
                }

                return result;
            }

            set
            {
                while (Interlocked.CompareExchange(ref nameSpaceIndexTypePathPairLock, 1, 0) != 0)
                    Thread.Sleep(1);

                nameSpace = value.nameSpace;
                index = value.index;

                type = value.type;
                path = value.path;

                Interlocked.Decrement(ref nameSpaceIndexTypePathPairLock);
            }
        }

        int forceLocalSpriteLock = 0;
        public bool forceLocalSprite
        {
            get
            {
                while (Interlocked.CompareExchange(ref forceLocalSpriteLock, 1, 0) != 0)
                    Thread.Sleep(1);

                bool result = _forceLocalSprite;
                Interlocked.Decrement(ref forceLocalSpriteLock);

                return result;
            }
        }

        [SerializeField] bool _forceLocalSprite = false;

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

        public Sprite GetSprite() => GetSprite(type, path, index, nameSpace, spriteTag, forceLocalSprite);

        public static Sprite GetSprite(string type, string name, int index, string nameSpace = "", string tag = ResourceManager.spriteDefaultTag, bool forceLocalSprite = false)
        {
            if (Kernel.isPlaying && InitialLoadManager.isInitialLoadEnd && !forceLocalSprite)
            {
                Sprite[] sprites = ResourceManager.SearchSprites(type, name, nameSpace, tag);
                if (sprites != null && sprites.Length > 0)
                    return sprites[index.Clamp(0, sprites.Length - 1)];

                return null;
            }
            else
            {
                if (Kernel.isPlaying)
                {
                    if (cachedLocalSprites.TryGetValue(nameSpace, out var result))
                    {
                        if (result.TryGetValue(type, out var result2))
                        {
                            if (result2.TryGetValue(name, out var sprites))
                            {
                                Sprite[] sprites2 = null;
                                if (sprites.ContainsKey(tag))
                                    sprites2 = sprites[tag];
                                else if (sprites.ContainsKey(ResourceManager.spriteDefaultTag))
                                    sprites2 = sprites[ResourceManager.spriteDefaultTag];

                                if (sprites2 != null && sprites2.Length > 0)
                                    return sprites2[index.Clamp(0, sprites2.Length - 1)];
                                else
                                    return null;
                            }
                            else
                                result2[name] = new Dictionary<string, Sprite[]>();
                        }
                        else
                        {
                            result[nameSpace][type] = new()
                            {
                                [name] = new Sprite[0]
                            };
                        }
                    }
                    else
                    {
                        cachedLocalSprites[nameSpace] = new Dictionary<string, Dictionary<string, Dictionary<string, Sprite[]>>>
                        {
                            [type] = new()
                            {
                                [name] = new()
                            }
                        };
                    }
                }

                {
                    Dictionary<string, Sprite[]> sprites = ResourceManager.GetSprites(Kernel.streamingAssetsPath, type, name, nameSpace);
                    if (sprites == null)
                        return null;

                    Sprite[] sprites2 = null;
                    if (sprites.ContainsKey(tag))
                        sprites2 = sprites[tag];
                    else if (sprites.ContainsKey(ResourceManager.spriteDefaultTag))
                        sprites2 = sprites[ResourceManager.spriteDefaultTag];

                    if (Kernel.isPlaying)
                        cachedLocalSprites[nameSpace][type][name] = sprites;

                    if (sprites2 != null && sprites2.Length > 0)
                        return sprites2[index.Clamp(0, sprites2.Length - 1)];
                    else
                        return null;
                }
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