using Newtonsoft.Json;
using SCKRM.ProjectSetting;
using SCKRM.Renderer;
using SCKRM.Threads;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SCKRM.Object
{
    [WikiDescription("오브젝트 풀링을 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Object/Object Pooling System", 0)]
    public sealed class ObjectPoolingSystem : Manager<ObjectPoolingSystem>
    {
        [ProjectSettingSaveLoad]
        public sealed class Data
        {
            [JsonProperty] public static Dictionary<string, string> prefabList { get; set; } = new Dictionary<string, string>();
        }

        static ObjectList objectList { get; } = new ObjectList();
        class ObjectList
        {
            public List<string> objectKey = new List<string>();
            public List<(MonoBehaviour monoBehaviour, IObjectPooling objectPooling)> objectPooling = new List<(MonoBehaviour, IObjectPooling)>();
        }



        void Awake() => SingletonCheck(this);

        /// <summary>
        /// 오브젝트를 미리 생성합니다
        /// </summary>
        /// <param name="objectKey">미리 생성할 오브젝트 키</param>
        public static void ObjectAdvanceCreate(string objectKey)
        {
            MonoBehaviour monoBehaviour = Resources.Load<MonoBehaviour>(Data.prefabList[objectKey]);
            IObjectPooling objectPooling = monoBehaviour as IObjectPooling;
            if (objectPooling == null)
                return;

            ObjectAdd(objectKey, monoBehaviour);
        }

        /// <summary>
        /// 오브젝트를 리스트에 추가합니다
        /// </summary>
        /// <param name="objectKey">추가할 오브젝트의 키</param>
        /// <param name="monoBehaviour">추가할 오브젝트</param>
        public static void ObjectAdd(string objectKey, MonoBehaviour monoBehaviour)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(ObjectAdvanceCreate));
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(ObjectAdvanceCreate));
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(ObjectAdvanceCreate));
            if (instance == null)
                throw new NullScriptMethodException(nameof(ObjectPoolingSystem), nameof(ObjectRemove));
            if (monoBehaviour == null)
                throw new NullReferenceException(nameof(monoBehaviour));

            MonoBehaviour instantiate = Instantiate(monoBehaviour, instance.transform);
            IObjectPooling objectPooling = instantiate as IObjectPooling;
            if (objectPooling == null)
                return;

            objectPooling.objectKey = objectKey;
            ObjectRemove(objectKey, instantiate, objectPooling);
        }

        /// <summary>
        /// 오브젝트가 리스트에 있는지 감지합니다 (리소스 폴더에 있는 프리팹은 알아서 감지하고 생성하니, 이 함수를 쓸 필요가 없습니다)
        /// </summary>
        /// <param name="objectKey">감지할 오브젝트 키</param>
        /// <returns></returns>
        public static bool ObjectContains(string objectKey) => objectList.objectKey.Contains(objectKey);

        /// <summary>
        /// 오브젝트를 생성합니다
        /// </summary>
        /// <param name="objectKey">생성할 오브젝트 키</param>
        /// <param name="parent">생성할 오브젝트가 자식으로갈 오브젝트</param>
        /// <returns></returns>
        public static (MonoBehaviour monoBehaviour, IObjectPooling objectPooling) ObjectCreate(string objectKey, Transform parent = null, bool autoRefresh = true)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(ObjectCreate));
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(ObjectCreate));
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(ObjectCreate));
            if (instance == null)
                throw new NullScriptMethodException(nameof(ObjectPoolingSystem), nameof(ObjectCreate));

            if (objectList.objectKey.Contains(objectKey))
            {
                (MonoBehaviour monoBehaviour, IObjectPooling objectPooling) = objectList.objectPooling[objectList.objectKey.IndexOf(objectKey)];

                monoBehaviour.transform.SetParent(parent, false);
                monoBehaviour.gameObject.SetActive(true);

                objectPooling.objectKey = objectKey;
                
                {
                    int i = objectList.objectKey.IndexOf(objectKey);
                    objectList.objectKey.RemoveAt(i);
                    objectList.objectPooling.RemoveAt(i);
                }

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
                objectPooling.isActived = true;
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.

                if (autoRefresh)
                    RendererManager.Refresh(objectPooling.refreshableObjects, false);

                objectPooling.OnCreate();
                return (monoBehaviour, objectPooling);
            }
            else if (Data.prefabList.ContainsKey(objectKey))
            {
                GameObject gameObject = Resources.Load<GameObject>(Data.prefabList[objectKey]);
                if (gameObject == null)
                    return (null, null);

                IObjectPooling objectPooling = Instantiate(gameObject, parent).GetComponent<IObjectPooling>();
                if (objectPooling == null)
                    return (null, null);

                MonoBehaviour monoBehaviour = (MonoBehaviour)objectPooling;

                monoBehaviour.name = objectKey;
                objectPooling.objectKey = objectKey;

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
                objectPooling.isActived = true;
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.

                if (autoRefresh)
                    RendererManager.Refresh(objectPooling.refreshableObjects, false);

                objectPooling.OnCreate();
                return (monoBehaviour, objectPooling);
            }

            return (null, null);
        }

        /// <summary>
        /// 오브젝트를 삭제합니다
        /// </summary>
        /// <param name="objectKey">지울 오브젝트 키</param>
        /// <param name="objectPooling">지울 오브젝트</param>
        public static bool ObjectRemove(string objectKey, MonoBehaviour monoBehaviour, IObjectPooling objectPooling)
        {
            if (!ThreadManager.isMainThread)
                throw new NotMainThreadMethodException(nameof(ObjectRemove));
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException(nameof(ObjectRemove));
            if (!InitialLoadManager.isInitialLoadEnd)
                throw new NotInitialLoadEndMethodException(nameof(ObjectCreate));
            if (instance == null)
                throw new NullScriptMethodException(nameof(ObjectPoolingSystem), nameof(ObjectRemove));
            if (monoBehaviour == null)
                throw new NullReferenceException(nameof(monoBehaviour));
            if (objectPooling == null)
                throw new NullReferenceException(nameof(objectPooling));

            monoBehaviour.gameObject.SetActive(false);
            monoBehaviour.transform.SetParent(instance.transform);

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.
            objectPooling.isActived = false;
#pragma warning restore CS0618 // 형식 또는 멤버는 사용되지 않습니다.

            objectList.objectKey.Add(objectKey);
            objectList.objectPooling.Add((monoBehaviour, objectPooling));

            return true;
        }
    }
}