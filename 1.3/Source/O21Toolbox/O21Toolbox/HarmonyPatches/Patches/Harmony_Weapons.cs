using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Utility;
using O21Toolbox.WeaponExt;

namespace O21Toolbox.HarmonyPatches.Patches
{
    [StaticConstructorOnStartup]
    public class Harmony_Weapons
    {
        [HarmonyPatch(typeof(CompEquippable), "get_PrimaryVerb")]
        public static class CompEquippable_GetPrimaryVerb_PostFix
        {
            [HarmonyPostfix]
            static void Postfix(CompEquippable __instance, ref Verb __result)
            {
                if (__instance.parent.TryGetComp<Comp_VerbSwitch>() != null)
                {
                    __result.verbProps = __instance.parent.TryGetComp<Comp_VerbSwitch>().Active;
                }
            }
        }

        [HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
        public static class Pawn_GetGizmos_Postfix
        {
            [HarmonyPostfix]
            static void PostFix(Pawn __instance, ref IEnumerable<Gizmo> __result)
            {
                Pawn_EquipmentTracker pawn_EquipmentTracker = __instance.equipment;
                if (pawn_EquipmentTracker != null)
                {
                    ThingWithComps thingWithComps = pawn_EquipmentTracker.Primary;
                    if (thingWithComps != null)
                    {
                        Comp_VerbSwitch verbSwitchComp = thingWithComps.GetComp<Comp_VerbSwitch>();
                        if (verbSwitchComp != null && Get_Gizmos(verbSwitchComp).Count() > 0 && __instance != null && __instance.Faction == Faction.OfPlayer)
                        {
                            __result = __result.Concat(Get_Gizmos(verbSwitchComp));
                        }
                    }
                }
            }

            public static IEnumerable<Gizmo> Get_Gizmos(Comp_VerbSwitch verbSwitchComp)
            {
                var enumerator = verbSwitchComp.VerbSwitchGizmos().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    yield return current;
                }
            }
        }
    }
}
