using SCKRM;
using SCKRM.Json;
using SCKRM.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK
{
    public class Visualizer : MonoBehaviour
    {
        [SerializeField] VisualizerBar barPrefab;
        VisualizerBar[] bars = new VisualizerBar[0];



        [SerializeField] bool _all = false; public bool all { get => _all; set => _all = value; }
        [SerializeField] bool _left = false; public bool left { get => _left; set => _left = value; }
        [Min(1), SerializeField] int _divide = 4; public int divide { get => _divide; set => _divide = value; }
        [Min(0), SerializeField] float _alpha = 1; public float alpha { get => _alpha; set => _alpha = value; }
        [Min(0), SerializeField] int _offset = 0; public int offset { get => _offset; set => _offset = value; }
        [Min(0), SerializeField] float _size = 1; public float size { get => _size; set => _size = value; }
        [Min(1), SerializeField] int _length = 160; public int length { get => _length; set => _length = value; }



        float[] samples = new float[256];
        SoundPlayer tempSoundPlayer;
        void Update()
        {
            if (bars.Length != length)
            {
                for (int i = 0; i < bars.Length; i++)
                    Destroy(bars[i]);

                for (int i = 0; i < bars.Length; i++)
                {
                    VisualizerBar visualizerBar = Instantiate(barPrefab, transform);
                    bars[i] = visualizerBar;
                    visualizerBar.rectTransform.anchoredPosition = new Vector2(i * 8, 0);
                    visualizerBar.rectTransform.sizeDelta = new Vector2(5, 1);
                    visualizerBar.image.color = new Color(1, 1, 1, alpha);
                }
            }

            if (tempSoundPlayer != BGMManager.bgm && BGMManager.bgm.soundPlayer != null)
                BGMManager.bgm.soundPlayer.onAudioFilterReadEvent += VisualizerUpdate;
        }

        void OnDisable()
        {
            if (BGMManager.bgm.soundPlayer != null)
                BGMManager.bgm.soundPlayer.onAudioFilterReadEvent -= VisualizerUpdate;
        }

        int i = 0;
        double timer = AudioSettings.dspTime;
        public void VisualizerUpdate(float[] data, int channels)
        {
            if (samples.Length != data.Length)
                samples = new float[data.Length];

            if (all)
            {
                if (timer <= AudioSettings.dspTime)
                {
                    timer += 0.005f;

                    samples = data;

                    int k = 0;
                    for (int j = 0; j < bars.Length; j++)
                    {
                        if (j - k >= samples.Length)
                            k += samples.Length;

                        bars[j].size = samples[j - k].Abs() * 2400 * size;
                    }
                }
            }
            else
            {
                if (timer <= AudioSettings.dspTime)
                {
                    timer += 0.01f;

                    samples = data;

                    float average = 0;
                    for (int j = 0; j < samples.Length; j += 2)
                        average += samples[j].Abs();

                    average /= samples.Length / 2f;

                    if (divide > bars.Length)
                        divide = bars.Length;

                    for (int j = 0; j < divide; j++)
                    {
                        int index = i + (bars.Length / divide * j) + offset;
                        if (index >= bars.Length)
                        {
                            if (index - bars.Length >= bars.Length)
                                bars[0].size = average * 2400 * size;
                            else
                                bars[index - bars.Length].size = average * 2400 * size;
                        }
                        else
                            bars[index].size = average * 2400 * size;
                    }

                    if (left)
                    {
                        i--;
                        if (i < 0)
                            i = bars.Length - 1;
                    }
                    else
                    {
                        i++;
                        if (i >= bars.Length)
                            i = 0;
                    }
                }
            }
        }
    }
}