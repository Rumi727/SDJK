using SCKRM.Language;
using SCKRM.Renderer;
using SCKRM.Resource;
using SCKRM.Threads;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SCKRM.UI.SideBar
{
    [AddComponentMenu("SC KRM/UI/Kerenl/Side Bar/Running Task Info")]
    [RequireComponent(typeof(RectTransform))]
    public sealed class RunningTaskInfo : UIObjectPooling, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] CustomTextMeshProRenderer _nameText;
        public CustomTextMeshProRenderer nameText => _nameText;

        [SerializeField] CustomTextMeshProRenderer _infoText;
        public CustomTextMeshProRenderer infoText => _infoText;

        [SerializeField] ProgressBar _progressBar;
        public ProgressBar progressBar => _progressBar;



        [SerializeField] CanvasGroup _removeButtonCanvasGroup;
        public CanvasGroup removeButtonCanvasGroup => _removeButtonCanvasGroup;



        public AsyncTask asyncTask { get; set; }
        public int asyncTaskIndex { get; set; }

        public override void OnCreate()
        {
            base.OnCreate();

            LanguageManager.currentLanguageChange += InfoLoad;
            ThreadManager.threadChange += InfoLoad;
        }

        public void InfoLoad()
        {
            if (asyncTask != null)
            {
                nameText.nameSpacePathReplacePair = asyncTask.name;
                infoText.nameSpacePathReplacePair = asyncTask.info;

                nameText.Refresh();
                infoText.Refresh();
            }
        }

        [System.NonSerialized] string tempName = "";
        [System.NonSerialized] string tempInfo = "";
        [System.NonSerialized] bool pointer = false;
        [System.NonSerialized] bool lastLoop = true;
        void Update()
        {
            if (asyncTask == null || asyncTaskIndex >= AsyncTaskManager.asyncTasks.Count)
            {
                progressBar.allowNoResponse = false;
                progressBar.progress = 1;
                progressBar.maxProgress = 1;

                if (progressBar.fillShow.anchorMax.x >= 0.99f || lastLoop)
                    Remove();

                return;
            }

            if (tempName != asyncTask.name || tempInfo != asyncTask.info)
                InfoLoad();

            if (!asyncTask.cantCancel)
            {
                if (pointer || removeButtonCanvasGroup.gameObject == EventSystem.current.currentSelectedGameObject)
                    removeButtonCanvasGroup.alpha = removeButtonCanvasGroup.alpha.MoveTowards(1, 0.2f * Kernel.fpsUnscaledDeltaTime);
                else
                    removeButtonCanvasGroup.alpha = removeButtonCanvasGroup.alpha.MoveTowards(0, 0.2f * Kernel.fpsUnscaledDeltaTime);
            }
            else
            {
                removeButtonCanvasGroup.alpha = 0;
                removeButtonCanvasGroup.interactable = false;
            }

            lastLoop = asyncTask.loop;

            if (!asyncTask.loop)
            {
                if (!progressBar.gameObject.activeSelf)
                    progressBar.gameObject.SetActive(true);

                progressBar.progress = asyncTask.progress;
                progressBar.maxProgress = asyncTask.maxProgress;
            }
            else if (progressBar.gameObject.activeSelf)
                progressBar.gameObject.SetActive(false);
        }

        public void Cancel() => asyncTask.Remove();

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            rectTransform.sizeDelta = new Vector2(430, 19);
            asyncTask = null;

            nameText.nameSpacePathReplacePair = "";
            infoText.nameSpacePathReplacePair = "";

            nameText.Refresh();
            infoText.Refresh();

            progressBar.Initialize();

            progressBar.gameObject.SetActive(true);
            progressBar.enabled = true;

            LanguageManager.currentLanguageChange -= InfoLoad;
            ThreadManager.threadChange -= InfoLoad;

            return true;
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => pointer = true;

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => pointer = false;
    }
}