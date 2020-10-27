using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Sound;

namespace O21Toolbox.HarmonyPatches.Patches
{
    public class Harmony_Sounds
    {
    }

    [HarmonyPatch(typeof(Verb_MeleeAttack), "SoundMiss")]
    public static class Patch_Verb_MeleeAttack_SoundMiss
    {
        [HarmonyPrefix]
        public static void SoundMissPrefix(ref SoundDef __result, Verb_MeleeAttack __instance)
        {
            if (__instance.caster is Pawn pawn)
            {
                var pawn_EquipmentTracker = pawn.equipment;
                if (pawn_EquipmentTracker != null)
                {
                    //Log.Message("2");
                    var thingWithComps =
                        pawn_EquipmentTracker
                            .Primary; // (ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);

                    if (thingWithComps != null)
                    {
                        //Log.Message("3");
                        var CompExtraSounds = thingWithComps.GetComp<Comp_ExtraSounds>();
                        if (CompExtraSounds != null)
                            if (CompExtraSounds.Props.soundMiss != null)
                                __result = CompExtraSounds.Props.soundMiss;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Verb_MeleeAttack), "SoundHitPawn")]
    public static class Patch_Verb_MeleeAttack_SoundHitPawn
    {
        [HarmonyPrefix]
        public static void SoundHitPawnPrefix(ref SoundDef __result, Verb_MeleeAttack __instance)
        {
            if (__instance.caster is Pawn pawn)
            {
                var pawn_PawnKindDef = pawn.kindDef;
                if (pawn_PawnKindDef != null)
                {
                    var extraSoundsExtension = pawn_PawnKindDef.GetModExtension<DefModExt_ExtraSounds>();
                    if (extraSoundsExtension != null)
                    {
                        if (extraSoundsExtension != null)
                            if (extraSoundsExtension.soundHitPawn != null)
                                __result = extraSoundsExtension.soundHitPawn;
                    }
                }

                var pawn_EquipmentTracker = pawn.equipment;
                if (pawn_EquipmentTracker != null)
                {
                    //Log.Message("2");
                    var thingWithComps =
                        pawn_EquipmentTracker
                            .Primary; // (ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);

                    if (thingWithComps != null)
                    {
                        //Log.Message("3");
                        var CompExtraSounds = thingWithComps.GetComp<Comp_ExtraSounds>();

                        if (CompExtraSounds != null)
                            if (CompExtraSounds.Props.soundHitPawn != null)
                                __result = CompExtraSounds.Props.soundHitPawn;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Verb_MeleeAttack), "SoundHitBuilding")]
    public static class Patch_Verb_MeleeAttack_SoundHitBuilding
    {
        [HarmonyPrefix]
        public static void SoundHitBuildingPrefix(ref SoundDef __result, Verb_MeleeAttack __instance)
        {
            if (__instance.caster is Pawn pawn)
            {
                var pawn_EquipmentTracker = pawn.equipment;
                if (pawn_EquipmentTracker != null)
                {
                    //Log.Message("2");
                    var thingWithComps =
                        pawn_EquipmentTracker
                            .Primary; // (ThingWithComps)AccessTools.Field(typeof(Pawn_EquipmentTracker), "primaryInt").GetValue(pawn_EquipmentTracker);

                    if (thingWithComps != null)
                    {
                        //Log.Message("3");
                        var CompExtraSounds = thingWithComps.GetComp<Comp_ExtraSounds>();
                        if (CompExtraSounds != null)
                            if (CompExtraSounds.Props.soundHitBuilding != null)
                            {
                                __result = CompExtraSounds.Props.soundHitBuilding;
                            }
                    }
                }
            }
        }
    }
}
