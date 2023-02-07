using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
using SDJK.Effect;
using SDJK.Map.Ruleset.SDJK.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace SDJK.Ruleset.SDJK.Input
{
    public sealed class SDJKInputManager : ManagerBase<SDJKInputManager>
    {
        [SerializeField] SDJKManager _sdjkManager; public SDJKManager sdjkManager => _sdjkManager;
        [SerializeField] EffectManager _effectManager; public EffectManager effectManager => _effectManager;

        public SDJKMapFile map => sdjkManager.map;

        List<bool> inputsDown = new List<bool>();
        List<bool> inputs = new List<bool>();
        List<bool> inputsUp = new List<bool>();



        public void Refresh()
        {
            if (SingletonCheck(this))
            {
                for (int i = 0; i < map.notes.Count; i++)
                {
                    inputsDown.Add(false);
                    inputsUp.Add(false);
                    inputs.Add(false);
                }

                sdjkManager.createdReplay.inputs.Add(RhythmManager.currentBeatSound, pressKeys.ToArray());
            }
        }

        List<string> pressKeys = new List<string>();
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            if (instance != null)
            {
                bool isRefresh = false;

                for (int i = 0; i < map.notes.Count; i++)
                {
                    string keyString = InternalGetKeyString(i);
                    bool down, up;

                    inputsDown[i] = down = InputManager.GetKey(keyString, InputType.Down);
                    inputs[i] = InputManager.GetKey(keyString, InputType.Alway);
                    inputsUp[i] = up = InputManager.GetKey(keyString, InputType.Up);

                    if (!sdjkManager.isReplay)
                    {
                        if (down)
                        {
                            pressKeys.Add(InternalGetKeyString(i));
                            isRefresh = true;
                        }
                        else if (up)
                        {
                            for (int k = 0; k < pressKeys.Count; k++)
                            {
                                if (pressKeys[k] == keyString)
                                    pressKeys.RemoveAt(k);
                            }

                            isRefresh = true;
                        }
                    }
                }

                if (isRefresh)
                    sdjkManager.createdReplay.inputs.Add(RhythmManager.currentBeatSound, pressKeys.ToArray());
            }
        }

        string InternalGetKeyString(int keyIndex)
        {
            const string originalInputKey = "ruleset.sdjk.";
            return originalInputKey + map.notes.Count + "." + keyIndex;
        }

        public bool ReplayGetKey(int keyIndex, double beat, out double findedBeat)
        {
            const string originalInputKey = "ruleset.sdjk.";
            string inputKey = originalInputKey + map.notes.Count + "." + keyIndex;

            string[] pressKeys = sdjkManager.currentReplay.inputs.GetValue(beat, out findedBeat);
            bool input = pressKeys.Contains(inputKey);
            if (!input)
                return false;

            return true;
        }

        /// <exception cref="NotSupportedException">
        /// 이 예외가 발생한다면 학교생활부로 연락해주세요
        /// </exception>
        public bool GetKey(int keyIndex, InputType inputType = InputType.Down)
        {
            switch (inputType)
            {
                case InputType.Down:
                    return inputsDown[keyIndex];
                case InputType.Alway:
                    return inputs[keyIndex];
                case InputType.Up:
                    return inputsDown[keyIndex];
            }

            throw new NotSupportedException("쿠루미! 이것 좀 봐! 불가능한 일이 일어났어!");
        }
    }
}
