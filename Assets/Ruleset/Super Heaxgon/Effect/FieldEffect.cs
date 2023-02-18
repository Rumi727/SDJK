using SCKRM;
using SCKRM.Rhythm;
using SDJK.Map.Ruleset.SuperHexagon.Map;
using SDJK.Ruleset.SDJK.Effect;
using UnityEngine;

namespace SDJK.Ruleset.SuperHexagon
{
    public sealed class FieldEffect : SuperHexagonEffect
    {
        [SerializeField] Field _field; public Field field => _field;

        float zRotationOffset = 0;
        protected override void RealUpdate()
        {
            zRotationOffset += map.effect.fieldZRotationSpeed.GetValue(RhythmManager.currentBeatScreen) * Kernel.fpsSmoothDeltaTime;
            zRotationOffset = zRotationOffset.Repeat(360);

            Vector3 rotation = (Vector3)map.effect.fieldRotation.GetValue(RhythmManager.currentBeatScreen);
            transform.localEulerAngles = new Vector3(rotation.x, rotation.y, rotation.z + zRotationOffset);
        }
    }
}
