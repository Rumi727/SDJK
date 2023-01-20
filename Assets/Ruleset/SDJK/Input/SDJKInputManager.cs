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

        List<KeyCode> pressKeys = new List<KeyCode>();
        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            if (instance != null)
            {
                bool isRefresh = false;

                for (int i = 0; i < map.notes.Count; i++)
                {
                    inputsDown[i] = InternalGetKey(i, InputType.Down, out List<KeyCode> keyCodes);
                    inputs[i] = InternalGetKey(i, InputType.Alway, out _);
                    inputsUp[i] = InternalGetKey(i, InputType.Up, out _);

                    if (!sdjkManager.isReplay)
                    {
                        if (InputManager.GetKey(keyCodes, InputType.Down))
                        {
                            pressKeys.AddRange(keyCodes);
                            isRefresh = true;
                        }
                        else if (InputManager.GetKey(keyCodes, InputType.Up))
                        {
                            for (int j = 0; j < keyCodes.Count; j++)
                            {
                                for (int k = 0; k < pressKeys.Count; k++)
                                {
                                    if (pressKeys[k] == keyCodes[j])
                                        pressKeys.RemoveAt(k);
                                }
                            }

                            isRefresh = true;
                        }
                    }
                }

                if (isRefresh)
                    sdjkManager.createdReplay.inputs.Add(RhythmManager.currentBeatSound, pressKeys.ToArray());
            }
        }

        bool InternalGetKey(int keyIndex, InputType inputType, out List<KeyCode> keyCode)
        {
            const string originalInputKey = "ruleset.sdjk.";
            string inputKey = originalInputKey + map.notes.Count + "." + keyIndex;

            keyCode = InputManager.controlSettingList[inputKey];
            return InputManager.GetKey(inputKey, inputType);
        }

        public bool ReplayGetKey(int keyIndex, double beat, out double findedBeat)
        {
            const string originalInputKey = "ruleset.sdjk.";
            string inputKey = originalInputKey + map.notes.Count + "." + keyIndex;

            findedBeat = double.MinValue;

            List<KeyCode> list = InputManager.controlSettingList[inputKey];
            if (list == null)
                return false;
            else if (list.Count <= 0)
                return false;

            KeyCode[] pressKeyCodes = sdjkManager.currentReplay.inputs.GetValue(beat, out findedBeat);
            for (int i = 0; i < list.Count; i++)
            {
                bool input = pressKeyCodes.Contains(list[i]);
                if (!input)
                    return false;
            }

            return true;
        }

        /// <exception cref="System.NotSupportedException">
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

            throw new System.NotSupportedException("쿠루미! 이것 좀 봐! 불가능한 일이 일어났어!");
        }
    }
}
