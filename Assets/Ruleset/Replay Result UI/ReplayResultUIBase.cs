using SCKRM;
using SCKRM.UI;
using SDJK.Map;
using SDJK.Replay;

namespace SDJK.Ruleset
{
    public abstract class ReplayResultUIBase : UIAni
    {
        protected IRuleset ruleset = null;
        protected MapFile map = null;
        protected ReplayFile replay = null;

        public virtual void Refresh(IRuleset ruleset, MapFile map, ReplayFile replay)
        {
            this.ruleset = ruleset;
            this.map = map;
            this.replay = replay;
        }

        void Update()
        {
            if (ruleset == null || map == null || replay == null)
                return;

            if (lerp)
                RealUpdate(lerpValue * Kernel.fpsUnscaledSmoothDeltaTime);
            else
                RealUpdate(1);
        }

        public virtual void RealUpdate(float lerpValue) { }
        public abstract void Remove();
    }
}
