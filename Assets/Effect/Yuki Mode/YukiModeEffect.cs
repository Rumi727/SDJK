using SCKRM;
using SCKRM.Object;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Effect
{
    public class YukiModeEffect : Effect
    {
        [SerializeField] string _leftPrefab = "yuki_mode_effect.left"; public string leftPrefab => _leftPrefab;
        [SerializeField] string _rightPrefab = "yuki_mode_effect.right"; public string rightPrefab => _rightPrefab;

        [SerializeField] bool _onlyRight = false; public bool onlyRight => _onlyRight;

        [SerializeField, Min(1)] float _count = 3; public float count => _count;
        [SerializeField, Min(0)] float _width = 10; public float width => _width;
        [SerializeField] bool _forceShow = false; public bool forceShow => _forceShow;

        void OnEnable()
        {
            PrefabRefresh();
            lastCount = count.CeilToInt();
        }

        void OnDisable()
        {
            for (int i = 0; i < prefabs.Count; i++)
                prefabs[i].Remove();

            prefabs.Clear();
            lastCount = -1;
        }

        int lastCount = -1;
        protected override void RealUpdate()
        {
            if (lastCount != count.CeilToInt())
            {
                PrefabRefresh();
                lastCount = count.CeilToInt();
            }
        }

        List<YukiModeEffectPrefabParent> prefabs = new List<YukiModeEffectPrefabParent>();
        void PrefabRefresh()
        {
            for (int i = 0; i < prefabs.Count; i++)
                prefabs[i].Remove();

            prefabs.Clear();

            for (int i = 0; i < count.Ceil(); i++)
            {
                if (!onlyRight)
                {
                    YukiModeEffectPrefabParent left = (YukiModeEffectPrefabParent)ObjectPoolingSystem.ObjectCreate(leftPrefab, transform).monoBehaviour;

                    left.Refresh(this, i, true);
                    prefabs.Add(left);
                }

                YukiModeEffectPrefabParent right = (YukiModeEffectPrefabParent)ObjectPoolingSystem.ObjectCreate(rightPrefab, transform).monoBehaviour;

                right.Refresh(this, i, false);
                prefabs.Add(right);
            }
        }
    }
}
