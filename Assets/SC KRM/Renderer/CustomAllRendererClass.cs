using UnityEngine;
using SCKRM.Resource;
using System.Threading;

namespace SCKRM.Renderer
{
    [WikiDescription("모든 렌더러들의 부모")]
    public abstract class CustomAllRenderer : MonoBehaviour, IRendererRefreshable, INameSpaceKey
    {
        /// <summary>
        /// 렌더링 할 파일의 네임스페이스 (Thread-safe)
        /// </summary>
        [WikiDescription("렌더링 할 파일의 네임스페이스 (Thread-safe)")]
        public string nameSpace
        {
            get
            {
                while (Interlocked.CompareExchange(ref nameSpaceLock, 1, 0) != 0)
                    Thread.Sleep(1);

                string nameSpace = _nameSpace;

                Interlocked.Decrement(ref nameSpaceLock);
                return nameSpace;
            }
            set
            {
                while (Interlocked.CompareExchange(ref nameSpaceLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _nameSpace = value;
                Interlocked.Decrement(ref nameSpaceLock);
            }
        }
        int nameSpaceLock = 0;
        [SerializeField] string _nameSpace = "";

        /// <summary>
        /// 렌더링 할 파일의 경로 (Thread-safe)
        /// </summary>
        [WikiDescription("렌더링 할 파일의 경로 (Thread-safe)")]
        public string path
        {
            get
            {
                while (Interlocked.CompareExchange(ref pathLock, 1, 0) != 0)
                    Thread.Sleep(1);

                string path = _path;

                Interlocked.Decrement(ref pathLock);
                return path;
            }
            set
            {
                while (Interlocked.CompareExchange(ref pathLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _path = value;

                Interlocked.Decrement(ref pathLock);
            }
        }
        int pathLock = 0;
        [SerializeField] string _path = "";
        string INameSpaceKey.key { get => path; set => path = value; }

        /// <summary>
        /// 네임스페이스랑 경로랑 동시에 설정할 수 있습니다 (Thread-safe)
        /// </summary>
        [WikiDescription("네임스페이스랑 경로랑 동시에 설정할 수 있습니다 (Thread-safe)")]
        public NameSpacePathPair nameSpacePathPair
        {
            get => new NameSpacePathPair(nameSpace, path);
            set
            {
                nameSpace = value.nameSpace;
                path = value.path;
            }
        }

        /// <summary>
        /// 렌더러를 새로 고칩니다
        /// </summary>
        [WikiDescription("렌더러를 새로 고칩니다")]
        public abstract void Refresh();
    }

    public struct NameSpacePathPair
    {
        public string path;
        public string nameSpace;

        public NameSpacePathPair(string path)
        {
            nameSpace = "";
            this.path = path;
        }

        public NameSpacePathPair(string nameSpace, string path)
        {
            this.nameSpace = nameSpace;
            this.path = path;
        }

        public static implicit operator string(NameSpacePathPair value) => value.ToString();

        public static implicit operator NameSpacePathPair(string value)
        {
            string nameSpace = ResourceManager.GetNameSpace(value, out value);
            return new NameSpacePathPair(nameSpace, value);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(nameSpace))
                return path;
            else
                return nameSpace + ":" + path;
        }
    }
}