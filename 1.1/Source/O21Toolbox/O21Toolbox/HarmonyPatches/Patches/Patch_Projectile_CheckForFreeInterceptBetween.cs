using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Shield;

namespace O21Toolbox.HarmonyPatches.Patches
{
    [HarmonyPatch(typeof(Projectile), "CheckForFreeInterceptBetween")]
    public class Patch_Projectile_CheckForFreeInterceptBetween
    {
        // Token: 0x0600037B RID: 891 RVA: 0x00025AE4 File Offset: 0x00023CE4
        [HarmonyPostfix]
        public static void Postfix(Projectile __instance, ref bool __result, Vector3 lastExactPos, Vector3 newExactPos)
        {
            if(__result == false)
            {
                List<Thing> list = __instance.Map.listerThings.AllThings.Where(t => t.def.HasComp(typeof(Comp_ShieldBuilding))).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].TryGetComp<Comp_ShieldBuilding>().CheckIntercept(__instance, lastExactPos, newExactPos))
                    {
                        __instance.Destroy(DestroyMode.Vanish);
                        __result = true;
                    }
                }
            }
        }
    }
}
