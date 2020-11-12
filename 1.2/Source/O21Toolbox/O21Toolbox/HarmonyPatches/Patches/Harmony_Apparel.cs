using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.ApparelExt;

namespace O21Toolbox.HarmonyPatches
{
    public class Harmony_Apparel
    {
        public static FieldInfo int_PawnRenderer_GetPawn;
        //private static HashSet<ThingStuffPair> apparelList;

        public static void Harmony_Patch(Harmony O21ToolboxHarmony, Type patchType)
        {

        }

        #region ApparelPatches

        [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal")]
        [HarmonyPatch(new Type[]
        {
            typeof(Vector3),
            typeof(float),
            typeof(bool),
            typeof(Rot4),
            typeof(Rot4),
            typeof(RotDrawMode),
            typeof(bool),
            typeof(bool),
            typeof(bool)
        })]
        public static class PawnRenderer_RenderPawnInternal_Postfix
        {
            [HarmonyPostfix]
            public static void Postfix(PawnRenderer __instance, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
            {
                if (!__instance?.graphics?.pawn?.RaceProps?.Animal ?? false)
                {
                    List<ApparelGraphicRecord> offsetApparelList = new List<ApparelGraphicRecord>();
                    // Get all apparel with the defModExt.
                    foreach (Apparel ap in __instance?.graphics?.pawn?.apparel?.WornApparel)
                    {
                        ApparelGraphicRecord item;
                        if (ap.def.HasModExtension<DefModExt_ApparelOffset>())
                        {
                            DefModExt_ApparelOffset modExt = ap.def.GetModExtension<DefModExt_ApparelOffset>();
                            if (TryGetGraphicApparelSpecial(ap, __instance.graphics.pawn.story.bodyType, modExt, out item))
                            {
                                offsetApparelList.Add(item);
                            }
                        }
                    }

                    // Render if any Apparel in the list
                    if (offsetApparelList?.Count >= 1)
                    {
                        Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
                        Mesh mesh = MeshPool.humanlikeBodySet.MeshAt(bodyFacing);
                        Vector3 vector2 = rootLoc;
                        vector2.y += ((bodyFacing == Rot4.South) ? 0.006122449f : 0.02755102f);

                        for (int i = 0; i < offsetApparelList.Count; i++)
                        {
                            ApparelGraphicRecord apparelGraphicRecord = offsetApparelList[i];
                            Pawn pawn = __instance.graphics.pawn;
                            DefModExt_ApparelOffset modExt = apparelGraphicRecord.sourceApparel.def.GetModExtension<DefModExt_ApparelOffset>();

                            Material material = apparelGraphicRecord.graphic.MatAt(bodyFacing, null);
                            material = OverrideMaterialIfNeeded(__instance, material, pawn, portrait);
                            if (apparelGraphicRecord.sourceApparel.def.apparel.wornGraphicData != null)
                            {
                                Vector2 vector3 = apparelGraphicRecord.sourceApparel.def.apparel.wornGraphicData.BeltOffsetAt(bodyFacing, pawn.story.bodyType);
                                Vector2 vector4 = apparelGraphicRecord.sourceApparel.def.apparel.wornGraphicData.BeltScaleAt(pawn.story.bodyType);
                                Matrix4x4 matrix = Matrix4x4.Translate(vector2) * Matrix4x4.Rotate(quaternion) * Matrix4x4.Translate(new Vector3(vector3.x, modExt.zOffset, vector3.y)) * Matrix4x4.Scale(new Vector3(vector4.x, 1f, vector4.y));
                                GenDraw.DrawMeshNowOrLater_NewTemp(mesh, matrix, material, portrait);
                            }
                        }
                    }
                }
            }

            private static Material OverrideMaterialIfNeeded(PawnRenderer __instance, Material original, Pawn pawn, bool portrait = false)
            {
                Material baseMat = (!portrait && pawn.IsInvisible()) ? InvisibilityMatPool.GetInvisibleMat(original) : original;
                return __instance.graphics.flasher.GetDamagedMat(baseMat);
            }
        }

        /**public static void GenerateStartingApparelForPostfix()
        {
            Traverse.Create(typeof(PawnApparelGenerator)).Field("allApparelPairs").GetValue<List<ThingStuffPair>>().AddRange(HarmonyPatches.apparelList);
        }**/

        //[HarmonyPatch(typeof(PawnApparelGenerator), "GenerateStartingApparelFor")]
        //public static class GenerateStartingApparelFor
        //{
        //    [HarmonyPrefix]
        //    public static void Prefix(Pawn pawn)
        //    {
        //        Traverse traverse = Traverse.Create(typeof(PawnApparelGenerator)).Field("allApparelPairs");
        //        apparelList = new HashSet<ThingStuffPair>();
        //        foreach (ThingStuffPair thingStuffPair in GenList.ListFullCopy<ThingStuffPair>(traverse.GetValue<List<ThingStuffPair>>()))
        //        {
        //            if (thingStuffPair.thing != null && !ApparelExt.RestrictionCheck.CanWear(thingStuffPair.thing, pawn))
        //            {
        //                apparelList.Add(thingStuffPair);
        //            }
        //        }
        //        traverse.GetValue<List<ThingStuffPair>>().RemoveAll((ThingStuffPair tsp) => apparelList.Contains(tsp));
        //    }

        //    [HarmonyPostfix]
        //    public static void Postfix()
        //    {
        //        Traverse.Create(typeof(PawnApparelGenerator)).Field("allApparelPairs").GetValue<List<ThingStuffPair>>().AddRange(apparelList);
        //    }
        //}

        [HarmonyPatch(typeof(Pawn_ApparelTracker))]
        [HarmonyPatch("Notify_ApparelAdded")]
        public class Harmony_Pawn_ApparelTracker_Notify_ApparelAdded
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn_ApparelTracker __instance, Apparel apparel)
            {
                DefModExt_HediffApparel modExtension = apparel.def.GetModExtension<DefModExt_HediffApparel>();
                bool flag = modExtension != null && modExtension.wearingHediff != null;
                if (flag)
                {
                    Pawn pawn = __instance.pawn;
                    bool flag2 = modExtension.wearingOn == null;
                    if (flag2)
                    {
                        pawn.health.AddHediff(modExtension.wearingHediff, null, null, null);
                    }
                    else
                    {
                        foreach (BodyPartRecord part in pawn.def.race.body.GetPartsWithDef(modExtension.wearingOn))
                        {
                            pawn.health.AddHediff(modExtension.wearingHediff, part, null, null);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Pawn_ApparelTracker))]
        [HarmonyPatch("Notify_ApparelRemoved")]
        public class Harmony_Pawn_ApparelTracker_Notify_ApparelRemoved
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn_ApparelTracker __instance, Apparel apparel)
            {
                DefModExt_HediffApparel modExtension = apparel.def.GetModExtension<DefModExt_HediffApparel>();
                bool flag = modExtension != null && modExtension.wearingHediff != null;
                if (flag)
                {
                    HashSet<Hediff> hashSet = new HashSet<Hediff>();
                    Pawn_HealthTracker health = __instance.pawn.health;
                    foreach (Hediff hediff in health.hediffSet.hediffs)
                    {
                        bool flag2 = hediff.def == modExtension.wearingHediff;
                        if (flag2)
                        {
                            hashSet.Add(hediff);
                        }
                    }
                    foreach (Hediff hediff2 in hashSet)
                    {
                        health.RemoveHediff(hediff2);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(JobGiver_OptimizeApparel), "ApparelScoreGain_NewTmp")]
        public static class ApparelScoreGainPostFix
        {
            [HarmonyPostfix]
            public static void Postfix(Pawn pawn, Apparel ap, ref float __result)
            {
                if (__result < 0f)
                {
                    return;
                }
                if (!pawn.AnimalOrWildMan() && !ApparelExt.RestrictionCheck.CanWear(ap, pawn))
                {
                    __result = -1000f;
                }
            }
        }

        [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
        public static class AddHumanlikeOrdersPostfix
        {
            [HarmonyPostfix]
            public static void Postfix(ref List<FloatMenuOption> opts, Pawn pawn, Vector3 clickPos)
            {
                IntVec3 c = IntVec3.FromVector3(clickPos);
                if (pawn.equipment != null)
                {
                    ThingWithComps equipment = (ThingWithComps)c.GetThingList(pawn.Map).FirstOrDefault((Thing t) => t.TryGetComp<CompEquippable>() != null && t.def.IsWeapon);
                    if (equipment != null)
                    {
                        List<FloatMenuOption> list = (from fmo in opts
                                                      where !fmo.Disabled && fmo.Label.Contains("Equip".Translate(equipment.LabelShort))
                                                      select fmo).ToList<FloatMenuOption>();
                        if (!list.NullOrEmpty<FloatMenuOption>() && !WeaponExt.RestrictionCheck.CanEquip(equipment.def, pawn))
                        {
                            foreach (FloatMenuOption item2 in list)
                            {
                                int index2 = opts.IndexOf(item2);
                                opts.Remove(item2);
                                opts.Insert(index2, new FloatMenuOption("CannotEquip".Translate(equipment.LabelShortCap) + " (missing required apparel)", null, MenuOptionPriority.Default, null, null, 0f, null, null));
                            }
                        }
                    }
                }
                Apparel apparel = pawn.Map.thingGrid.ThingAt<Apparel>(c);
                if (apparel != null)
                {
                    List<FloatMenuOption> list2 = (from fmo in opts
                                                   where !fmo.Disabled && fmo.Label.Contains("ForceWear".Translate(apparel.LabelShort, apparel)) && !fmo.Label.Contains("CannotWear".Translate(apparel.LabelShort, apparel))
                                                   select fmo).ToList<FloatMenuOption>();
                    if (!list2.NullOrEmpty<FloatMenuOption>() && !ApparelExt.RestrictionCheck.CanWear(apparel, pawn))
                    {
                        foreach (FloatMenuOption item3 in list2)
                        {
                            int index3 = opts.IndexOf(item3);
                            opts.Remove(item3);
                            opts.Insert(index3, new FloatMenuOption("CannotWear".Translate(apparel.LabelShort, apparel) + " (" + pawn.story.bodyType.defName.ToString() + " body can't wear this)", null, MenuOptionPriority.Default, null, null, 0f, null, null));
                        }
                    }
                }
            }
        }

        private static bool TryGetGraphicApparelSpecial(Apparel apparel, BodyTypeDef bodyType, DefModExt_ApparelOffset modExt, out ApparelGraphicRecord rec)
        {
            if (bodyType == null)
            {
                Log.Error("Getting apparel graphic with undefined body type.", false);
                bodyType = BodyTypeDefOf.Male;
            }
            if (modExt.wornGraphicPath.NullOrEmpty())
            {
                rec = new ApparelGraphicRecord(null, null);
                return false;
            }
            string path;
            if (!modExt.bodyDependant)
            {
                path = modExt.wornGraphicPath;
            }
            else
            {
                path = modExt.wornGraphicPath + "_" + bodyType.defName;
            }
            Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, apparel.def.graphicData.drawSize, apparel.DrawColor);
            rec = new ApparelGraphicRecord(graphic, apparel);
            return true;
        }

        public static Pawn PawnRenderer_GetPawn_GetPawn(PawnRenderer instance)
        {
            return (Pawn)int_PawnRenderer_GetPawn.GetValue(instance);
        }
        #endregion ApparelPatches
    }
}
