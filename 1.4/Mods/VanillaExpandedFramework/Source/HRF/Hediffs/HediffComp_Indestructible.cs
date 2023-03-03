using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Verse;

namespace HRF
{
    public class HediffComp_Indestructible : HediffComp
    {
    }

    public static class IndestructibleHediffs
    {
        public static void DoPatches(Harmony harm)
        {
            harm.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "CalculateMissingPartHediffsFromInjury"),
                postfix: new HarmonyMethod(typeof(IndestructibleHediffs), "RemoveIndestructibleParts"));
            harm.Patch(AccessTools.Method(typeof(HediffSet), "GetPartHealth"),
                postfix: new HarmonyMethod(typeof(IndestructibleHediffs), "FixPartHealth"));
        }

        public static void RemoveIndestructibleParts(ref HashSet<Hediff> __result, Pawn_HealthTracker __instance)
        {
            if (__result == null) return;
            var toRemove = new HashSet<Hediff>();
            foreach (var hediff in from hediff in __result
                from hediff1 in __instance.hediffSet.hediffs.Where(h =>
                    h.Part == hediff.Part || hediff.Part.GetAllParentParts().Contains(h.Part) || h.Part == null)
                where hediff1.TryGetComp<HediffComp_Indestructible>() != null
                select hediff)
            {
                toRemove.Add(hediff);
                var childParts = hediff.Part.GetAllChildParts().ToList();
                toRemove.AddRange(__result.Where(hd => childParts.Contains(hd.Part)));
            }

            __result.RemoveWhere(hd => toRemove.Contains(hd));
        }

        public static void FixPartHealth(ref float __result, HediffSet __instance, BodyPartRecord part)
        {
            var parents = part.GetAllParentParts().ToList();
            if (__result == 0f &&
                __instance.hediffs.Any(hd =>
                    (hd.Part == part || parents.Contains(hd.Part) || hd.Part == null) &&
                    hd.TryGetComp<HediffComp_Indestructible>() != null))
                __result = 1f;
        }

        public static IEnumerable<BodyPartRecord> GetAllChildParts(this BodyPartRecord record)
        {
            var parts = new List<BodyPartRecord> {record};
            foreach (var childPart in record.GetDirectChildParts()) parts.AddRange(childPart.GetAllChildParts());
            return parts;
        }

        public static IEnumerable<BodyPartRecord> GetAllParentParts(this BodyPartRecord record)
        {
            var part = record.parent;
            while (part != null)
            {
                yield return part;
                part = part.parent;
            }
        }
    }
}