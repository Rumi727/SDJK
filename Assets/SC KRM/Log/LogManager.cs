using System.Collections.Concurrent;
using UnityEngine;
using SCKRM.UI.SideBar;
using SCKRM.Renderer;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

namespace SCKRM.Log
{
    [WikiDescription("로그를 관리하는 클래스 입니다")]
    [AddComponentMenu("SC KRM/Log/Log Manager")]
    public sealed class LogManager : ManagerBase<LogManager>
    {
        [SerializeField] Toggle errorOrWarningShowToggle;

        public static bool errorOrWarningShow
        {
            get
            {
                if (instance != null && instance.errorOrWarningShowToggle != null)
                    _errorOrWarningShow = instance.errorOrWarningShowToggle.isOn;

                return _errorOrWarningShow;
            }

            set
            {
                if (invokeIgnore)
                    return;

                _errorOrWarningShow = value;

                invokeIgnore = true;

                if (instance != null && instance.errorOrWarningShowToggle != null)
                    instance.errorOrWarningShowToggle.isOn = value;

                invokeIgnore = false;
            }
        }
        static bool _errorOrWarningShow = false;
        static bool invokeIgnore = false;



        ConcurrentQueue<Log> logs = new ConcurrentQueue<Log>();
        void OnEnable()
        {
            if (SingletonCheck(this))
                Application.logMessageReceivedThreaded += log;
        }

        void OnDisable() => Application.logMessageReceivedThreaded -= log;

        void Update()
        {
            if (InitialLoadManager.isInitialLoadEnd)
            {
                while (logs.TryDequeue(out Log log))
                {
                    if (log.stackTrace != "")
                        NoticeManager.Notice(new NameSpacePathReplacePair(log.name), new NameSpacePathReplacePair(log.info + "\n\n" + log.stackTrace), log.type);
                    else
                        NoticeManager.Notice(new NameSpacePathReplacePair(log.name), new NameSpacePathReplacePair(log.info), log.type);
                }
            }
        }

        void log(string condition, string stackTrace, LogType type)
        {
            if (!errorOrWarningShow)
                return;

            string name = type.ToString();
            string info = condition;
            Log? log = null;

            if (type == LogType.Exception)
            {
                if (condition.Contains(":"))
                {
                    name = condition.Remove(condition.IndexOf(':'));
                    info = condition.Substring(condition.IndexOf(':') + 2);
                }

                log = new Log(name, info, stackTrace, NoticeManager.Type.error);
            }
            else if (type == LogType.Error || type == LogType.Assert)
                log = new Log(name, info, stackTrace, NoticeManager.Type.error);
            else if (type == LogType.Warning)
                log = new Log(name, info, stackTrace, NoticeManager.Type.warning);
            /*else
                log = new Log(name, info, stackTrace, NoticeManager.Type.none);*/

            if (log != null)
                logs.Enqueue(log.Value);
        }

        public struct Log
        {
            public Log(string name, string info, string stackTrace, NoticeManager.Type type)
            {
                this.name = name;
                this.info = info;
                this.stackTrace = stackTrace;
                this.type = type;
            }

            public string name { get; }
            public string info { get; }
            public string stackTrace { get; }
            public NoticeManager.Type type { get; }
        }
    }
}