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
    public class DefModExt_RaceApparel : DefModExtension
    {
        public List<RaceApparelData> raceData = new List<RaceApparelData>();
        public Dictionary<string, RaceApparelData> dataMap;

        public RaceApparelData TryGetRaceApparelData(string raceDef)
        {
            if(dataMap == null)
            {
                dataMap = new Dictionary<string, RaceApparelData>();
                foreach(RaceApparelData rad in raceData)
                {
                    if (string.IsNullOrWhiteSpace(rad.raceDef))
                    {
                        LogUtil.LogError($"Missing <raceDef> tag in RaceApparelData");
                        continue;
                    }
                    if (dataMap.ContainsKey(rad.raceDef))
                    {
                        LogUtil.LogError($"Duplicate apparel data for {rad.raceDef}");
                    }

                    ThingDef race = DefDatabase<ThingDef>.GetNamedSilentFail(rad.raceDef);
                    if(race == null)
                    {
                        LogUtil.LogWarning($"Could not find def for race called '{rad.raceDef}'.");
                    }
                    dataMap.Add(rad.raceDef, rad);
                }
            }

            if(raceDef == null)
            {
                return null;
            }
            return dataMap.TryGetValue(raceDef);
        }
    }
}
