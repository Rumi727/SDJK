using SCKRM.Renderer;
using SCKRM.UI;
using System.Collections.Generic;
using UnityEngine;

namespace SCKRM.Language.UI
{
    [WikiDescription("언어 리스트를 표시하기 위한 클래스 입니다")]
    [AddComponentMenu("SC KRM/Language/UI/Language List")]
    public sealed class LanguageList : SCKRM.UI.UI
    {
        [SerializeField] Dropdown dropdown;

        public void LanguageRefresh()
        {
            LanguageManager.LanguageChangeEventInvoke();
            RendererManager.AllTextRerender(true);
        }

        protected override void Awake()
        {
            InitialLoadManager.initialLoadEnd += ListRefresh;
            Kernel.allRefreshEnd += ListRefresh;
        }

        protected override void OnDestroy()
        {
            InitialLoadManager.initialLoadEnd -= ListRefresh;
            Kernel.allRefreshEnd -= ListRefresh;
        }

        public void ListRefresh()
        {
            LanguageManager.Language[] languages = LanguageManager.GetLanguages();
            List<string> options = new List<string>();
            List<string> customLabel = new List<string>();

            for (int i = 0; i < languages.Length; i++)
            {
                LanguageManager.Language language = languages[i];

                if (!options.Contains(language.language))
                {
                    options.Add(language.language);
                    customLabel.Add($"{language.languageName} ({language.languageRegion})");
                }
            }

            dropdown.options = options.ToArray();
            dropdown.customLabel = customLabel.ToArray();
        }
    }
}