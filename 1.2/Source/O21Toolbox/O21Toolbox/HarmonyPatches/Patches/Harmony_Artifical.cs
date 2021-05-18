using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.Needs;

namespace O21Toolbox.HarmonyPatches
{
    //[HarmonyPatch(typeof(RaceProperties), "IsFlesh", MethodType.Getter)]
    //public class Patch_IsFlesh_Get
    //{
    //    [HarmonyPostfix]
    //    public static void Postfix(RaceProperties __instance, ref bool __result)
    //    {
    //        if (__instance.FleshType.IsArtificialPawn())
    //        {
    //            __result = false;
    //        }
    //    }
    //}

    [HarmonyPatch(typeof(AddictionUtility), "CheckDrugAddictionTeachOpportunity")]
    public class Patch_AddictionUtility_CheckDrugAddictionTeachOpportunity
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn pawn)
        {
            if (pawn.def.race.FleshType.IsArtificialPawn())
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(FoodUtility), "IsAcceptablePreyFor")]
    public class Patch_FoodUtility_IsAcceptablePreyFor
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, Pawn predator, Pawn prey)
        {
            if (prey.def.race.FleshType.IsArtificialPawn())
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(IncidentWorker_Disease), "PotentialVictims")]
    public class Patch_IncidentWorker_Disease_PotentialVictims
    {
        [HarmonyPostfix]
        public static void Postfix(ref IEnumerable<Pawn> __result, IIncidentTarget target)
        {
            __result = __result.Where(delegate (Pawn p)
            {
                if (p.RaceProps.FleshType.IsArtificialPawn())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            });
        }
    }

    [HarmonyPatch(typeof(ImmunityHandler), "DiseaseContractChanceFactor")]
    [HarmonyPatch(new Type[] { typeof(HediffDef), typeof(BodyPartRecord) })]
    public class Patch_ImmunityHandler_DiseaseContractChanceFactor
    {
        [HarmonyPostfix]
        public static void Postfix(ImmunityHandler __instance, float __result)
        {
            if (__result > 0f)
            {
                if (__instance.pawn is Pawn && __instance.pawn.def.HasModExtension<ArtificialPawnProperties>())
                {
                    __result = 0f;
                }
            }
        }
    }

    [HarmonyPatch(typeof(StunHandler), "Notify_DamageApplied")]
    [HarmonyPatch(new Type[] { typeof(DamageInfo), typeof(bool) })]
    public class Notify_DamageApplied_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(DamageInfo dinfo, ref bool affectedByEMP, Thing ___parent)
        {
            if (___parent is Pawn && ___parent.def.HasModExtension<ArtificialPawnProperties>())
            {
                Pawn pawn = (Pawn)___parent;
                if (pawn.def.GetModExtension<ArtificialPawnProperties>().affectedByEMP)
                {
                    affectedByEMP = true;
                }
            }
        }
    }

    [HarmonyPatch(typeof(StunHandler), "AffectedByEMP", MethodType.Getter)]
    public class Patch_StunHandler_AffectedByEMP
    {
        [HarmonyPostfix]
        public static void Postfix(ref bool __result, ref StunHandler __instance, Thing ___parent)
        {
            if(___parent is Pawn) 
            {
                Pawn pawn = (Pawn)___parent;
                if (pawn != null && pawn.def.HasModExtension<ArtificialPawnProperties>())
                {
                    if (pawn.def.GetModExtension<ArtificialPawnProperties>().affectedByEMP)
                    {
                        if (pawn.apparel != null && pawn.apparel.WornApparel.Any(a => a.def.HasModExtension<DefModExt_EMPShielding>()))
                        {
                            __result = false;
                            return;
                        }
                        if ((pawn.health?.hediffSet?.hediffs?.Any(h => h.def.HasModExtension<DefModExt_EMPShielding>()) ?? false))
                        {
                            __result = false;
                            return;
                        }
                        __result = true;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(SkillRecord), "Interval")]
    public class Patch_SkillRecord_Interval
    {
        [HarmonyPrefix]
        public static bool Prefix(SkillRecord __instance)
        {
            Pawn pawn = (Pawn)__instance.pawn;
            ArtificialPawnProperties modExt;
            bool flag = (modExt = pawn.def.GetModExtension<ArtificialPawnProperties>()) != null && modExt.noSkillLoss;
            return !flag;
        }
    }
}
