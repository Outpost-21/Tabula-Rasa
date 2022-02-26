using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;
using JetBrains.Annotations;
using O21Toolbox.ApparelExt;
using O21Toolbox.Drones;
using O21Toolbox.PawnKindExt;
using O21Toolbox.SlotLoadable;

namespace O21Toolbox.HarmonyPatches.Patches
{
    public class Harmony_PawnGeneration
    {
    }

    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public class Patch_PawnGenerator_GeneratePawn
    {
        [HarmonyPostfix]
        public static void Postfix(PawnGenerationRequest request, Pawn __result)
        {
            Pawn pawn = __result;
            DefModExt_ExtendedPawnKind modExt = pawn.kindDef.GetModExtension<DefModExt_ExtendedPawnKind>();
            if (modExt != null && modExt.clearApparel)
            {
                for (int i = 0; i < pawn.apparel.WornApparel.Count(); i++)
                {
                    pawn.apparel.Remove(pawn.apparel.WornApparel[i]);
                }
            }
        }
    }

    [HarmonyPatch(typeof(PawnGenerator), "GenerateInitialHediffs")]
    public static class Patch_PawnGen_GenerateInitialHediffs
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, PawnGenerationRequest request)
        {
            DefModExt_ExtendedPawnKind modExt = pawn.kindDef.GetModExtension<DefModExt_ExtendedPawnKind>();

            if (modExt != null && !pawn.Dead)
            {
                if (!modExt.additionalHediffs.NullOrEmpty())
                {
                    if (modExt.randomAdditionalHediff) 
                    {
                        AdditionalHediffEntry result = GetHediffEntry(pawn.kindDef);
                        if (result != null)
                        {
                            GiveHediffEntry(result, pawn);
                        }
                    }
                    else
                    {
                        foreach(AdditionalHediffEntry entry in modExt.additionalHediffs)
                        {
                            GiveHediffEntry(entry, pawn);
                        }
                    }
                }

                List<Hediff> hediffs = pawn.health.hediffSet.hediffs;

                if (modExt.clearChronicIllness)
                {
                    for (int i = 0; i < hediffs.Count(); i++)
                    {
                        if (hediffs[i].def.chronic)
                        {
                            pawn.health.RemoveHediff(hediffs[i]);
                        }
                    }
                }


                if (modExt.clearAddictions)
                {
                    for (int i = 0; i < hediffs.Count(); i++)
                    {
                        if (hediffs[i].def.IsAddiction)
                        {
                            pawn.health.RemoveHediff(hediffs[i]);
                        }
                    }
                }

                if (modExt.replaceMissingParts)
                {
                    for (int i = 0; i < hediffs.Count(); i++)
                    {
                        if (pawn.health.hediffSet.PartIsMissing(hediffs[i].part))
                        {
                            pawn.health.RestorePart(hediffs[i].part);
                        }
                    }
                }
            }
        }

        public static void GiveHediffEntry(AdditionalHediffEntry entry, Pawn pawn)
        {
            Hediff hediff = HediffMaker.MakeHediff(entry.hediff, pawn);
            hediff.Severity = entry.severityRange.RandomInRange;
            pawn.health.AddHediff(hediff);
        }

        public static AdditionalHediffEntry GetHediffEntry(PawnKindDef pawnkind)
        {
            if (pawnkind.HasModExtension<DefModExt_ExtendedPawnKind>())
            {
                DefModExt_ExtendedPawnKind modExt = pawnkind.GetModExtension<DefModExt_ExtendedPawnKind>();
                AdditionalHediffEntry resultEntry;
                if (!modExt.additionalHediffs.NullOrEmpty())
                {
                    Func<AdditionalHediffEntry, float> selector = (AdditionalHediffEntry x) => x.weight;
                    resultEntry = modExt.additionalHediffs.RandomElementByWeight(selector);

                    return resultEntry;
                }
            }

            return null;
        }
    }

    [HarmonyPatch(typeof(PawnGenerator), "FinalLevelOfSkill")]
	public static class Patch_PawnGen_FinalLevelOfSkill
	{
		[HarmonyPostfix]
		public static void Postfix(ref int __result, Pawn pawn, SkillDef sk)
        {
            DefModExt_ExtendedPawnKind modExt = pawn.kindDef.GetModExtension<DefModExt_ExtendedPawnKind>();

            if(modExt != null)
            {
                if (!modExt.skillSettings.NullOrEmpty())
                {
                    if(!modExt.skillSettings.Where(sr => sr.skill == sk).EnumerableNullOrEmpty())
                    {
                        __result = modExt.skillSettings.Find(sr => sr.skill == sk).level;
                    }
                    else if (modExt.flattenSkills)
                    {
                        __result = 0;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PawnGenerator), "GenerateSkills")]
    public static class Patch_PawnGen_GenerateSkills
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn)
        {
            DefModExt_ExtendedPawnKind modExt = pawn.kindDef.GetModExtension<DefModExt_ExtendedPawnKind>();

            if (modExt != null)
            {
                if (modExt.clearPassions)
                {
                    for (int i = 0; i < pawn.skills.skills.Count(); i++)
                    {
                        pawn.skills.skills[i].passion = Passion.None;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PawnGenerator), "TryGenerateNewPawnInternal")]
    public static class Patch_PawnGen_TryGenerateNewPawnInternal
    {
        [HarmonyPrefix]
        static void Prefix(ref PawnGenerationRequest request)
        {
            PawnKindDef pawnKindDef = request.KindDef;
            if (Faction.OfPlayerSilentFail != null && pawnKindDef == PawnKindDefOf.Villager)
            {
                Faction faction = request.Faction;
                if (faction != null && faction.IsPlayer)
                {
                    ThingDef race = pawnKindDef.race;
                    Faction ofPlayer = Faction.OfPlayer;
                    if (race != ((ofPlayer != null) ? ofPlayer.def.basicMemberKind.race : null))
                    {
                        Faction ofPlayer2 = Faction.OfPlayer;
                        pawnKindDef = ((ofPlayer2 != null) ? ofPlayer2.def.basicMemberKind : null);
                    }
                }
            }
            IEnumerable<AltRaceForFactions> allDefsListForReading = DefDatabase<AltRaceForFactions>.AllDefsListForReading;
            request.KindDef = pawnKindDef;
        }

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo raceDef = AccessTools.Field(typeof(PawnKindDef), nameof(PawnKindDef.race));
            //MethodInfo pawnRequest = AccessTools.PropertyGetter(typeof(PawnGenerationRequest), "Faction");

            var codes = new List<CodeInstruction>(instructions);
            foreach(CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.OperandIs(raceDef))
                {
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = AccessTools.Method(typeof(Patch_PawnGen_TryGenerateNewPawnInternal), nameof(GetAltRaceDef));
                }
                yield return instruction;
            }
        }

        public static ThingDef GetAltRaceDef(PawnKindDef pawnkind)
        {
            ThingDef result = pawnkind.race;

            DefModExt_ExtendedPawnKind modExt = null;
            if (pawnkind.HasModExtension<DefModExt_ExtendedPawnKind>())
            {
                modExt = pawnkind.GetModExtension<DefModExt_ExtendedPawnKind>();
            }

            //Dictionary<ThingDef, float> possibleRaces = new Dictionary<ThingDef, float>();
            // Add base race value.
            //possibleRaces.Add(pawnkind.race, O21ToolboxMod.settings.humanSpawnWeight);

            // Pull all potential races for the current request if any match the faction.
            //if (faction != null && (modExt != null || !modExt.overrideFactionRaces))
            //{
            //    List<AltRaceForFactions> allDefsListForReading = DefDatabase<AltRaceForFactions>.AllDefsListForReading;
            //    if (!allDefsListForReading.NullOrEmpty())
            //    {
            //        for (int i = 0; i < allDefsListForReading.Count(); i++)
            //        {
            //            for (int j = 0; j < allDefsListForReading[i].altRaceOptions.Count(); j++)
            //            {
            //                AltRaceOption option = allDefsListForReading[i].altRaceOptions[j];
            //                if (option.faction == faction.def && !possibleRaces.Keys.Contains(option.raceDef))
            //                {
            //                    possibleRaces.Add(option.raceDef, option.weight);
            //                }
            //            }
            //        }
            //    }
            //}
            // Also pull anything from the pawnKinds if they're not already covered.
            //if (modExt != null)
            //{
            //    ThingDefEntry resultEntry;
            //    if (!modExt.altRaces.NullOrEmpty())
            //    {
            //        for (int i = 0; i < modExt.altRaces.Count(); i++)
            //        {
            //            for (int j = 0; j < modExt.altRaces[i].races.Count(); j++)
            //            {
            //                ThingDef def = modExt.altRaces[i].races[j];
            //                if(!possibleRaces.ContainsKey(def))
            //                {
            //                    possibleRaces.Add(def, modExt.altRaces[i].weight);
            //                }
            //            }
            //        }
            //    }
            //}

            // Grab a weighted result from the list.
            //if (!possibleRaces.NullOrEmpty())
            //{
            //    List<Pair<ThingDef, float>> options = new List<Pair<ThingDef, float>>();
            //    foreach (KeyValuePair<ThingDef, float> pair in possibleRaces)
            //    {
            //        options.Add(new Pair<ThingDef, float>(pair.Key, pair.Value));
            //    }


            //    Func<Pair<ThingDef, float>, float> selector = (Pair<ThingDef, float> x) => x.second;
            //    result = options.RandomElementByWeight(selector).first;
            //}


            if (modExt != null)
            {
                ThingDefEntry resultEntry;
                if (!modExt.altRaces.NullOrEmpty())
                {
                    float defaultWeight = 20f;
                    if (pawnkind.race == PawnKindDefOf.Colonist.race)
                    {
                        defaultWeight = O21ToolboxMod.settings.humanSpawnWeight;
                    }
                    modExt.altRaces.Add(new ThingDefEntry()
                    {
                        races = new List<ThingDef>() { pawnkind.race },
                        weight = defaultWeight
                    });
                    Func<ThingDefEntry, float> selector = (ThingDefEntry x) => x.weight;
                    resultEntry = modExt.altRaces.RandomElementByWeight(selector);

                    return resultEntry.races.RandomElement();
                }
            }
            return result;
        }
    }

    [HarmonyPatch(typeof(PawnGenerator), "GenerateGearFor")]
    public static class Patch_PawnGen_GenerateGearFor
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, PawnGenerationRequest request)
        {
            CheckSlotLoadables(pawn);
        }

        public static void CheckSlotLoadables(Pawn pawn)
        {
            if (pawn.kindDef.HasModExtension<DefModExt_ExtendedPawnKind>())
            {
                DefModExt_ExtendedPawnKind modExt = pawn.kindDef.GetModExtension<DefModExt_ExtendedPawnKind>();
                if (modExt.slottableWeapon)
                {
                    List<ThingWithComps> list = pawn.equipment.AllEquipmentListForReading;
                    for (int i = 0; i < list.Count; i++)
                    {
                        ThingWithComps equipment = list[i];
                        Comp_SlotLoadable comp = equipment.TryGetComp<Comp_SlotLoadable>();
                        if(comp != null)
                        {
                            for (int j = 0; j < modExt.slottableRestrictions.Count; j++)
                            {
                                if (!comp.SlotDefs.NullOrEmpty() && comp.SlotDefs.Contains(modExt.slottableRestrictions[j].slotLoadableDef))
                                {
                                    Thing slottable = ThingMaker.MakeThing(modExt.slottableRestrictions[j].slottableThingDefs.RandomElement());
                                    comp.Slots.Find(slot => slot.def == modExt.slottableRestrictions[j].slotLoadableDef).TryLoadSlot(slottable);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
