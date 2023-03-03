using RimWorld;
using Verse;

namespace HRF
{
    public class SavedSkillRecord : IExposable
    {
        public SkillDef def;

        public int levelInt;

        public Passion passion;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Values.Look(ref levelInt, "levelInt");
            Scribe_Values.Look(ref passion, "passion");
        }
    }
}
