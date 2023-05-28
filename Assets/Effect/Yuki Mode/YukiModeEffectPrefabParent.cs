using SCKRM;
using SCKRM.Object;
using SCKRM.Rhythm;

namespace SDJK.Effect
{
    public abstract class YukiModeEffectPrefabParent : ObjectPoolingBase
    {
        public YukiModeEffect yukiModeEffect { get; private set; }

        public int indexOffset { get; private set; }
        public bool isLeft { get; private set; }

        public bool yukiMode { get; private set; } = false;

        public double offsetCurrentBeat { get; private set; } = 0;
        public double offsetCurrentBeatReapeat { get; private set; } = 0;

        double lastCurrentBeatReapeat = 0;
        protected virtual void Update()
        {
            if (yukiModeEffect == null)
                return;

            offsetCurrentBeat = RhythmManager.currentBeatScreen - indexOffset;
            offsetCurrentBeatReapeat = offsetCurrentBeat.Repeat(yukiModeEffect.count.Ceil());

            if (RhythmManager.speed < 0)
            {
                if (offsetCurrentBeatReapeat > lastCurrentBeatReapeat)
                    yukiMode = RhythmManager.screenYukiMode || yukiModeEffect.forceShow;
            }
            else
            {
                if (offsetCurrentBeatReapeat < lastCurrentBeatReapeat)
                    yukiMode = RhythmManager.screenYukiMode || yukiModeEffect.forceShow;
            }

            lastCurrentBeatReapeat = offsetCurrentBeatReapeat;
        }

        public virtual void Refresh(YukiModeEffect yukiModeEffect, int indexOffset, bool isLeft)
        {
            this.yukiModeEffect = yukiModeEffect;

            this.indexOffset = indexOffset;
            this.isLeft = isLeft;
        }

        public override void Remove()
        {
            base.Remove();

            yukiModeEffect = null;

            indexOffset = 0;
            isLeft = false;

            yukiMode = false;

            offsetCurrentBeat = 0;
            offsetCurrentBeatReapeat = 0;

            lastCurrentBeatReapeat = 0;
        }
    }
}
