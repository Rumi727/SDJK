using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SCKRM.KnownFolder
{
    [WikiDescription("윈도우의 특수 폴더 관련 메소드가 있는 클래스 입니다")]
    public static class KnownFolders
    {
        /// <summary>
        /// 특수 폴더를 가져옵니다
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [WikiDescription("특수 폴더를 가져옵니다")]
        public static string GetPath(KnownFolderType type)
        {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
            SHGetKnownFolderPath(type.GetGuid(), 0x00004000, new IntPtr(0), out var PathPointer);
            var Result = Marshal.PtrToStringUni(PathPointer);
            Marshal.FreeCoTaskMem(PathPointer);

            return Result;
#else
            throw new NotSupportedException();
#endif
        }

#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
        [DllImport("Shell32.dll")]
        static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);
#endif
    }

    //https://gitlab.com/Syroot/KnownFolders/-/blob/master/src/Syroot.KnownFolders/KnownFolderType.cs
    public enum KnownFolderType
    {
        [KnownFolderGuid("008CA0B1-55B4-4C56-B8A8-4DE4B299D3BE")]
        AccountPictures,
        [KnownFolderGuid("724EF170-A42D-4FEF-9F26-B60E846FBA4F")]
        AdminTools,
        [KnownFolderGuid("B2C5E279-7ADD-439F-B28C-C41FE1BBF672")]
        AppDataDesktop,
        [KnownFolderGuid("7BE16610-1F7F-44AC-BFF0-83E15F2FFCA1")]
        AppDataDocuments,
        [KnownFolderGuid("7CFBEFBC-DE1F-45AA-B843-A542AC536CC9")]
        AppDataFavorites,
        [KnownFolderGuid("559D40A3-A036-40FA-AF61-84CB430A4D34")]
        AppDataProgramData,
        [KnownFolderGuid("A3918781-E5F2-4890-B3D9-A7E54332328C")]
        ApplicationShortcuts,
        [KnownFolderGuid("AB5FB87B-7CE2-4F83-915D-550846C9537B")]
        CameraRoll,
        [KnownFolderGuid("9E52AB10-F80D-49DF-ACB8-4330F5687855")]
        CDBurning,
        [KnownFolderGuid("D0384E7D-BAC3-4797-8F14-CBA229B392B5")]
        CommonAdminTools,
        [KnownFolderGuid("C1BAE2D0-10DF-4334-BEDD-7AA20B227A9D")]
        CommonOemLinks,
        [KnownFolderGuid("0139D44E-6AFE-49F2-8690-3DAFCAE6FFB8")]
        CommonPrograms,
        [KnownFolderGuid("A4115719-D62E-491D-AA7C-E74B8BE3B067")]
        CommonStartMenu,
        [KnownFolderGuid("82A5EA35-D9CD-47C5-9629-E15D2F714E6E")]
        CommonStartup,
        [KnownFolderGuid("B94237E7-57AC-4347-9151-B08C6C32D1F7")]
        CommonTemplates,
        [KnownFolderGuid("56784854-C6CB-462B-8169-88E350ACB882")]
        Contacts,
        [KnownFolderGuid("2B0F765D-C0E9-4171-908E-08A611B84FF6")]
        Cookies,
        [KnownFolderGuid("B4BFCC3A-DB2C-424C-B029-7FE99A87C641")]
        Desktop,
        [KnownFolderGuid("5CE4A5E9-E4EB-479D-B89F-130C02886155")]
        DeviceMetadataStore,
        [KnownFolderGuid("FDD39AD0-238F-46AF-ADB4-6C85480369C7")]
        Documents,
        [KnownFolderGuid("7B0DB17D-9CD2-4A93-9733-46CC89022E7C")]
        DocumentsLibrary,
        [KnownFolderGuid("F42EE2D3-909F-4907-8871-4C22FC0BF756")]
        DocumentsLocalized,
        [KnownFolderGuid("374DE290-123F-4565-9164-39C4925E467B")]
        Downloads,
        [KnownFolderGuid("7d83ee9b-2244-4e70-b1f5-5393042af1e4")]
        DownloadsLocalized,
        [KnownFolderGuid("1777F761-68AD-4D8A-87BD-30B759FA33DD")]
        Favorites,
        [KnownFolderGuid("FD228CB7-AE11-4AE3-864C-16F3910AB8FE")]
        Fonts,
        [KnownFolderGuid("054FAE61-4DD8-4787-80B6-090220C4B700")]
        GameTasks,
        [KnownFolderGuid("D9DC8A3B-B784-432E-A781-5A1130A75963")]
        History,
        [KnownFolderGuid("BCB5256F-79F6-4CEE-B725-DC34E402FD46")]
        ImplicitAppShortcuts,
        [KnownFolderGuid("352481E8-33BE-4251-BA85-6007CAEDCF9D")]
        InternetCache,
        [KnownFolderGuid("1B3EA5DC-B587-4786-B4EF-BD1DC332AEAE")]
        Libraries,
        [KnownFolderGuid("BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968")]
        Links,
        [KnownFolderGuid("F1B32785-6FBA-4FCF-9D55-7B8E7F157091")]
        LocalAppData,
        [KnownFolderGuid("A520A1A4-1780-4FF6-BD18-167343C5AF16")]
        LocalAppDataLow,
        [KnownFolderGuid("2A00375E-224C-49DE-B8D1-440DF7EF3DDC")]
        LocalizedResourcesDir,
        [KnownFolderGuid("4BD8D571-6D19-48D3-BE97-422220080E43")]
        Music,
        [KnownFolderGuid("2112AB0A-C86A-4FFE-A368-0DE96E47012E")]
        MusicLibrary,
        [KnownFolderGuid("A0C69A99-21C8-4671-8703-7934162FCF1D")]
        MusicLocalized,
        [KnownFolderGuid("C5ABBF53-E17F-4121-8900-86626FC2C973")]
        NetHood,
        [KnownFolderGuid("31C0DD25-9439-4F12-BF41-7FF4EDA38722")]
        Objects3D,
        [KnownFolderGuid("2C36C0AA-5812-4B87-BFD0-4CD0DFB19B39")]
        OriginalImages,
        [KnownFolderGuid("69D2CF90-FC33-4FB7-9A0C-EBB0F0FCB43C")]
        PhotoAlbums,
        [KnownFolderGuid("A990AE9F-A03B-4E80-94BC-9912D7504104")]
        PicturesLibrary,
        [KnownFolderGuid("33E28130-4E1E-4676-835A-98395C3BC3BB")]
        Pictures,
        [KnownFolderGuid("0DDD015D-B06C-45D5-8C4C-F59713854639")]
        PicturesLocalized,
        [KnownFolderGuid("DE92C1C7-837F-4F69-A3BB-86E631204A23")]
        Playlists,
        [KnownFolderGuid("9274BD8D-CFD1-41C3-B35E-B13F55A758F4")]
        PrintHood,
        [KnownFolderGuid("5E6C858F-0E22-4760-9AFE-EA3317B67173")]
        Profile,
        [KnownFolderGuid("62AB5D82-FDC1-4DC3-A9DD-070D1D495D97")]
        ProgramData,
        [KnownFolderGuid("905E63B6-C1BF-494E-B29C-65B732D3D21A")]
        ProgramFiles,
        [KnownFolderGuid("6D809377-6AF0-444B-8957-A3773F02200E")]
        ProgramFilesX64,
        [KnownFolderGuid("7C5A40EF-A0FB-4BFC-874A-C0F2E0B9FA8E")]
        ProgramFilesX86,
        [KnownFolderGuid("F7F1ED05-9F6D-47A2-AAAE-29D317C6F066")]
        ProgramFilesCommon,
        [KnownFolderGuid("6365D5A7-0F0D-45E5-87F6-0DA56B6A4F7D")]
        ProgramFilesCommonX64,
        [KnownFolderGuid("DE974D24-D9C6-4D3E-BF91-F4455120B917")]
        ProgramFilesCommonX86,
        [KnownFolderGuid("A77F5D77-2E2B-44C3-A6A2-ABA601054A51")]
        Programs,
        [KnownFolderGuid("DFDF76A2-C82A-4D63-906A-5644AC457385")]
        Public,
        [KnownFolderGuid("C4AA340D-F20F-4863-AFEF-F87EF2E6BA25")]
        PublicDesktop,
        [KnownFolderGuid("ED4824AF-DCE4-45A8-81E2-FC7965083634")]
        PublicDocuments,
        [KnownFolderGuid("3D644C9B-1FB8-4F30-9B45-F670235F79C0")]
        PublicDownloads,
        [KnownFolderGuid("DEBF2536-E1A8-4C59-B6A2-414586476AEA")]
        PublicGameTasks,
        [KnownFolderGuid("48DAF80B-E6CF-4F4E-B800-0E69D84EE384")]
        PublicLibraries,
        [KnownFolderGuid("3214FAB5-9757-4298-BB61-92A9DEAA44FF")]
        PublicMusic,
        [KnownFolderGuid("B6EBFB86-6907-413C-9AF7-4FC2ABF07CC5")]
        PublicPictures,
        [KnownFolderGuid("E555AB60-153B-4D17-9F04-A5FE99FC15EC")]
        PublicRingtones,
        [KnownFolderGuid("0482AF6C-08F1-4C34-8C90-E17EC98B1E17")]
        PublicUserTiles,
        [KnownFolderGuid("2400183A-6185-49FB-A2D8-4A392A602BA3")]
        PublicVideos,
        [KnownFolderGuid("52A4F021-7B75-48A9-9F6B-4B87A210BC8F")]
        QuickLaunch,
        [KnownFolderGuid("AE50C081-EBD2-438A-8655-8A092E34987A")]
        Recent,
        [KnownFolderGuid("1A6FDBA2-F42D-4358-A798-B74D745926C5")]
        RecordedTVLibrary,
        [KnownFolderGuid("8AD10C31-2ADB-4296-A8F7-E4701232C972")]
        ResourceDir,
        [KnownFolderGuid("C870044B-F49E-4126-A9C3-B52A1FF411E8")]
        Ringtones,
        [KnownFolderGuid("3EB685DB-65F9-4CF6-A03A-E3EF65729F3D")]
        RoamingAppData,
        [KnownFolderGuid("AAA8D5A5-F1D6-4259-BAA8-78E7EF60835E")]
        RoamedTileImages,
        [KnownFolderGuid("00BCFC5A-ED94-4E48-96A1-3F6217F21990")]
        RoamingTiles,
        [KnownFolderGuid("B250C668-F57D-4EE1-A63C-290EE7D1AA1F")]
        SampleMusic,
        [KnownFolderGuid("C4900540-2379-4C75-844B-64E6FAF8716B")]
        SamplePictures,
        [KnownFolderGuid("15CA69B3-30EE-49C1-ACE1-6B5EC372AFB5")]
        SamplePlaylists,
        [KnownFolderGuid("859EAD94-2E85-48AD-A71A-0969CB56A6CD")]
        SampleVideos,
        [KnownFolderGuid("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4")]
        SavedGames,
        [KnownFolderGuid("7D1D3A04-DEBB-4115-95CF-2F29DA2920DA")]
        SavedSearches,
        [KnownFolderGuid("B7BEDE81-DF94-4682-A7D8-57A52620B86F")]
        Screenshots,
        [KnownFolderGuid("0D4C3DB6-03A3-462F-A0E6-08924C41B5D4")]
        SearchHistory,
        [KnownFolderGuid("7E636BFE-DFA9-4D5E-B456-D7B39851D8A9")]
        SearchTemplates,
        [KnownFolderGuid("8983036C-27C0-404B-8F08-102D10DCFD74")]
        SendTo,
        [KnownFolderGuid("7B396E54-9EC5-4300-BE0A-2482EBAE1A26")]
        SidebarDefaultParts,
        [KnownFolderGuid("A75D362E-50FC-4FB7-AC2C-A8BEAA314493")]
        SidebarParts,
        [KnownFolderGuid("A52BBA46-E9E1-435F-B3D9-28DAA648C0F6")]
        SkyDrive,
        [KnownFolderGuid("767E6811-49CB-4273-87C2-20F355E1085B")]
        SkyDriveCameraRoll,
        [KnownFolderGuid("24D89E24-2F19-4534-9DDE-6A6671FBB8FE")]
        SkyDriveDocuments,
        [KnownFolderGuid("339719B5-8C47-4894-94C2-D8F77ADD44A6")]
        SkyDrivePictures,
        [KnownFolderGuid("625B53C3-AB48-4EC1-BA1F-A1EF4146FC19")]
        StartMenu,
        [KnownFolderGuid("B97D20BB-F46A-4C97-BA10-5E3608430854")]
        Startup,
        [KnownFolderGuid("1AC14E77-02E7-4E5D-B744-2EB1AE5198B7")]
        System,
        [KnownFolderGuid("D65231B0-B2F1-4857-A4CE-A8E7C6EA7D27")]
        SystemX86,
        [KnownFolderGuid("A63293E8-664E-48DB-A079-DF759E0509F7")]
        Templates,
        [KnownFolderGuid("9E3995AB-1F9C-4F13-B827-48B24B6C7174")]
        UserPinned,
        [KnownFolderGuid("0762D272-C50A-4BB0-A382-697DCD729B80")]
        UserProfiles,
        [KnownFolderGuid("5CD7AEE2-2219-4A67-B85D-6C9CE15660CB")]
        UserProgramFiles,
        [KnownFolderGuid("BCBD3057-CA5C-4622-B42D-BC56DB0AE516")]
        UserProgramFilesCommon,
        [KnownFolderGuid("18989B1D-99B5-455B-841C-AB7C74E4DDFC")]
        Videos,
        [KnownFolderGuid("491E922F-5643-4AF4-A7EB-4E7A138D8174")]
        VideosLibrary,
        [KnownFolderGuid("35286A68-3C57-41A1-BBB1-0EAE73D76C95")]
        VideosLocalized,
        [KnownFolderGuid("F38BF404-1D43-42F2-9305-67DE0B28FC23")]
        Windows
    }

#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
    //https://gitlab.com/Syroot/KnownFolders/-/blob/master/src/Syroot.KnownFolders/KnownFolderType.cs
    static class KnownFolderTypeExtensions
    {
        internal static Guid GetGuid(this KnownFolderType value)
        {
            FieldInfo member = typeof(KnownFolderType).GetField(value.ToString());
            return ((KnownFolderGuidAttribute)Attribute.GetCustomAttributes(member, typeof(KnownFolderGuidAttribute), false)[0]).Guid;
        }
    }

    //https://gitlab.com/Syroot/KnownFolders/-/blob/master/src/Syroot.KnownFolders/KnownFolderType.cs
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    class KnownFolderGuidAttribute : Attribute
    {
        internal Guid Guid { get; }

        internal KnownFolderGuidAttribute(string guid) => Guid = new Guid(guid);
    }
#endif
}
