using HarmonyLib;
using RimWorld;
using Verse;

namespace HRF
{
    public class Verb_Graveblossom : Verb_AreaEffect
    {
        protected override void AffectCell(IntVec3 cell)
        {
            if (cell.GetFirstThing<Plant>(caster.Map) == null) return;
            var thing = (Graveblossom) ThingMaker.MakeThing(ThingDef.Named("Graveblossom"));
            thing.PlaceTick = Find.TickManager.TicksGame;
            GenSpawn.Spawn(thing, cell, caster.Map);
        }
    }

    public static class GraveblossomHelpers
    {
        public static void DoPatches(Harmony harm)
        {
            harm.Patch(AccessTools.Method(typeof(Plant), "get_GrowthRateFactor_Light"),
                postfix: new HarmonyMethod(typeof(GraveblossomHelpers), "GrowthRateFactor_Light_Postfix"));
            harm.Patch(AccessTools.Method(typeof(Plant), "get_Resting"),
                postfix: new HarmonyMethod(typeof(GraveblossomHelpers), "Resting_Postfix"));
        }

        public static void GrowthRateFactor_Light_Postfix(ref float __result, Plant __instance)
        {
            if (__instance.Position.GetFirstThing<Graveblossom>(__instance.Map) != null) __result = 1f;
        }

        public static void Resting_Postfix(ref bool __result, Plant __instance)
        {
            if (__instance.Position.GetFirstThing<Graveblossom>(__instance.Map) != null) __result = false;
        }
    }

    public class Graveblossom : Thing
    {
        public int PlaceTick;

        public override void TickLong()
        {
            if (PlaceTick + 180000 <= Find.TickManager.TicksGame) Destroy();
        }
    }
}