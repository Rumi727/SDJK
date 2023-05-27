using Cysharp.Threading.Tasks;
using K4.Threading;
using SCKRM.Input;
using SCKRM.Object;
using SCKRM.Renderer;
using SCKRM.Threads;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SCKRM.UI.SideBar
{
    [AddComponentMenu("SC KRM/UI/Kerenl/Side Bar/Notice Manager")]
    public sealed class NoticeManager : UIManagerBase<NoticeManager>
    {
        [SerializeField] Transform _noticeListTransform; public Transform noticeListTransform => _noticeListTransform;
        [SerializeField] SideBarAni noticeBar;
        [SerializeField] UnityEvent _noticeAdd;

        public static List<Notice> noticeList { get; } = new List<Notice>();
        public static event Action noticeAdd = () => { };

        protected override void Awake()
        {
            if (SingletonCheck(this))
                noticeAdd += _noticeAdd.Invoke;
        }

        void Update()
        {
            if (InitialLoadManager.isInitialLoadEnd)
            {
                if (noticeBar.isShow && noticeList.Count > 0 && InputManager.GetKey("notice_manager.notice_remove", InputType.Down, InputManager.inputLockDenyAll))
                    LastRemove();

                if (noticeBar.isShow && noticeList.Count > 0 && InputManager.GetKey("notice_manager.notice_clear_all", InputType.Down, InputManager.inputLockDenyAll))
                    Clear();
            }
        }

        public static void FirstRemove()
        {
            if (noticeList.Count >= 0)
            {
                noticeList[0].Remove();
                noticeList.RemoveAt(0);
            }
        }

        public static void LastRemove()
        {
            if (noticeList.Count > 0)
            {
                noticeList[noticeList.Count - 1].Remove();
                noticeList.RemoveAt(noticeList.Count - 1);
            }
        }

        public static void Clear()
        {
            for (int i = 0; i < noticeList.Count; i++)
                noticeList[i].Remove();

            noticeList.Clear();
        }

        public void AllAsyncTaskCancel() => AsyncTaskManager.AllAsyncTaskCancel();

        public static void Notice(NameSpacePathReplacePair name, NameSpacePathReplacePair info) => notice(name, info, Type.none).Forget();
        public static void Notice(NameSpacePathReplacePair name, NameSpacePathReplacePair info, Type type) => notice(name, info, type).Forget();

        static async UniTaskVoid notice(NameSpacePathReplacePair name, NameSpacePathReplacePair info, Type type)
        {
            if (!ThreadManager.isMainThread)
            {
                K4UnityThreadDispatcher.Execute(() => Notice(name, info, type)).Forget();
                return;
            }
            if (!Kernel.isPlaying)
                throw new NotPlayModeMethodException();

            await UniTask.WaitUntil(() => InitialLoadManager.isInitialLoadEnd, PlayerLoopTiming.Update, AsyncTaskManager.cancelToken);

            if (noticeList.Count >= 10)
                FirstRemove();

            Notice notice = (Notice)ObjectPoolingSystem.ObjectCreate("notice_manager.notice", instance.noticeListTransform).monoBehaviour;
            notice.transform.SetAsFirstSibling();
            notice.nameText.nameSpacePathReplacePair = name;
            notice.infoText.nameSpacePathReplacePair = info;

            notice.nameText.Refresh();
            notice.infoText.Refresh();

            noticeList.Add(notice);

            if (type != Type.none)
            {
                notice.icon.gameObject.SetActive(true);

                notice.icon.nameSpaceIndexTypePathPair = "sc-krm:0:gui/notice_icon/" + type;
                notice.icon.Refresh();

                notice.childSizeFitter.min = 70;
                notice.verticalLayout.padding.left = 70;
            }

            noticeAdd?.Invoke();
        }

        public enum Type
        {
            none,
            warning,
            error
        }
    }
}