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

    [HarmonyPatch(typeof(HealthAIUtility), "FindBestMedicine")]
    public static class HealthAIUtility_FindBestMedicine
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn healer, Pawn patient, ref Thing __result)
        {
            if (__result != null)
            {
                Comp_UseHealthPack healthPackComp = __result.TryGetComp<Comp_UseHealthPack>();
                if (healthPackComp != null)
                {
                    if (!HealthUtil.CanSealWounds(patient))
                    {
                        return;
                    }
                    Predicate<Thing> validator = (Thing m) => !m.IsForbidden(healer) && patient.playerSettings.medCare.AllowsMedicine(m.def) && healer.CanReserve(m, 10, 1, null, false) && m.TryGetComp<Comp_UseHealthPack>() != null;
                    Func<Thing, float> priorityGetter = (Thing t) => t.def.GetStatValueAbstract(StatDefOf.MedicalPotency, null);
                    __result = GenClosest.ClosestThing_Global_Reachable(patient.Position, patient.Map, patient.Map.listerThings.ThingsInGroup(ThingRequestGroup.Medicine), PathEndMode.ClosestTouch, TraverseParms.For(healer, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, priorityGetter);
                }
            }
        }
    }
}
