using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using UnityEngine;
using RimWorld;
using RimWorld.BaseGen;
using Verse;
using HarmonyLib;

using O21Toolbox.ApparelExt;
using O21Toolbox.Drones;
using O21Toolbox.Research;
using O21Toolbox.Utility;

namespace O21Toolbox.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        /// <summary>
        /// For internal use: Field for getting the pawn.
        /// </summary>
        public static FieldInfo int_Pawn_NeedsTracker_GetPawn;
        public static FieldInfo int_PawnRenderer_GetPawn;
        public static FieldInfo int_Need_Food_Starving_GetPawn;
        public static FieldInfo int_ConditionalPercentageNeed_need;
        public static FieldInfo int_Pawn_HealthTracker_GetPawn;
        public static FieldInfo int_Pawn_InteractionsTracker_GetPawn;

        public static NeedDef Need_Bladder;
        public static NeedDef Need_Hygiene;

        private static readonly Type patchType = typeof(HarmonyPatches);

        static HarmonyPatches()
        {
            //Try get needs.
            Need_Bladder = DefDatabase<NeedDef>.GetNamedSilentFail("Bladder");
            Need_Hygiene = DefDatabase<NeedDef>.GetNamedSilentFail("Hygiene");

            Harmony O21ToolboxHarmony = new Harmony("com.o21toolbox.rimworld.mod");

            Harmony_Apparel.Harmony_Patch(O21ToolboxHarmony, patchType);

            Harmony_Drones.Harmony_Patch(O21ToolboxHarmony, patchType);

            Harmony_Needs.Harmony_Patch(O21ToolboxHarmony, patchType);

            #region ModularWeapon
            //O21ToolboxHarmony.Patch(AccessTools.Method(typeof(PawnRenderer), "DrawEquipmentAiming", null, null), null, new HarmonyMethod(HarmonyPatches.patchType, "DrawEquipmentAimingPostfix", null), null);
            #endregion ModularWeapon

            #region ThirdPartyImrpovements
            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Save Our Ship 2"))
            {
                SaveOurShip2_CompatibilityHook(O21ToolboxHarmony);
            }
            #endregion

            O21ToolboxHarmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        // Patches

        #region ModularWeaponPatches
        /** public static void DrawEquipmentAimingPostfix(Thing eq, Vector3 drawLoc, float aimAngle)
        {
            float num = aimAngle - 90f;
            Mesh mesh;
            if (aimAngle > 20f && aimAngle < 160f)
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }
            else if (aimAngle > 200f && aimAngle < 340f)
            {
                mesh = MeshPool.plane10Flip;
                num -= 180f;
                num -= eq.def.equippedAngleOffset;
            }
            else
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }
            num %= 360f;
            Graphic_StackCount graphic_StackCount = eq.Graphic as Graphic_StackCount;
            Material matSingle;
            if (eq.def.HasModExtension<DefModExtension_ModularWeapon>())
            {
                matSingle = eq.def.GetModExtension<DefModExtension_ModularWeapon>().GetCurrentTexture.MatSingle;
            }
            else if (graphic_StackCount != null)
            {
                matSingle = graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingle;
            }
            else
            {
                matSingle = eq.Graphic.MatSingle;
            }
            Graphics.DrawMesh(mesh, drawLoc, Quaternion.AngleAxis(num, Vector3.up), matSingle, 0);
        }**/
        #endregion ModularWeaponPatches


        #region ThirdPartyImprovements
        public static void SaveOurShip2_CompatibilityHook(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(typeof(SaveOurShip2.ShipInteriorMod2), "hasSpaceSuit"), null, new HarmonyMethod(typeof(HarmonyPatches), "SOS2CompatibilityHook_hasSpaceSuit_Postfix"));
        }
        public static void SOS2CompatibilityHook_hasSpaceSuit_Postfix(Pawn thePawn, ref bool __result)
        {
            if (thePawn != null && __result == false)
            {
                if (thePawn.def.HasModExtension<DefModExt_SpaceCapable>())
                {
                    __result = true;
                }
                else if(thePawn.apparel != null)
                {
                    bool hasHelmet = false;
                    bool hasSuit = false;
                    bool raceSpaceCapable = false;
                    if (thePawn.def.HasModExtension<DefModExt_SpaceCapable>())
                    {
                        raceSpaceCapable = true;
                    }
                    if(!raceSpaceCapable)
                    {
                        foreach (Apparel ap in thePawn.apparel.WornApparel)
                        {
                            if (ap.def.HasModExtension<DefModExt_SpaceApparel>())
                            {
                                DefModExt_SpaceApparel ext = ap.def.GetModExtension<DefModExt_SpaceApparel>();
                                if (ext.equipmentType == spaceEquipmentType.full)
                                {
                                    hasHelmet = true;
                                    hasSuit = true;
                                }
                                else if (ext.equipmentType == spaceEquipmentType.helmet)
                                {
                                    hasHelmet = true;
                                }
                                else if (ext.equipmentType == spaceEquipmentType.suit)
                                {
                                    hasSuit = true;
                                }
                            }
                        }
                    }

                    __result = (hasHelmet && hasSuit) || raceSpaceCapable;
                }
            }
        }
        #endregion
    }
}