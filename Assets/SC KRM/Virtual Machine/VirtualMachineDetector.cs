using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;
using SCKRM.ProjectSetting;
using Newtonsoft.Json;
using System.IO;

#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
using System.Management;
#endif

namespace SCKRM.VM
{
    [WikiDescription("가상 머신을 감지하는 클래스 입니다")]
    public static class VirtualMachineDetector
    {
        [ProjectSettingSaveLoad]
        public sealed class Data
        {
            [JsonProperty] public static bool vmBan { get; set; } = false;
        }

        [WikiDescription("하드웨어를 확인하는 방식으로 가상 머신을 감지합니다 (윈도우 전용)")]
        public static bool HardwareDetection()
        {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
            using var searcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem");
            using var items = searcher.Get();

            foreach (var item in items)
            {
                string manufacturer = item["Manufacturer"].ToString().ToLower();
                string model = item["Model"].ToString();
                if ((manufacturer == "microsoft corporation" && model.ToUpperInvariant().Contains("VIRTUAL")) || manufacturer.Contains("vmware") || model == "VirtualBox")
                    return true;
            }

            return false;
#else
            throw new NotSupportedException();
#endif
        }

        [WikiDescription("프로세서를 확인하는 방식으로 가상 머신을 감지합니다")]
        public static bool ProcessDetection()
        {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
            return Process.GetProcesses().Any(x => x.ProcessName == "vmtoolsd" || x.ProcessName == "VBoxTray" || x.ProcessName == "VBoxService");
#else
            throw new NotSupportedException();
#endif
        }

        [WikiDescription("폴더 / 파일을 확인하는 방식으로 가상 머신을 감지합니다")]
        public static bool FileDetection()
        {
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
            return Directory.Exists("C:/Program Files/VMware/VMware Tools") || Directory.Exists("C:/Program Files/Oracle/VirtualBox Guest Additions");
#else
            throw new NotSupportedException();
#endif
        }
    }
}
