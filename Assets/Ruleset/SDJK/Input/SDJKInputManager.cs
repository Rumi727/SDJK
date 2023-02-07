using SCKRM;
using SCKRM.Input;
using SCKRM.Rhythm;
using SDJK.Effect;
using SDJK.Map.Ruleset.SDJK.Map;
using System;
using System.Collections.Generic;
using UnityEngine;

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
            }
        }

        void Update()
        {
            if (!RhythmManager.isPlaying)
                return;

            if (instance != null)
            {
                for (int i = 0; i < map.notes.Count; i++)
                {
                    string keyString = GetKeyString(i, map.notes.Count);

                    inputsDown[i] = InputManager.GetKey(keyString, InputType.Down);
                    inputs[i] = InputManager.GetKey(keyString, InputType.Alway);
                    inputsUp[i] = InputManager.GetKey(keyString, InputType.Up);
                }
            }
        }

        public static string GetKeyString(int keyIndex, int keyCount)
        {
            const string originalInputKey = "ruleset.sdjk.";
            return originalInputKey + keyCount + "." + keyIndex;
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
                    return inputsUp[keyIndex];
            }

            throw new NotSupportedException("쿠루미! 이것 좀 봐! 불가능한 일이 일어났어!");
        }
    }
}
