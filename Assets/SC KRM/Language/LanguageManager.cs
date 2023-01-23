using Newtonsoft.Json;
using SCKRM.Json;
using SCKRM.Resource;
using SCKRM.SaveLoad;
using System;
using System.Collections.Generic;
using System.IO;

namespace SCKRM.Language
{
    [WikiDescription("언어를 관리하는 클래스 입니다")]
    public static class LanguageManager
    {
        public struct Language
        {
            public Language(string language, string languageName, string languageRegion)
            {
                this.language = language;
                this.languageName = languageName;
                this.languageRegion = languageRegion;
            }

            public string language { get; }

            public string languageName { get; }
            public string languageRegion { get; }
        }

        [GeneralSaveLoad]
        public sealed class SaveData
        {
            [JsonProperty] public static string currentLanguage { get; set; } = "en_us";
        }


        [WikiDescription("언어를 변경할때 발생하는 이벤트 입니다")] public static event Action currentLanguageChange;
        [WikiDescription("언어 변경 이벤트를 실행합니다")] public static void LanguageChangeEventInvoke() => currentLanguageChange?.Invoke();

        /// <summary>
        /// 리소스팩에서 언어 파일을 가져온뒤, 키 값으로 텍스트를 찾고 반환합니다
        /// After importing the language file from the resource pack, it finds and returns the text as a key value.
        /// </summary>
        /// <param name="key">
        /// 키
        /// Key</param>
        /// <param name="nameSpace">
        /// 네임스페이스
        /// Name Space
        /// </param>
        /// <param name="language">
        /// 언어
        /// </param>
        /// <returns></returns>
        [WikiDescription(
@"리소스팩에서 언어 파일을 가져온뒤, 키 값으로 텍스트를 찾고 반환합니다
After importing the language file from the resource pack, it finds and returns the text as a key value."
)]
        public static string LanguageLoad(string key, string nameSpace = "", string language = "")
        {
            if (key == null)
                key = "";
            if (language == null)
                language = "";

            if (nameSpace == "")
                nameSpace = ResourceManager.defaultNameSpace;
            if (language == "")
                language = SaveData.currentLanguage;

            string value = JsonManager.JsonReadDictionary<string, string>(key, PathUtility.Combine(ResourceManager.languagePath, language) + ".json", nameSpace).ConstEnvironmentVariable();
            if (value == default)
                return key;

            return value;
        }

        /// <summary>
        /// 언어 리스트를 가져옵니다
        /// Get a list of languages
        /// </summary>
        /// <returns></returns>
        [WikiDescription("언어 리스트를 가져옵니다\nGet a list of languages")]
        public static Language[] GetLanguages()
        {
            List<Language> languages = new List<Language>();
            List<string> languageList = new List<string>();
            for (int packIndex = 0; packIndex < ResourceManager.SaveData.resourcePacks.Count; packIndex++)
            {
                string resourcePackPath = ResourceManager.SaveData.resourcePacks[packIndex];

                for (int nameSpaceIndex = 0; nameSpaceIndex < ResourceManager.nameSpaces.Count; nameSpaceIndex++)
                {
                    string nameSpace = ResourceManager.nameSpaces[nameSpaceIndex];
                    if (Directory.Exists(PathUtility.Combine(resourcePackPath, ResourceManager.languagePath).Replace("%NameSpace%", nameSpace)))
                    {
                        string[] directorys = Directory.GetFiles(PathUtility.Combine(resourcePackPath, ResourceManager.languagePath).Replace("%NameSpace%", nameSpace), "*.json");

                        for (int languageIndex = 0; languageIndex < directorys.Length; languageIndex++)
                        {
                            string path = directorys[languageIndex].Replace("\\", "/");
                            string language = path.Substring(path.LastIndexOf("/") + 1, path.LastIndexOf(".") - path.LastIndexOf("/") - 1);

                            if (languageList.Contains(language))
                                continue;

                            string languageName = "";
                            string languageRegion = "";

                            Dictionary<string, string> languageFile = JsonManager.JsonRead<Dictionary<string, string>>(path, true);

                            if (languageFile.ContainsKey("language.name"))
                                languageName = languageFile["language.name"];
                            if (languageFile.ContainsKey("language.region"))
                                languageRegion = languageFile["language.region"];

                            languages.Add(new Language(language, languageName, languageRegion));
                        }
                    }
                }
            }

            return languages.ToArray();
        }
    }
}