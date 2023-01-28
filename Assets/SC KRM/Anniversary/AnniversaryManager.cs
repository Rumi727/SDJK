using SCKRM.NTP;
using SCKRM.Renderer;
using SCKRM.UI.SideBar;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SCKRM
{
    [WikiDescription("특정한 날을 기념하기 위한 클래스 입니다 (예: 쿠루미쨩 생일)\n기념일이 되면 알림을 표시합니다")]
    [AddComponentMenu("SC KRM/Anniversary/Anniversary Manager")]
    public sealed class AnniversaryManager : ManagerBase<AnniversaryManager>
    {
        public static List<Anniversary> anniversaryList { get; } = new List<Anniversary>();

        [Awaken]
        static void Awaken() => BuiltinAnniversary();

        void Awake() => SingletonCheck(this);

        void Update()
        {
            //기념일
            //초기로딩이 끝나야 알림을 띄울수 있으니 조건을 걸어둡니다
            if (InitialLoadManager.isInitialLoadEnd)
            {
                for (int i = 0; i < anniversaryList.Count; i++)
                    anniversaryList[i].Execute();
            }
        }

        static void BuiltinAnniversary()
        {
            //학교생활!
            anniversaryList.Add(new Anniversary(dateTime =>
            {
                NameSpacePathReplacePair title = "sc-krm:notice.school_live.birthday.title";
                NameSpacePathReplacePair description = "sc-krm:notice.school_live.birthday.description";
                ReplaceOldNewPair replace = new ReplaceOldNewPair("%value%", (dateTime.Year - 2012).ToString());

                title.replace = new ReplaceOldNewPair[] { replace };
                description.replace = new ReplaceOldNewPair[] { replace };

                NoticeManager.Notice(title, description);
            }, 7, 1));

            //학교생활! 애니
            anniversaryList.Add(new Anniversary(dateTime =>
            {
                NameSpacePathReplacePair title = "sc-krm:notice.school_live_ani.birthday.title";
                NameSpacePathReplacePair description = "sc-krm:notice.school_live_ani.birthday.description";
                ReplaceOldNewPair replace = new ReplaceOldNewPair("%value%", (dateTime.Year - 2015).ToString());

                title.replace = new ReplaceOldNewPair[] { replace };
                description.replace = new ReplaceOldNewPair[] { replace };

                NoticeManager.Notice(title, description);
            }, 7, 9));

            //쿠루미쨩
            anniversaryList.Add(new Anniversary(dateTime =>
            {
                NoticeManager.Notice("notice.ebisuzawa_kurumi_chan.birthday.title", "notice.ebisuzawa_kurumi_chan.birthday.description");
            }, 8, 7));

            //유키
            anniversaryList.Add(new Anniversary(dateTime =>
            {
                NoticeManager.Notice("notice.takeya_yuki.birthday.title", "notice.takeya_yuki.birthday.description");
            }, 4, 5));

            //유리
            anniversaryList.Add(new Anniversary(dateTime =>
            {
                NoticeManager.Notice("notice.wakasa_yuri.birthday.title", "notice.wakasa_yuri.birthday.description");
            }, 10, 11));

            //미키
            anniversaryList.Add(new Anniversary(dateTime =>
            {
                NoticeManager.Notice("notice.naoki_miki.birthday.title", "notice.naoki_miki.birthday.description");
            }, 12, 10));

            //메구미
            anniversaryList.Add(new Anniversary(dateTime =>
            {
                NoticeManager.Notice("notice.sakura_megumi.birthday.title", "notice.sakura_megumi.birthday.description");
            }, 3, 10));

            //스크래치 네이버 카페
            anniversaryList.Add(new Anniversary(dateTime =>
            {
                NameSpacePathReplacePair title = "sc-krm:notice.onell0.birthday.title";
                NameSpacePathReplacePair description = "sc-krm:notice.onell0.birthday.description";
                ReplaceOldNewPair replace = new ReplaceOldNewPair("%value%", (dateTime.Year - 2010).ToString());

                title.replace = new ReplaceOldNewPair[] { replace };
                description.replace = new ReplaceOldNewPair[] { replace };

                NoticeManager.Notice(title, description);
            }, 2, 9));

            //설날
            anniversaryList.Add(new Anniversary(dateTime =>
            {
                NoticeManager.Notice("notice.korean_new_year.title", "notice.korean_new_year.description");
            }, 1, 1, true, new TimeSpan(9, 0, 0)));

            //만우절
            anniversaryList.Add(new Anniversary(dateTime =>
            {
                NoticeManager.Notice("notice.april_fools_day.title", "notice.april_fools_day.description");
            }, 4, 1));

            //마플님
            anniversaryList.Add(new Anniversary(dateTime =>
            {
                NoticeManager.Notice("notice.marple.birthday.title", "notice.marple.birthday.description");
            }, 4, 23));
        }
    }

    public class Anniversary
    {
        public Action<DateTime> action;

        public int month;
        public int day;

        public bool lunar;
        public bool isUtc;
        public TimeSpan utcOffset;

        public Anniversary(Action<DateTime> action, int month, int day, bool lunar = false)
        {
            this.action = action;

            this.month = month;
            this.day = day;

            this.lunar = lunar;

            isUtc = false;
            utcOffset = TimeSpan.Zero;

            lastYear = 0;
            lastMonth = 0;
            lastDay = 0;
        }

        public Anniversary(Action<DateTime> action, int month, int day, bool lunar, TimeSpan utcOffset)
        {
            this.action = action;

            this.month = month;
            this.day = day;

            this.lunar = lunar;

            isUtc = true;
            this.utcOffset = utcOffset;

            lastYear = 0;
            lastMonth = 0;
            lastDay = 0;
        }

        int lastYear;
        int lastMonth;
        int lastDay;
        public void Execute()
        {
            DateTime dateTime;
            if (isUtc)
                dateTime = NTPDateTime.utcNow + utcOffset;
            else
                dateTime = NTPDateTime.now;

            if (lunar)
                dateTime = dateTime.ToSolarDate();

            //최적화를 위해 년, 월, 일이 변경되어야 실행됩니다
            if (lastYear != dateTime.Year || lastMonth != dateTime.Month || lastDay != dateTime.Day)
            {
                if (dateTime.Month == month && dateTime.Day == day)
                    action?.Invoke(dateTime);

                lastYear = dateTime.Year;
                lastMonth = dateTime.Month;
                lastDay = dateTime.Day;
            }
        }
    }
}