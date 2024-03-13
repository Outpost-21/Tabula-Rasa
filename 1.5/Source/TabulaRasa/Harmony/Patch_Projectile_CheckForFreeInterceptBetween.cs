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

namespace TabulaRasa
{
    [HarmonyPatch(typeof(Projectile), "CheckForFreeInterceptBetween")]
    public class Patch_Projectile_CheckForFreeInterceptBetween
    {
        [HarmonyPostfix]
        public static void Postfix(Projectile __instance, ref bool __result, Vector3 lastExactPos, Vector3 newExactPos)
        {
            if (__result == false)
            {
                List<ThingWithComps> list = __instance.Map.GetComponent<MapComp_ShieldList>().shieldGenList;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].TryGetComp<Comp_Shield>().CheckIntercept(__instance, lastExactPos, newExactPos))
                    {
                        __instance.Destroy(DestroyMode.Vanish);
                        __result = true;
                        break;
                    }
                }
            }
        }
    }
}
