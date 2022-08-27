using Discord;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SCKRM.Discord
{
    using Discord = global::Discord.Discord;
    [WikiDescription("디스코드를 관리하는 클래스 입니다")]
    public class DiscordManager : Manager<DiscordManager>
    {
        [WikiDescription("디스코드가 실행 중 인지에 대한 여부입니다")]
        public static bool discordIsRunning { get; private set; }

        [WikiDescription("디스코드 인스턴스를 가져옵니다")]
        public static Discord discord
        {
            get
            {
                if (!discordIsRunning)
                    throw new DiscordNotLoading();

                return _discord;
            }
        }
        static Discord _discord;

        [WikiDescription("디스코드에 접속한 유저를 가져옵니다")]
        public static User? currentUser { get; private set; }

        #region Manager
        public static AchievementManager achievementManager => discord.GetAchievementManager();
        public static ActivityManager activityManager => discord.GetActivityManager();
        public static ApplicationManager applicationManager => discord.GetApplicationManager();
        public static ImageManager imageManager => discord.GetImageManager();
        public static LobbyManager lobbyManager => discord.GetLobbyManager();
        public static NetworkManager networkManager => discord.GetNetworkManager();
        public static OverlayManager overlayManager => discord.GetOverlayManager();
        public static RelationshipManager relationshipManager => discord.GetRelationshipManager();
        public static StorageManager storageManager => discord.GetStorageManager();
        public static StoreManager storeManager => discord.GetStoreManager();
        public static UserManager userManager => discord.GetUserManager();
        public static VoiceManager voiceManager => discord.GetVoiceManager();
        #endregion

        [WikiDescription("입력된 애플리케이션 아이디를 가져옵니다")]
        public static long applicationId
        {
            get
            {
                if (!discordIsRunning)
                    throw new DiscordNotLoading();

                return instance._applicationId;
            }
        }
        [SerializeField] long _applicationId;

        void Awake()
        {
            if (SingletonCheck(this))
            {
                try
                {
                    Initialization();
                }
                catch (ResultException)
                {
                    Debug.Log("Discord is not running");
                }
            }
        }

        void Update()
        {
            try
            {
                if (discordIsRunning)
                    discord.RunCallbacks();
                else
                    Initialization();
            }
            catch (ResultException)
            {
                if (discordIsRunning)
                    Debug.Log("Discord is not running");

                _discord = null;
                currentUser = null;
                discordIsRunning = false;
            }
        }

        [WikiDescription("디스코드 API를 초기화합니다")]
        public static void Initialization()
        {
            _discord = new Discord(instance._applicationId, (ulong)CreateFlags.NoRequireDiscord);
            discordIsRunning = true;

            userManager.OnCurrentUserUpdate += () =>
            {
                currentUser = userManager.GetCurrentUser();

                Debug.Log("Discord api loading...");
                Debug.Log("Discord user id: " + currentUser.Value.Id);
                Debug.Log("Discord user username: " + currentUser.Value.Username);

                //디스코드 API를 정상적으로 불러왔는지 체크하고, 실패하면 심각한 오류가 발생할 수 있으니, 프로그램을 종료합니다
                if (!DiscordCheck(Kernel.saveDataPath, currentUser.Value.Id))
                    InitialLoadManager.ApplicationForceQuit(nameof(DiscordManager), "Discord check failed");
            };

            Debug.Log("Discord is running");
        }

        [DllImport("discord_check.dll")]
        [WikiDescription("디스코드 API를 정상적으로 불러왔는지 체크하고,\n실패하면 심각한 오류가 발생할 수 있으니, 프로그램을 종료합니다")]
        public static extern bool DiscordCheck(string saveDataPath, long userId);
    }

    public class DiscordNotLoading : Exception
    {
        public DiscordNotLoading() : base("디스코드가 실행되지 않습니다!\nDiscord is not running!")
        {

        }
    }
}
