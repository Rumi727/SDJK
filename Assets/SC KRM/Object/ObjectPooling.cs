using System;
using UnityEngine;

namespace SCKRM.Object
{
    public interface IObjectPooling : IRemoveable
    {
        string objectKey { get; set; }
        bool isActived { get; [Obsolete("It is managed by the ObjectPoolingSystem class. Please do not touch it.")] internal set; }

        IRefreshable[] refreshableObjects { get; }

        void OnCreate();

        public static void OnCreateDefault(Transform transform, IObjectPooling objectPooling)
        {
            transform.gameObject.name = objectPooling.objectKey;

            transform.localPosition = Vector3.zero;

            transform.localEulerAngles = Vector3.zero;
            transform.localScale = Vector3.one;
        }

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

        public static bool RemoveDefault(UI.UI ui, IObjectPooling objectPooling)
        {
            if (!objectPooling.isActived)
                return false;

            ObjectPoolingSystem.ObjectRemove(objectPooling.objectKey, ui, objectPooling);
            ui.name = objectPooling.objectKey;

            ui.rectTransform.anchoredPosition = Vector3.zero;

            ui.rectTransform.localEulerAngles = Vector3.zero;
            ui.rectTransform.localScale = Vector3.one;

            ui.StopAllCoroutines();
            return true;
        }
    }

    [AddComponentMenu("SC KRM/Object/Object Pooling")]
    public class ObjectPooling : MonoBehaviour, IObjectPooling
    {
        public string objectKey { get; set; }

        public bool isRemoved => !isActived;

        public bool isActived { get; private set; }
        bool IObjectPooling.isActived { get => isActived; set => isActived = value; }



        IRefreshable[] _refreshableObjects;
        public IRefreshable[] refreshableObjects => _refreshableObjects = this.GetComponentsInChildrenFieldSave(_refreshableObjects, true);



        /// <summary>
        /// Please put base.OnCreate() when overriding
        /// </summary>
        public virtual void OnCreate() => IObjectPooling.OnCreateDefault(transform, this);

        /// <summary>
        /// Please put base.Remove() when overriding
        /// </summary>
        public virtual bool Remove() => IObjectPooling.RemoveDefault(this, this);
    }
}
