using Discord;
using SCKRM.Discord;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SCKRM.Discord
{
    using Discord = global::Discord.Discord;
    [WikiDescription("디스코드를 관리하는 클래스 입니다")]
    public class DiscordManager : ManagerBase<DiscordManager>
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



        [WikiDescription("기본 활동입니다")]
        public static BasicActivity basicActivity
        {
            get
            {
                if (!discordIsRunning)
                    throw new DiscordNotLoading();

                return instance._basicActivity;
            }
        }
        [SerializeField] BasicActivity _basicActivity;



        void Awake()
        {
            if (SingletonCheck(this))
            {
                try
                {
                    Initialization();

                    Kernel.shutdownEvent += () =>
                    {
                        DiscordAPIDispose();
                        return true;
                    };
                }
                catch
                {
                    Debug.Log("Discord is not running");
                }
            }
        }

        float timer = 0;
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

            if (timer >= 4)
            {
                if (updateAction != null)
                {
                    updateAction?.Invoke();
                    updateAction = null;

                    timer = 0;
                }
            }
            else
                timer += Kernel.unscaledDeltaTime;
        }



        /*static Texture2D lastLargeImage = null;
        static string lastLargeImageBase64;
        static Texture2D lastSmallImage = null;
        static string lastSmallImageBase64;*/

        static Action updateAction;
        public static void UpdateActivity(string details = null, string state = null, string largeImage = null, string largeText = null, string smallImage = null, string smallText = null, long? startTime = null, long? endTime = null)
        {
            if (!discordIsRunning)
                return;

            /*if (largeImage == null) largeImage = basicActivity.largeImage;
            if (smallImage == null) smallImage = basicActivity.smallImage;

            string largeImageBase64 = lastLargeImageBase64;
            string smallImageBase64 = lastSmallImageBase64;

            if (lastLargeImage != largeImage)
            {
                if (largeImage != null)
                    largeImageBase64 = "https://cdn.discordapp.com/data:image/jpeg;base64," + Convert.ToBase64String(largeImage.EncodeToPNG());
                else
                    largeImageBase64 = "";

                lastLargeImage = largeImage;
                lastLargeImageBase64 = largeImageBase64;
            }

            if (lastSmallImage != smallImage)
            {
                if (smallImage != null)
                    smallImageBase64 = "https://cdn.discordapp.com/data:image/png;base64," + Convert.ToBase64String(smallImage.EncodeToPNG());
                else
                    smallImageBase64 = "";

                lastSmallImage = smallImage;
                lastSmallImageBase64 = smallImageBase64;
            }*/

            UpdateActivity(new Activity()
            {
                Details = details ?? basicActivity.details,
                State = state ?? basicActivity.state,
                Assets = new ActivityAssets()
                {
                    LargeImage = largeImage ?? basicActivity.largeImage,
                    LargeText = largeText ?? basicActivity.largeText,
                    SmallImage = smallImage ?? basicActivity.smallImage,
                    SmallText = smallText ?? basicActivity.smallText
                },
                Timestamps = new ActivityTimestamps()
                {
                    Start = startTime ?? basicActivity.startTime,
                    End = endTime ?? basicActivity.endTime
                }
            });
        }

        [Conditional("UNITY_EDITOR"), Conditional("UNITY_STANDALONE")]
        static void UpdateActivity(Activity activity) => updateAction = () => activityManager.UpdateActivity(activity, x => { });



        [WikiDescription("디스코드 API를 초기화합니다")]
        public static void Initialization()
        {
            DiscordAPIDispose();
            _discord = new Discord(instance._applicationId, (ulong)CreateFlags.NoRequireDiscord);

            discordIsRunning = true;

            userManager.OnCurrentUserUpdate += () =>
            {
                currentUser = userManager.GetCurrentUser();

                Debug.Log("Discord api refresh...", nameof(DiscordManager));
                Debug.Log("Discord user id: " + currentUser.Value.Id, nameof(DiscordManager));
                Debug.Log("Discord user username: " + currentUser.Value.Username, nameof(DiscordManager));

                //디스코드 API를 정상적으로 불러왔는지 체크하고, 실패하면 심각한 오류가 발생할 수 있으니, 프로그램을 종료합니다
                if (!DiscordCheck(Kernel.saveDataPath, currentUser.Value.Id))
                    InitialLoadManager.ApplicationForceQuit("Discord check failed", nameof(DiscordManager));
            };

            Debug.Log("Discord is running");
            UpdateActivity();
        }

        public static void DiscordAPIDispose()
        {
            if (discordIsRunning)
            {
                discord?.Dispose();
                Debug.Log("Discord api disposed");
            }
        }

#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
        [DllImport("discord_check.dll")]
        [WikiDescription("디스코드 API를 정상적으로 불러왔는지 체크하고,\n실패하면 심각한 오류가 발생할 수 있으니, 프로그램을 종료합니다")]
        public static extern bool DiscordCheck(string saveDataPath, long userId);
#else
        public static bool DiscordCheck(string saveDataPath, long userId) => true;
#endif
    }

    public class DiscordNotLoading : Exception
    {
        public DiscordNotLoading() : base("디스코드가 실행되지 않습니다!\nDiscord is not running!")
        {

        }
    }
}
