using SCKRM;
using SCKRM.Sound;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace SDJK.Effect
{
    public class Visualizer : UIEffect
    {
        [SerializeField, NotNull] VisualizerBar barPrefab;
        int barsLock = 0;
        VisualizerBar[] bars = new VisualizerBar[0];



        public bool circle { get => _circle; set => _circle = value; } [SerializeField] bool _circle = false;



        /// <summary>
        /// Thread-Safe
        /// </summary>
        public bool left
        {
            get
            {
                while (Interlocked.CompareExchange(ref barsLock, 1, 0) != 0)
                    Thread.Sleep(1);

                bool left = _left;

                Interlocked.Decrement(ref barsLock);

                return left;
            }
            set
            {
                while (Interlocked.CompareExchange(ref barsLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _left = value;

                Interlocked.Decrement(ref barsLock);
            }
        }
        [SerializeField] bool _left = false;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public int divide
        {
            get
            {
                while (Interlocked.CompareExchange(ref barsLock, 1, 0) != 0)
                    Thread.Sleep(1);

                int divide = _divide.Clamp(1);

                Interlocked.Decrement(ref barsLock);

                return divide;
            }
            set
            {
                while (Interlocked.CompareExchange(ref barsLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _divide = value.Clamp(1);

                Interlocked.Decrement(ref barsLock);
            }
        }
        [Min(1), SerializeField] int _divide = 4;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public int offset
        {
            get
            {
                while (Interlocked.CompareExchange(ref barsLock, 1, 0) != 0)
                    Thread.Sleep(1);

                int offset = _offset;

                Interlocked.Decrement(ref barsLock);

                return offset;
            }
            set
            {
                while (Interlocked.CompareExchange(ref barsLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _offset = value;

                Interlocked.Decrement(ref barsLock);
            }
        }
        [SerializeField] int _offset = 0;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public float size
        {
            get
            {
                while (Interlocked.CompareExchange(ref barsLock, 1, 0) != 0)
                    Thread.Sleep(1);

                float size = _size;

                Interlocked.Decrement(ref barsLock);

                return size;
            }
            set
            {
                while (Interlocked.CompareExchange(ref barsLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _size = value;

                Interlocked.Decrement(ref barsLock);
            }
        }
        [Min(0), SerializeField] float _size = 0;

        /// <summary>
        /// Thread-Safe
        /// </summary>
        public float speed
        {
            get
            {
                while (Interlocked.CompareExchange(ref barsLock, 1, 0) != 0)
                    Thread.Sleep(1);

                float speed = _speed;

                Interlocked.Decrement(ref barsLock);

                return speed;
            }
            set
            {
                while (Interlocked.CompareExchange(ref barsLock, 1, 0) != 0)
                    Thread.Sleep(1);

                _speed = value;

                Interlocked.Decrement(ref barsLock);
            }
        }
        [Min(0), SerializeField] float _speed = 0;

        public int length { get => _length; set => _length = value; } [Min(1), SerializeField] int _length = 160;



        void OnEnable()
        {
            lastSoundPlayer = null;
            tempCircle = false;
        }



        public override void Refresh(bool force = false) { }



        ISoundPlayer lastSoundPlayer;
        bool tempCircle = false;
        void Update()
        {
            if (effectManager != null && lastSoundPlayer != effectManager.soundPlayer && effectManager.soundPlayer != null)
            {
                if (lastSoundPlayer != null && !lastSoundPlayer.isRemoved)
                    lastSoundPlayer.onAudioFilterReadEvent -= VisualizerUpdate;

                effectManager.soundPlayer.onAudioFilterReadEvent += VisualizerUpdate;
                lastSoundPlayer = effectManager.soundPlayer;
            }

            if (Interlocked.CompareExchange(ref barsLock, 1, 0) != 0)
                return;

            try
            {
                bool isBarsLengthChanged = false;

                if (bars != null)
                    isBarsLengthChanged = bars.Length != length;

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

                for (int i = 0; i < bars.Length; i++)
                {
                    VisualizerBar visualizerBar = bars[i];
                    visualizerBar.SizeUpdate();
                }
            }
            finally
            {
                Interlocked.Decrement(ref barsLock);
            }
        }

        void OnDisable()
        {
            if (effectManager.soundPlayer != null && effectManager.soundPlayer != null)
                effectManager.soundPlayer.onAudioFilterReadEvent -= VisualizerUpdate;
        }

        int targetBarIndex = 0;
        List<float> sampleAverages = new List<float>();
        Stopwatch timer = Stopwatch.StartNew();
        public void VisualizerUpdate(float[] data, int channels)
        {
            if (Interlocked.CompareExchange(ref barsLock, 1, 0) != 0)
                return;

            try
            {
                bool left = _left;
                int divide = _divide.Clamp(1, length);
                float size = _size;
                int offset = _offset.Repeat(length / divide);
                float speed = _speed.Clamp(0);

                float finalAverage = 0;
                for (int i = 0; i < channels; i++)
                {
                    float sampleChannel = 0;
                    for (int j = i; j < data.Length; j += channels)
                    {
                        float sample = data[j].Abs();
                        if (sample > sampleChannel)
                            sampleChannel = sample;
                    }

                    finalAverage += sampleChannel / channels;
                }

                sampleAverages.Add(finalAverage);

                if (timer.Elapsed.TotalSeconds >= 0.01f / speed)
                {
                    timer.Restart();

                    float average = 0;
                    for (int i = 0; i < sampleAverages.Count; i++)
                        average += sampleAverages[i];

                    sampleAverages.Clear();

                    for (int i = 0; i < divide; i++)
                    {
                        int index = targetBarIndex + (bars.Length / divide * i) + offset;
                        if (index >= bars.Length)
                        {
                            if (index - bars.Length >= bars.Length)
                                bars[0].size = average * 720 * size;
                            else
                                bars[index - bars.Length].size = average * 720 * size;
                        }
                        else
                            bars[index].size = average * 720 * size;
                    }

                    if (left)
                    {
                        targetBarIndex--;
                        if (targetBarIndex < 0)
                            targetBarIndex = bars.Length - 1;
                    }
                    else
                    {
                        targetBarIndex++;
                        if (targetBarIndex >= bars.Length)
                            targetBarIndex = 0;
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