using System;
using UnityEngine;

namespace SCKRM.Object
{
    [WikiDescription("오브젝트 풀링 인터페이스")]
    public interface IObjectPooling : IRemoveable
    {
        string objectKey { get; set; }
        bool isActived { get; [Obsolete("It is managed by the ObjectPoolingSystem class. Please do not touch it.")] internal set; }

        IRefreshable[] refreshableObjects { get; }

        void OnCreate();

        bool IsDestroyed();

        [WikiDescription("오브젝트를 생성할때의 기본 스크립트")]
        public static void OnCreateDefault(Transform transform, IObjectPooling objectPooling)
        {
            transform.gameObject.name = objectPooling.objectKey;

            transform.localPosition = Vector3.zero;

            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        [WikiDescription("오브젝트를 삭제할때의 기본 스크립트")]
        public static bool RemoveDefault(MonoBehaviour monoBehaviour, IObjectPooling objectPooling)
        {
            if (!objectPooling.isActived)
                return false;

            ObjectPoolingSystem.ObjectRemove(objectPooling.objectKey, monoBehaviour, objectPooling);
            monoBehaviour.name = objectPooling.objectKey;

            monoBehaviour.transform.localPosition = Vector3.zero;

            monoBehaviour.transform.localEulerAngles = Vector3.zero;
            monoBehaviour.transform.localScale = Vector3.one;

            monoBehaviour.StopAllCoroutines();
            return true;
        }

        [WikiIgnore]
        public static bool RemoveDefault(UI.UI ui, IObjectPooling objectPooling)
        {
            if (!objectPooling.isActived)
                return false;

            ObjectPoolingSystem.ObjectRemove(objectPooling.objectKey, ui, objectPooling);
            if (!Kernel.isPlaying)
                return false;

            ui.name = objectPooling.objectKey;

            ui.rectTransform.anchoredPosition = Vector3.zero;

            ui.rectTransform.localEulerAngles = Vector3.zero;
            ui.rectTransform.localScale = Vector3.one;

            ui.StopAllCoroutines();
            return true;
        }
    }

    [WikiDescription("오브젝트 풀링으로 생성된 오브젝트를 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Object/Object Pooling")]
    public class ObjectPoolingBase : MonoBehaviour, IObjectPooling
    {
        [WikiDescription("오브젝트 키")] public string objectKey { get; set; }

        [WikiDescription("삭제 여부")] public bool isRemoved => !isActived;

        [WikiDescription("활성화 여부")] public bool isActived { get; private set; }
        bool IObjectPooling.isActived { get => isActived; set => isActived = value; }



        IRefreshable[] _refreshableObjects;
        [WikiDescription("새로고침 가능한 오브젝트들을 가져옵니다")] public IRefreshable[] refreshableObjects => _refreshableObjects = this.GetComponentsInChildrenFieldSave(_refreshableObjects, true);



        /// <summary>
        /// Please put base.OnCreate() when overriding
        /// </summary>
        public virtual void OnCreate() => IObjectPooling.OnCreateDefault(transform, this);

        /// <summary>
        /// Please put base.Remove() when overriding
        /// </summary>
        [WikiDescription("오브젝트 삭제")]
        public virtual bool Remove() => IObjectPooling.RemoveDefault(this, this);

        public bool IsDestroyed() => this == null;
    }
}
