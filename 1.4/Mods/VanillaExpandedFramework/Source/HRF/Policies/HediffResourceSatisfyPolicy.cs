using Verse;

namespace HRF
{
    public class HediffResourceSatisfyPolicy : IExposable
    {
        public FloatRange resourceSeekingThreshold;
        public bool seekingIsEnabled;
        public HediffResourceSatisfyPolicy()
        {

        }
        public void ExposeData()
        {
            Scribe_Values.Look(ref resourceSeekingThreshold, "resourceSeekingThreshold");
            Scribe_Values.Look(ref seekingIsEnabled, "seekingIsEnabled");
        }
    }
}
