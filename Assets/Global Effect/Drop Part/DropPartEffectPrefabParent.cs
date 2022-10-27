using SCKRM;
using SCKRM.Object;
using SCKRM.Rhythm;
using SDJK.Effect;

namespace SDJK
{
    public abstract class DropPartEffectPrefabParent : ObjectPooling
    {
        public DropPartEffect dropPartEffect { get; private set; }

        public int indexOffset { get; private set; }
        public bool isLeft { get; private set; }

        public bool dropPart { get; private set; } = false;

        public double offsetCurrentBeat { get; private set; } = 0;
        public double offsetCurrentBeatReapeat { get; private set; } = 0;

        double lastCurrentBeatReapeat = 0;
        protected virtual void Update()
        {
            if (dropPartEffect == null)
                return;

            offsetCurrentBeat = RhythmManager.currentBeat - indexOffset;
            offsetCurrentBeatReapeat = offsetCurrentBeat.Reapeat(dropPartEffect.count.Ceil());

            if (offsetCurrentBeatReapeat < lastCurrentBeatReapeat)
                dropPart = RhythmManager.dropPart || dropPartEffect.forceShow;

            lastCurrentBeatReapeat = offsetCurrentBeatReapeat;
        }

        public void Refresh(DropPartEffect dropPartEffect, int indexOffset, bool isLeft)
        {
            this.dropPartEffect = dropPartEffect;

            this.indexOffset = indexOffset;
            this.isLeft = isLeft;
        }

        public override bool Remove()
        {
            if (!base.Remove())
                return false;

            dropPartEffect = null;

            indexOffset = 0;
            isLeft = false;

            dropPart = false;

            offsetCurrentBeat = 0;
            offsetCurrentBeatReapeat = 0;

            lastCurrentBeatReapeat = 0;
            return true;
        }
    }
}
