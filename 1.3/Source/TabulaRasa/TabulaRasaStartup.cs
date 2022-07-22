using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static RimWorld.QuestGen.QuestNode_GetRandomPawnKindForFaction;

namespace TabulaRasa
{
    [StaticConstructorOnStartup]
    public static class TabulaRasaStartup
    {
        static TabulaRasaStartup()
        {
            FillLinkablesAutomatically();
            FillRaceAlternatesAutomatically();
        }

        public static void FillRaceAlternatesAutomatically()
        {
            foreach(RaceSpawningDef rsd in DefDatabase<RaceSpawningDef>.AllDefs)
            {
                if (CheckRaceSpawningDefForFlaws(rsd))
                {
                    DistributeRaceAmongFactionKinds(rsd);
                }
            }
        }

        public static void DistributeRaceAmongFactionKinds(RaceSpawningDef rsd)
        {
            List<PawnKindDef> viableKinds = new List<PawnKindDef>();
            foreach(PawnKindDef pkd in DefDatabase<PawnKindDef>.AllDefs)
            {
                if(pkd.defaultFactionType != null && rsd.factions.Contains(pkd.defaultFactionType))
                {
                    viableKinds.Add(pkd);
                }
            }

            for (int i = 0; i < viableKinds.Count(); i++)
            {
                PawnKindDef curPkd = viableKinds[i];
                if (!curPkd.HasModExtension<DefModExt_PawnKindRaces>())
                {
                    curPkd.modExtensions.Add(new DefModExt_PawnKindRaces());
                }
                for (int j = 0; j < rsd.races.Count(); j++)
                {
                    DefModExt_PawnKindRaces modExt = curPkd.GetModExtension<DefModExt_PawnKindRaces>();
                    if (!modExt.altRaces.Any(ar => ar.thingDef == rsd.races[j]))
                    {
                        ThingDefCountClass weightedRace = new ThingDefCountClass(rsd.races[j], rsd.weight);
                        modExt.altRaces.Add(weightedRace);
                    }
                }
            }
        }

        public static bool CheckRaceSpawningDefForFlaws(RaceSpawningDef rsd)
        {
            if (rsd.races.NullOrEmpty())
            {
                LogUtil.LogWarning($"RaceSpawning Def {rsd.defName} has no races provided, skipping...");
                return false;
            }
            if (rsd.factions.NullOrEmpty())
            {
                LogUtil.LogWarning($"RaceSpawning Def {rsd.defName} has no races provided, skipping...");
                return false;
            }
            return true;
        }

        public static void FillLinkablesAutomatically()
        {
            List<ThingDef> linkableNonFacilities = DefDatabase<ThingDef>.AllDefs.Where(def => def.HasComp(typeof(CompProperties_AffectedByFacilities)) && def.HasModExtension<DefModExt_AutomatedLinkables>()).ToList();

            List<ThingDef> linkableFacilities = DefDatabase<ThingDef>.AllDefs.Where(def => def.HasComp(typeof(CompProperties_Facility)) && def.HasModExtension<DefModExt_AutomatedLinkables>()).ToList();

            for (int i = 0; i < linkableNonFacilities.Count(); i++)
            {
                ThingDef currDef = linkableNonFacilities[i];
                DefModExt_AutomatedLinkables defExt = currDef.GetModExtension<DefModExt_AutomatedLinkables>();
                if (!defExt.linkableTags.NullOrEmpty())
                {
                    for (int j = 0; j < linkableFacilities.Count(); j++)
                    {
                        ThingDef currFac = linkableFacilities[j];
                        DefModExt_AutomatedLinkables facExt = currFac.GetModExtension<DefModExt_AutomatedLinkables>();
                        if (!facExt.linkableTags.NullOrEmpty() && !facExt.linkableTags.Intersect(defExt.linkableTags).EnumerableNullOrEmpty() && !currDef.GetCompProperties<CompProperties_AffectedByFacilities>().linkableFacilities.Contains(currFac))
                        {
                            currDef.GetCompProperties<CompProperties_AffectedByFacilities>().linkableFacilities.Add(currFac);
                        }
                    }
                }
            }
        }
    }
}
