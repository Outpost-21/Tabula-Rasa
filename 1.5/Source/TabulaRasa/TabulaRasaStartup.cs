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
            TabulaRasaSettings settings = TabulaRasaMod.settings;

            FillLinkablesAutomatically();
            if (ModLister.RoyaltyInstalled)
            {
                FixRoyaltyBullshit(settings);
            }
            EnableNeededSubCategories();
        }

        public static void EnableNeededSubCategories()
        {
            foreach(ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                DefModExt_SubCategoryBuilding modExt = def.GetModExtension<DefModExt_SubCategoryBuilding>();
                if(modExt != null)
                {
                    if(modExt.subCategory == null)
                    {
                        LogUtil.Error($"{def.defName} has a misconfigured DefModExt_SubCategoryBuilding, subCategory MUST be assigned.");
                    }
                    else
                    {
                        modExt.subCategory.enabled = true;
                    }
                }
            }
        }

        public static void FixRoyaltyBullshit(TabulaRasaSettings settings)
        {
            if (settings.empireHostilityFixedFactions.NullOrEmpty())
            {
                settings.empireHostilityFixedFactions = new Dictionary<string, bool>();
            }
            foreach (FactionDef faction in DefDatabase<FactionDef>.AllDefs.Where(f => f.isPlayer))
            {
                if (!settings.empireHostilityFixedFactions.ContainsKey(faction.defName))
                {
                    settings.empireHostilityFixedFactions.Add(faction.defName, true);
                }
            }

            FactionDef empireDef = DefDatabase<FactionDef>.GetNamedSilentFail("Empire");
            if(empireDef != null)
            {
                foreach (FactionDef fac in DefDatabase<FactionDef>.AllDefs.Where(d => d.isPlayer))
                {
                    if (!empireDef.permanentEnemyToEveryoneExcept.Contains(fac))
                    {
                        empireDef.permanentEnemyToEveryoneExcept.Add(fac);
                    }
                }
            }
        }

        //public static void RunCheckForIntelligentPawns()
        //{
        //    if (DefDatabase<ThingDef>.AllDefs.Any(p => p.thingClass == typeof(Pawn_IntelligentAnimal)))
        //    {
        //    }
        //}

        //public static void DisableCorpseRottingAsNeeded()
        //{
        //    foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
        //    {
        //        DefModExt_ArtificialPawn modExt = thingDef.GetModExtension<DefModExt_ArtificialPawn>();
        //        if (modExt != null)
        //        {
        //            ThingDef corpseDef = thingDef?.race?.corpseDef;
        //            if (corpseDef != null)
        //            {
        //                if (!modExt.corpseRots)
        //                {
        //                    corpseDef.comps.RemoveAll(compProperties => compProperties is CompProperties_Rottable);
        //                    corpseDef.comps.RemoveAll(compProperties => compProperties is CompProperties_SpawnerFilth);
        //                }
        //                if (!modExt.corpseEdible)
        //                {
        //                    if (corpseDef.modExtensions == null)
        //                    {
        //                        corpseDef.modExtensions = new List<DefModExtension>();
        //                    }
        //                    corpseDef.modExtensions.Add(new DefModExt_ArtificialPawn() { corpseEdible = false });
        //                }
        //            }
        //        }
        //    }
        //}

        //public static void FillRaceAlternatesAutomatically()
        //{
        //    foreach(RaceSpawningDef rsd in DefDatabase<RaceSpawningDef>.AllDefs)
        //    {
        //        if (CheckRaceSpawningDefForFlaws(rsd))
        //        {
        //            CheckIfSettingsExistAndFix(rsd);
        //            if (DealWithRaceSpawningSettings(rsd))
        //            {
        //                DistributeRaceAmongFactionKinds(rsd);
        //            }
        //        }
        //    }
        //}

        public static void CheckIfSettingsExistAndFix(RaceSpawningDef rsd)
        {
            if (TabulaRasaMod.settings.raceSpawningSettings.NullOrEmpty())
            {
                TabulaRasaMod.settings.raceSpawningSettings = new Dictionary<string, bool>();
            }
            if (!TabulaRasaMod.settings.raceSpawningSettings.ContainsKey(rsd.defName))
            {
                TabulaRasaMod.settings.raceSpawningSettings.Add(rsd.defName, true);
            }

            if (TabulaRasaMod.settings.raceSpawningWeights.NullOrEmpty())
            {
                TabulaRasaMod.settings.raceSpawningWeights = new Dictionary<string, float>();
            }
            if (!TabulaRasaMod.settings.raceSpawningWeights.ContainsKey(rsd.defName))
            {
                TabulaRasaMod.settings.raceSpawningWeights.Add(rsd.defName, rsd.weight);
            }
        }

        public static bool DealWithRaceSpawningSettings(RaceSpawningDef rsd)
        {
            bool enabled = true;
            if(TabulaRasaMod.settings.raceSpawningSettings.ContainsKey(rsd.defName))
            {
                enabled = TabulaRasaMod.settings.raceSpawningSettings.TryGetValue(rsd.defName);
            }
            if (TabulaRasaMod.settings.raceSpawningWeights.ContainsKey(rsd.defName))
            {
                rsd.weight = TabulaRasaMod.settings.raceSpawningWeights.TryGetValue(rsd.defName);
            }
            return enabled;
        }

        //public static void DistributeRaceAmongFactionKinds(RaceSpawningDef rsd)
        //{
        //    List<PawnKindDef> viableKinds = rsd.pawnKinds;

        //    for (int i = 0; i < viableKinds.Count(); i++)
        //    {
        //        try
        //        {
        //            LogUtil.LogDebug($"Patching races into {viableKinds[i].defName}...");
        //            PawnKindDef curPkd = viableKinds[i];
        //            if (!curPkd.HasModExtension<DefModExt_PawnKindRaces>())
        //            {
        //                if (curPkd.modExtensions.NullOrEmpty())
        //                {
        //                    curPkd.modExtensions = new List<DefModExtension>();
        //                }
        //                curPkd.modExtensions.Add(new DefModExt_PawnKindRaces());
        //            }
        //            for (int j = 0; j < rsd.races.Count(); j++)
        //            {
        //                DefModExt_PawnKindRaces modExt = curPkd.GetModExtension<DefModExt_PawnKindRaces>();
        //                if (modExt.altRaces.NullOrEmpty())
        //                {
        //                    modExt.altRaces = new List<WeightedRaceChoice>();
        //                }
        //                if (!modExt.altRaces.Any(ar => ar.race == rsd.races[j]))
        //                {
        //                    WeightedRaceChoice weightedRace = new WeightedRaceChoice(rsd.races[j], rsd.weight);
        //                    modExt.altRaces.Add(weightedRace);
        //                    LogUtil.LogDebug($"- {weightedRace.race} : {weightedRace.weight}");
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogWarning($"Exception caught in {viableKinds[i].defName} while trying to distribute races among pawnKinds.\n\n{ex}");
        //        }
        //    }
        //}

        //public static bool CheckRaceSpawningDefForFlaws(RaceSpawningDef rsd)
        //{
        //    if (rsd.races.NullOrEmpty())
        //    {
        //        LogUtil.LogWarning($"RaceSpawning Def {rsd.defName} has no races provided, skipping...");
        //        return false;
        //    }
        //    if (rsd.pawnKinds.NullOrEmpty())
        //    {
        //        LogUtil.LogWarning($"RaceSpawning Def {rsd.defName} has no races provided, skipping...");
        //        return false;
        //    }
        //    return true;
        //}

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
