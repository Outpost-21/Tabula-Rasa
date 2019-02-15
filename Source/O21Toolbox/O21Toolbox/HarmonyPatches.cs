using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Harmony;
using UnityEngine;
using RimWorld;
using Verse;
using System.Reflection;
using O21Toolbox.ResearchBenchSub;
using O21Toolbox.Needs;

namespace O21Toolbox
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        /// <summary>
        /// For internal use: Field for getting the pawn.
        /// </summary>
        public static FieldInfo int_Pawn_NeedsTracker_GetPawn;

        public static NeedDef Need_Bladder;
        public static NeedDef Need_Hygiene;

        static HarmonyPatches()
        {
            HarmonyInstance O21ToolboxHarmony = HarmonyInstance.Create("com.o21toolbox.rimworld.mod");

            //Patch, Method: Pawn_NeedsTracker
            {
                Type type = typeof(Pawn_NeedsTracker);

                //Get private variable 'pawn' from 'Pawn_NeedsTracker'.
                int_Pawn_NeedsTracker_GetPawn = type.GetField("pawn", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

                //Patch: Pawn_NeedsTracker.ShouldHaveNeed as Postfix
                O21ToolboxHarmony.Patch(
                    type.GetMethod("ShouldHaveNeed", BindingFlags.NonPublic | BindingFlags.Instance),
                    null,
                    new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_Pawn_NeedsTracker_ShouldHaveNeed))));
            }

            O21ToolboxHarmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        // Patches

        /// <summary>
        /// Patch for enabling comped research benches to act like they either are another bench, or act like they have a specific facility attached.
        /// </summary>
        /**
        [HarmonyPatch(typeof(ResearchProjectDef))]
        [HarmonyPatch(nameof(ResearchProjectDef.CanBeResearchedAt))]
        static class ResearchProjectDef_CanBeResearchedAt_Patch
        {
            static bool Prefix(ref bool __result, ref Building_ResearchBench bench, ResearchProjectDef ___instance)
            {
                // Only do anything if the result is false and the bench has the new comp.
                if (bench.TryGetComp<Comp_ResearchBenchSubstitutes>() != null && !__result)
                {
                    // Does the research have a required building?
                    if(___instance.requiredResearchBuilding != null)
                    {
                        if (bench.TryGetComp<Comp_ResearchBenchSubstitutes>().Props.ActLikeResearchBench.Contains(___instance.requiredResearchBuilding))
                        {
                            return true;
                        }
                    }
                    // Does the research have a required facility?
                    if(___instance.requiredResearchFacilities != null)
                    {
                        if(bench.TryGetComp<Comp_ResearchBenchSubstitutes>().Props.ActLikeResearchFacility != null)
                        {
                            int i;
                            for (i = 0; i < ___instance.requiredResearchFacilities.Count; i++)
                            {
                                if(bench.TryGetComp<Comp_ResearchBenchSubstitutes>().Props.ActLikeResearchFacility.Contains(___instance.requiredResearchFacilities[i]))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }

                return __result;
            }
        }
        **/

        /// <summary>
        /// Accesses the private (For whatever reason) pawn field in the Pawn_NeedsTracker class.
        /// </summary>
        /// <param name="instance">Instance where we should access the value.</param>
        /// <returns>Pawn if it got a pawn, null if it got no pawn.</returns>
        public static Pawn Pawn_NeedsTracker_GetPawn(Pawn_NeedsTracker instance)
        {
            return (Pawn)int_Pawn_NeedsTracker_GetPawn.GetValue(instance);
        }

        /// <summary>
        /// Adds an additional check for our custom needs.
        /// </summary>
        public static void Patch_Pawn_NeedsTracker_ShouldHaveNeed(ref Pawn_NeedsTracker __instance, ref bool __result, ref NeedDef nd)
        {
            //Do not bother checking if our need do not exist.
            Pawn pawn = Pawn_NeedsTracker_GetPawn(__instance);

            if (NeedsDefOf.O21Energy != null)
            {
                //Is the need our Energy need?
                if (nd == NeedsDefOf.O21Energy)
                {
                    if (pawn.def.HasModExtension<EnergyHediffs>())
                    {
                        __result = true;
                    }
                    else
                    {
                        __result = false;
                    }
                }
            }

            if (pawn.def.HasModExtension<EnergyHediffs>())
            {
                if (nd == NeedDefOf.Food || nd == NeedDefOf.Rest || nd == NeedDefOf.Joy || nd == NeedsDefOf.Beauty || nd == NeedsDefOf.Comfort || nd == NeedsDefOf.RoomSize || nd == NeedsDefOf.Outdoors || (Need_Bladder != null && nd == Need_Bladder) || (Need_Hygiene != null && nd == Need_Hygiene))
                {
                    if (pawn.def.HasModExtension<EnergyHediffs>())
                    {
                        __result = false;
                    }
                }
            }

            if (!pawn.def.HasModExtension<EnergyHediffs>())
            {
                if(nd == NeedsDefOf.O21Energy)
                {
                    __result = false;
                }
            }
        }
    }
}
