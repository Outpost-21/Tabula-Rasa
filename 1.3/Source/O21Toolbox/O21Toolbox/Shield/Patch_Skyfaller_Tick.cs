using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

using HarmonyLib;

namespace O21Toolbox.Shield
{
    [HarmonyPatch(typeof(Skyfaller), "Tick")]
    public class Patch_Skyfaller_Tick
    {
        [HarmonyPrefix]
        public static bool Prefix(Skyfaller __instance)
        {
            if (__instance.Map != null && __instance.ticksToImpact <= 20)
            {
                //Log.Message("Checking Skyfaller against shields.");
                List<ThingWithComps> list = __instance.Map.GetComponent<MapComp_ShieldList>().shieldGenList;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].TryGetComp<Comp_ShieldBuilding>().CheckIntercept(__instance))
                    {
                        SoundDef impactSound = DefDatabase<SoundDef>.GetNamed("Explosion_EMP");
                        impactSound.PlayOneShot(new TargetInfo(__instance.Position, __instance.Map, false));
                        foreach (IntVec3 cell in __instance.OccupiedRect().ToList())
                        {
                            MoteMaker.MakeStaticMote(cell, __instance.Map, DefDatabase<ThingDef>.GetNamed("Mote_ElectricalSpark"));
                            MoteMaker.MakeStaticMote(cell, __instance.Map, DefDatabase<ThingDef>.GetNamed("Mote_PsycastPsychicEffect"));
                        }
                        __instance.Destroy(DestroyMode.Vanish);
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
