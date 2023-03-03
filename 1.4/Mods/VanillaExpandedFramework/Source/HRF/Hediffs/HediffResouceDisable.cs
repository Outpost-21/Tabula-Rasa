using Verse;

namespace HRF
{
    public class HediffResouceDisable : IExposable
    {
        public HediffResouceDisable()
        {

        }

        public HediffResouceDisable(int delayTicks, string disableReason)
        {
            this.delayTicks = delayTicks;
            this.disableReason = disableReason;
        }

        public int delayTicks;
        public string disableReason;

        public void ExposeData()
        {
            Scribe_Values.Look(ref delayTicks, "delayTicks");
            Scribe_Values.Look(ref disableReason, "disableReason");
        }
    }
}
