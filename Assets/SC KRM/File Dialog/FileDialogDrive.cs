using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using UnityEngine;

namespace SCKRM.FileDialog.Drive
{
    [WikiDescription("윈도우 드라이브 관련 메소드가 들어있는 클래스 입니다")]
    public static class FileDialogDrive
    {
        const string KERNEL32 = "kernel32.dll";

        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
        [ResourceExposure(ResourceScope.None)]
        static extern bool GetVolumeInformation(string drive, [Out] StringBuilder volumeName, int volumeNameBufLen, out int volSerialNumber, out int maxFileNameLen, out int fileSystemFlags, [Out] StringBuilder fileSystemName, int fileSystemNameBufLen);

        [WikiDescription("드라이브의 볼륨 라벨을 가져옵니다")]
        public static string GetDriveVolumeLabel(DriveInfo driveInfo)
        {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
            // NTFS uses a limit of 32 characters for the volume label,
            // as of Windows Server 2003.
            const int volNameLen = 50;
            StringBuilder volumeName = new StringBuilder(volNameLen);
            const int fileSystemNameLen = 50;
            StringBuilder fileSystemName = new StringBuilder(fileSystemNameLen);
            int serialNumber, maxFileNameLen, fileSystemFlags;

            try
            {
                GetVolumeInformation(driveInfo.Name, volumeName, volNameLen, out serialNumber, out maxFileNameLen, out fileSystemFlags, fileSystemName, fileSystemNameLen);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return volumeName.ToString();
#else
            throw new NotSupportedException();
#endif
        }

        [WikiDescription("드라이브의 전체 이름을 가져옵니다")]
        public static string GetDriveFullName(DriveInfo driveInfo)
        {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
            string label = GetDriveVolumeLabel(driveInfo);
            string name = driveInfo.Name;
            name = name.Remove(driveInfo.Name.Length - 1);

            return label + " (" + name + ")";
#else
            return driveInfo.Name.Remove(driveInfo.Name.Length - 2);
#endif
        }
    }
}
