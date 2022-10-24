using SCKRM;
using SCKRM.Json;
using SCKRM.Sound;
using SCKRM.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace SDJK.MainMenu
{
    public class Visualizer : SCKRM.UI.UI
    {
        [SerializeField, NotNull] VisualizerBar barPrefab;
        int barsLock = 0;
        VisualizerBar[] bars = new VisualizerBar[0];



        public bool circle { get => _circle; set => _circle = value; } [SerializeField] bool _circle = false;



        /// <summary>
        /// Thread-Safe
        /// </summary>
        public bool all
        {
            get
            {
                while (Interlocked.CompareExchange(ref allLock, 1, 0) != 0)
                    Thread.Sleep(1);

                bool all = _all;

                Interlocked.Decrement(ref allLock);

                return all;
            }
            set
            {
                while (Interlocked.CompareExchange(ref allLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _all = value;

                Interlocked.Decrement(ref allLock);
            }
        }
        [SerializeField] bool _all = false;
        int allLock = 0;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public bool left
        {
            get
            {
                while (Interlocked.CompareExchange(ref leftLock, 1, 0) != 0)
                    Thread.Sleep(1);

                bool left = _left;

                Interlocked.Decrement(ref leftLock);

                return left;
            }
            set
            {
                while (Interlocked.CompareExchange(ref leftLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _left = value;

                Interlocked.Decrement(ref leftLock);
            }
        }
        [SerializeField] bool _left = false;
        int leftLock = 0;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public int divide
        {
            get
            {
                while (Interlocked.CompareExchange(ref divideLock, 1, 0) != 0)
                    Thread.Sleep(1);

                int divide = _divide.Clamp(1, length);

                Interlocked.Decrement(ref divideLock);

                return divide;
            }
            set
            {
                while (Interlocked.CompareExchange(ref divideLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _divide = value.Clamp(1, length);

                Interlocked.Decrement(ref divideLock);
            }
        }
        [Min(1), SerializeField] int _divide = 4;
        int divideLock = 0;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public int offset
        {
            get
            {
                while (Interlocked.CompareExchange(ref offsetLock, 1, 0) != 0)
                    Thread.Sleep(1);

                int offset = _offset.Clamp(0, length);

                Interlocked.Decrement(ref offsetLock);

                return offset;
            }
            set
            {
                while (Interlocked.CompareExchange(ref offsetLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _offset = value.Clamp(0, length);

                Interlocked.Decrement(ref offsetLock);
            }
        }
        [Min(0), SerializeField] int _offset = 0;
        int offsetLock = 0;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public float size
        {
            get
            {
                while (Interlocked.CompareExchange(ref sizeLock, 1, 0) != 0)
                    Thread.Sleep(1);

                float size = _size;

                Interlocked.Decrement(ref sizeLock);

                return size;
            }
            set
            {
                while (Interlocked.CompareExchange(ref sizeLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _size = value;

                Interlocked.Decrement(ref sizeLock);
            }
        }
        [Min(0), SerializeField] float _size = 0;
        int sizeLock = 0;

        public int length { get => _length; set => _length = value; } [Min(1), SerializeField] int _length = 160;



        protected override void OnEnable()
        {
            tempSoundPlayer = null;
            tempCircle = false;
        }



        float[] samples = new float[256];
        ISoundPlayer tempSoundPlayer;
        bool tempCircle = false;
        void Update()
        {
            if (Interlocked.CompareExchange(ref barsLock, 1, 0) != 0) { }

            bool isBarsLengthChanged = false;

            if (bars != null)
                isBarsLengthChanged = bars.Length != length;

            try
            {
                if (isBarsLengthChanged || tempCircle != circle)
                {
                    for (int i = 0; i < bars.Length; i++)
                    {
                        GameObject bar = bars[i].gameObject;
                        if (bar != null)
                            Destroy(bar);
                    }

                    bars = new VisualizerBar[length];

                    for (int i = 0; i < length; i++)
                    {
                        VisualizerBar visualizerBar = Instantiate(barPrefab, transform);
                        bars[i] = visualizerBar;

                        if (!circle)
                        {
                            Vector2 pos = new Vector2((float)i / length, 0);
                            visualizerBar.rectTransform.anchorMin = pos;
                            visualizerBar.rectTransform.anchorMax = pos;
                        }
                        else
                        {
                            float rotation = 360f / length * i;
                            float rotationRad = rotation * Mathf.Deg2Rad;

                            Vector2 pos = new Vector2((rotationRad.Sin() * 0.5f) + 0.5f, (rotationRad.Cos() * 0.5f) + 0.5f);
                            visualizerBar.rectTransform.anchorMin = pos;
                            visualizerBar.rectTransform.anchorMax = pos;

                            visualizerBar.rectTransform.localEulerAngles = new Vector3(0, 0, -rotation);
                        }
                    }

                    tempCircle = circle;
                }
            }
            finally
            {
                Interlocked.Decrement(ref barsLock);
            }

            if (BGMManager.bgm != null && tempSoundPlayer != BGMManager.bgm.soundPlayer && BGMManager.bgm.soundPlayer != null)
            {
                if (tempSoundPlayer != null && !tempSoundPlayer.isRemoved)
                    tempSoundPlayer.onAudioFilterReadEvent -= VisualizerUpdate;

                BGMManager.bgm.soundPlayer.onAudioFilterReadEvent += VisualizerUpdate;
                tempSoundPlayer = BGMManager.bgm.soundPlayer;
            }

            for (int i = 0; i < bars.Length; i++)
            {
                VisualizerBar visualizerBar = bars[i];
                visualizerBar.SizeUpdate();
            }
        }

        protected override void OnDisable()
        {
            if (BGMManager.bgm != null && BGMManager.bgm.soundPlayer != null)
                BGMManager.bgm.soundPlayer.onAudioFilterReadEvent -= VisualizerUpdate;
        }

        int i = 0;
        double timer = AudioSettings.dspTime;
        public void VisualizerUpdate(float[] data, int channels)
        {
            if (samples.Length != data.Length)
                samples = new float[data.Length];

            if (Interlocked.CompareExchange(ref barsLock, 1, 0) != 0) { }

            try
            {
                if (all)
                {
                    if (timer <= AudioSettings.dspTime)
                    {
                        timer = AudioSettings.dspTime + 0.005f;

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
                        timer = AudioSettings.dspTime + 0.01f;

                        samples = data;

                        float average = 0;
                        for (int j = 0; j < samples.Length; j += 2)
                            average += samples[j].Abs();

                        average /= samples.Length / 2f;

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
            finally
            {
                Interlocked.Decrement(ref barsLock);
            }
        }
    }
}