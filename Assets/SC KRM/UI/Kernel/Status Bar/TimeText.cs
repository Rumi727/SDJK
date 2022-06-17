using SCKRM.Language;
using SCKRM.Resource;
using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace SCKRM.UI.StatusBar
{
    [AddComponentMenu("SC KRM/UI/Kerenl/Status Bar/Time Text")]
    [RequireComponent(typeof(TMP_Text))]
    public sealed class TimeText : UI
    {
        [SerializeField, HideInInspector] TMP_Text _text;
        public TMP_Text text => _text = this.GetComponentFieldSave(_text);

        static string am = "";
        static string pm = "";
        static int tempMinute = -1;
        static int tempSecond = -1;

        static DateTimeFormatInfo dateTimeFormatInfo = new DateTimeFormatInfo();

        static bool tempTwentyFourHourSystem = false;
        static bool tempToggleSeconds = false;

        protected override void OnEnable()
        {
            InitialLoadManager.initialLoadEnd += LanguageChange;
            LanguageManager.currentLanguageChange += LanguageChange;
        }

        protected override void OnDestroy()
        {
            InitialLoadManager.initialLoadEnd -= LanguageChange;
            LanguageManager.currentLanguageChange -= LanguageChange;
        }

        void Update()
        {
            DateTime dateTime = DateTime.Now;
            if ((dateTime.Second != tempSecond && StatusBarManager.SaveData.toggleSeconds) || dateTime.Minute != tempMinute || StatusBarManager.SaveData.twentyFourHourSystem != tempTwentyFourHourSystem || StatusBarManager.SaveData.toggleSeconds != tempToggleSeconds)
            {
                dateTimeFormatInfo.AMDesignator = am;
                dateTimeFormatInfo.PMDesignator = pm;

                string time = "tt h:mm\nyyyy-MM-dd";

                if (StatusBarManager.SaveData.twentyFourHourSystem)
                    time = time.Replace("h", "H").Replace("tt", "");

                if (StatusBarManager.SaveData.toggleSeconds)
                    time = time.Replace("mm", "mm:ss");

                text.text = DateTime.Now.ToString(time, dateTimeFormatInfo);

                tempSecond = dateTime.Second;
                tempMinute = dateTime.Minute;
                tempTwentyFourHourSystem = StatusBarManager.SaveData.twentyFourHourSystem;
                tempToggleSeconds = StatusBarManager.SaveData.toggleSeconds;
            }
        }

        void LanguageChange()
        {
            am = ResourceManager.SearchLanguage("gui.am");
            pm = ResourceManager.SearchLanguage("gui.pm");
            tempMinute = -1;
        }
    }
}