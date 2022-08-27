using SCKRM.Renderer;
using SCKRM.UI.SideBar;
using System;
using UnityEngine;

namespace SCKRM
{
    [WikiDescription("특정한 날을 기념하기 위한 클래스 입니다 (예: 쿠루미쨩 생일)\n기념일이 되면 알림을 표시합니다")]
    [AddComponentMenu("SC KRM/Anniversary/Anniversary Manager")]
    public sealed class AnniversaryManager : Manager<AnniversaryManager>
    {
        void Awake() => SingletonCheck(this);

        static int tempYear;
        static int tempMonth;
        static int tempDay;
        void Update()
        {
            //기념일
            //초기로딩이 끝나야 알림을 띄울수 있으니 조건을 걸어둡니다
            //최적화를 위해 년, 월, 일이 변경되어야 실행됩니다
            DateTime dateTime = DateTime.Now;
            if (InitialLoadManager.isInitialLoadEnd && (tempYear != dateTime.Year || tempMonth != dateTime.Month || tempDay != dateTime.Day))
            {
                //음력 날짜를 정합니다
                DateTime dateTimeLunar = dateTime.ToLunarDate();

                if (dateTime.Month == 7 && dateTime.Day == 1) //7월이라면...
                {
                    NameSpacePathReplacePair title = "sc-krm:notice.school_live.birthday.title";
                    NameSpacePathReplacePair description = "sc-krm:notice.school_live.birthday.description";
                    ReplaceOldNewPair replace = new ReplaceOldNewPair("%value%", (dateTime.Year - 2012).ToString());

                    title.replace = new ReplaceOldNewPair[] { replace };
                    description.replace = new ReplaceOldNewPair[] { replace };

                    NoticeManager.Notice(title, description);
                }
                else if (dateTime.Month == 7 && dateTime.Day == 9) //7월 9일이라면...
                {
                    NameSpacePathReplacePair title = "sc-krm:notice.school_live_ani.birthday.title";
                    NameSpacePathReplacePair description = "sc-krm:notice.school_live_ani.birthday.description";
                    ReplaceOldNewPair replace = new ReplaceOldNewPair("%value%", (dateTime.Year - 2015).ToString());

                    title.replace = new ReplaceOldNewPair[] { replace };
                    description.replace = new ReplaceOldNewPair[] { replace };

                    NoticeManager.Notice(title, description);
                }
                else if (dateTime.Month == 8 && dateTime.Day == 7) //8월 7일이라면...
                    NoticeManager.Notice("notice.ebisuzawa_kurumi_chan.birthday.title", "notice.ebisuzawa_kurumi_chan.birthday.description");
                else if (dateTime.Month == 4 && dateTime.Day == 5) //4월 5일이라면...
                    NoticeManager.Notice("notice.takeya_yuki.birthday.title", "notice.takeya_yuki.birthday.description");
                else if (dateTime.Month == 10 && dateTime.Day == 11) //10월 11일이라면...
                    NoticeManager.Notice("notice.wakasa_yuri.birthday.title", "notice.wakasa_yuri.birthday.description");
                else if (dateTime.Month == 12 && dateTime.Day == 10) //12월 10일이라면...
                    NoticeManager.Notice("notice.naoki_miki.birthday.title", "notice.naoki_miki.birthday.description");
                else if (dateTime.Month == 3 && dateTime.Day == 10) //3월 10일이라면...
                    NoticeManager.Notice("notice.sakura_megumi.birthday.title", "notice.sakura_megumi.birthday.description");
                else if (dateTime.Month == 2 && dateTime.Day == 9) //2월 9일이라면...
                {
                    NameSpacePathReplacePair title = "sc-krm:notice.onell0.birthday.title";
                    NameSpacePathReplacePair description = "sc-krm:notice.onell0.birthday.description";
                    ReplaceOldNewPair replace = new ReplaceOldNewPair("%value%", (dateTime.Year - 2010).ToString());

                    title.replace = new ReplaceOldNewPair[] { replace };
                    description.replace = new ReplaceOldNewPair[] { replace };

                    NoticeManager.Notice(title, description);
                }
                else if (dateTimeLunar.Month == 1 && dateTimeLunar.Day == 1) //음력으로 1월 1일이라면...
                    NoticeManager.Notice("notice.korean_new_year.title", "notice.korean_new_year.description");
                else if (dateTime.Month == 4 && dateTimeLunar.Day == 1) //4월 1일이라면...
                    NoticeManager.Notice("notice.april_fools_day.title", "notice.april_fools_day.description");
                else if (dateTime.Month == 4 && dateTimeLunar.Day == 23) //4월 23일이라면...
                    NoticeManager.Notice("notice.marple.birthday.title", "notice.marple.birthday.description");

                tempYear = dateTime.Year;
                tempMonth = dateTime.Month;
                tempDay = dateTime.Day;
            }
        }
    }
}