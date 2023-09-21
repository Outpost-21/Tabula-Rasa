using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class PawnGroupMaker_Complex : PawnGroupMaker
    {
        public bool limitFactionPoints = false;
        public int minPoints = int.MinValue;
        public int maxPoints = int.MaxValue;

        public bool limitTemperature = false;
        public float minTemperature = -999f;
        public float maxTemperature = 999f;

        public bool limitTimeSinceStart = false;
        public int onlyAfterDays = int.MinValue;
        public int onlyBeforeDays = int.MaxValue;

        public bool requiredPollutionLevel = false;
        public bool requiredPollutionLevelExact = false;
        public PollutionLevel pollutionLevel = PollutionLevel.None;

        public bool CanGenerate(PawnGroupMakerParms parms)
        {
            int num = FactionUtil.FactionPoints();
            if (limitFactionPoints && (num < minPoints || num > maxPoints))
            {
                return false;
            }
            if (limitTemperature && (Find.CurrentMap.mapTemperature.OutdoorTemp < minTemperature || Find.CurrentMap.mapTemperature.OutdoorTemp > maxTemperature))
            {
                return false;
            }
            int days = Mathf.FloorToInt(Find.TickManager.TicksSinceSettle.TicksToDays());
            if (limitTimeSinceStart && (days >= onlyBeforeDays || days < onlyAfterDays))
            {
                return false;
            }
            if (requiredPollutionLevel && Find.CurrentMap.TileInfo.PollutionLevel() >= pollutionLevel)
            {
                return false;
            }
            if (requiredPollutionLevelExact && Find.CurrentMap.TileInfo.PollutionLevel() != pollutionLevel)
            {
                return false;
            }
            return true;
        }
    }
}
