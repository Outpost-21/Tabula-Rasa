using RimWorld;
using System.Collections.Generic;
using Verse;

namespace HRF
{
    public class StatBonuses : IExposable
    {
        public Dictionary<StatDef, StatBonus> statBonuses;
        public StatBonuses()
        {

        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref statBonuses, "statBonuses", LookMode.Def, LookMode.Deep, ref statDefsKeys, ref statBonusesValues);
        }

        private List<StatDef> statDefsKeys;
        private List<StatBonus> statBonusesValues;
    }
}
