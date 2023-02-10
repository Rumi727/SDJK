using SCKRM.Language;
using SCKRM.NTP;
using SCKRM.Resource;
using System;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace SCKRM.UI.StatusBar
{
    [AddComponentMenu("SC KRM/UI/Kerenl/Status Bar/Time Text")]
    [RequireComponent(typeof(TMP_Text))]
    public sealed class TimeText : UIBase
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

#pragma warning disable IDE0051 // 사용되지 않는 private 멤버 제거
        [Starten]
        static void Starten()
        {
            LanguageChange();
            LanguageManager.currentLanguageChange += LanguageChange;
        }
#pragma warning restore IDE0051 // 사용되지 않는 private 멤버 제거

        void Update()
        {
            DateTime dateTime = NTPDateTime.now;
            if ((dateTime.Second != tempSecond && StatusBarManager.SaveData.toggleSeconds) || dateTime.Minute != tempMinute || StatusBarManager.SaveData.twentyFourHourSystem != tempTwentyFourHourSystem || StatusBarManager.SaveData.toggleSeconds != tempToggleSeconds)
            {
                dateTimeFormatInfo.AMDesignator = am;
                dateTimeFormatInfo.PMDesignator = pm;

                string time = "tt h:mm\nyyyy-MM-dd";

                if (StatusBarManager.SaveData.twentyFourHourSystem)
                    time = time.Replace("h", "H").Replace("tt", "");

                if (StatusBarManager.SaveData.toggleSeconds)
                    time = time.Replace("mm", "mm:ss");

                text.text = dateTime.ToString(time, dateTimeFormatInfo);

                tempSecond = dateTime.Second;
                tempMinute = dateTime.Minute;
                tempTwentyFourHourSystem = StatusBarManager.SaveData.twentyFourHourSystem;
                tempToggleSeconds = StatusBarManager.SaveData.toggleSeconds;
            }
        }

        static void LanguageChange()
        {
            am = ResourceManager.SearchLanguage("gui.am");
            pm = ResourceManager.SearchLanguage("gui.pm");
            tempMinute = -1;
        }
    }
}