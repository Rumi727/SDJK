using Newtonsoft.Json;
using SCKRM;
using SCKRM.Rhythm;
using SCKRM.SaveLoad;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.Effect
{
    [RequireComponent(typeof(Image))]
    public class BackgroundTransparency : MonoBehaviour
    {
        [GeneralSaveLoad]
        public class SaveData
        {
            [JsonProperty] public static float backgroundTransparency { get; set; } = 70;
        }

        public Image image => this.GetComponentFieldSave(_image); Image _image;

        void Update()
        {
            Color color = image.color;

            if (RhythmManager.time >= -RhythmManager.startDelay)
                color.a = color.a.MoveTowards(SaveData.backgroundTransparency * 0.01f, 0.03f * Kernel.fpsUnscaledSmoothDeltaTime);
            else
                color.a = color.a.MoveTowards(0, 0.03f * Kernel.fpsUnscaledSmoothDeltaTime);

            if (image.color != color)
            {
                image.color = color;
                image.enabled = color.a != 0;
            }
        }
    }
}
