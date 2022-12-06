using SCKRM;
using SCKRM.Object;
using System.Collections.Generic;
using UnityEngine;

namespace SDJK.Effect
{
    public sealed class DropPartEffect : Effect
    {
        [SerializeField] string _leftPrefab = "drop_part_effect.left"; public string leftPrefab => _leftPrefab;
        [SerializeField] string _rightPrefab = "drop_part_effect.right"; public string rightPrefab => _rightPrefab;

        [SerializeField, Min(1)] float _count = 3; public float count => _count;
        [SerializeField, Min(0)] float _width = 10; public float width => _width;
        [SerializeField] bool _forceShow = false; public bool forceShow => _forceShow;

        public override void Refresh(bool force = false) { }

        int lastCount = -1;
        protected override void RealUpdate()
        {
            if (lastCount != count.CeilToInt())
            {
                PrefabRefresh();
                lastCount = count.CeilToInt();
            }
        }

        List<DropPartEffectPrefabParent> prefabs = new List<DropPartEffectPrefabParent>();
        void PrefabRefresh()
        {
            for (int i = 0; i < prefabs.Count; i++)
                prefabs[i].Remove();

            for (int i = 0; i < count.Ceil(); i++)
            {
                DropPartEffectPrefabParent left = (DropPartEffectPrefabParent)ObjectPoolingSystem.ObjectCreate(leftPrefab, transform).monoBehaviour;
                DropPartEffectPrefabParent right = (DropPartEffectPrefabParent)ObjectPoolingSystem.ObjectCreate(rightPrefab, transform).monoBehaviour;

                left.Refresh(this, i, true);
                right.Refresh(this, i, false);

                prefabs.Add(left);
                prefabs.Add(right);
            }
        }
    }
}
