using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

using HarmonyLib;
using Verse.Sound;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(Skyfaller), "Tick")]
    public class Patch_Skyfaller_Tick
    {
        [HarmonyPrefix]
        public static bool Prefix(Skyfaller __instance)
        {
            if (__instance.Map != null && __instance.ticksToImpact == 20)
            {
                if(__instance.Faction?.HostileTo(Faction.OfPlayer) ?? true || !__instance.innerContainer.NullOrEmpty() && __instance.innerContainer.Any(t => t.Faction?.HostileTo(Faction.OfPlayer) ?? true))
                {
                    List<ThingWithComps> list = __instance.Map.GetComponent<MapComp_ShieldList>().shieldGenList;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].TryGetComp<Comp_Shield>().CheckIntercept(__instance))
                        {
                            SoundDef impactSound = SoundDefOf.EnergyShield_AbsorbDamage;
                            impactSound.PlayOneShot(new TargetInfo(__instance.Position, __instance.Map, false));
                            foreach (IntVec3 cell in __instance.OccupiedRect().ToList())
                            {
                                FleckMaker.ThrowHeatGlow(cell, __instance.Map, 1f);
                                FleckMaker.ThrowLightningGlow(cell.ToVector3Shifted(), __instance.Map, 1f);
                                FleckMaker.Static(cell, __instance.Map, DefDatabase<FleckDef>.GetNamed("ElectricalSpark"), 2f);
                                FleckMaker.Static(cell, __instance.Map, DefDatabase<FleckDef>.GetNamed("PsycastPsychicEffect"), 2f);
                            }
                            __instance.Destroy(DestroyMode.KillFinalize);
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
