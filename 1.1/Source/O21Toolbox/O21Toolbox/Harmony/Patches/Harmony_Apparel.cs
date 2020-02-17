using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;
using RimWorld;
using Verse;

using Harmony;

using O21Toolbox.ApparelExt;

namespace O21Toolbox.Harmony
{
    public class Harmony_Apparel
    {
        public static FieldInfo int_PawnRenderer_GetPawn;
        public static void Harmony_Patch(HarmonyInstance O21ToolboxHarmony, Type patchType)
        {
            //O21ToolboxHarmony.Patch(AccessTools.Method(typeof(PawnRenderer), "RenderPawnInternal", new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool) }, null), null, new HarmonyMethod(patchType, "RenderPawnInternalPostfix", null), null);
            O21ToolboxHarmony.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "AddHumanlikeOrders", null, null), null, new HarmonyMethod(patchType, "AddHumanlikeOrdersPostfix", null), null);
            O21ToolboxHarmony.Patch(AccessTools.Method(typeof(JobGiver_OptimizeApparel), "ApparelScoreGain", null, null), null, new HarmonyMethod(patchType, "ApparelScoreGainPostFix", null), null);
            //O21ToolboxHarmony.Patch(AccessTools.Method(typeof(PawnApparelGenerator), "GenerateStartingApparelFor", null, null), new HarmonyMethod(patchType, "GenerateStartingApparelForPrefix", null), new HarmonyMethod(patchType, "GenerateStartingApparelForPostfix", null), null);
        }

        #region ApparelPatches

        //public static void RenderPawnInternalPostfix(PawnRenderer __instance, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump)
        //{
        //    if (!__instance.graphics.pawn.RaceProps.Animal)
        //    {
        //        List<ApparelGraphicRecord> offsetApparelList = new List<ApparelGraphicRecord>();
        //        // Get all apparel with the defModExt.
        //        foreach(Apparel ap in __instance.graphics.pawn.apparel.WornApparel)
        //        {
        //            ApparelGraphicRecord item;
        //            if (ap.def.HasModExtension<DefModExt_HeadwearOffset>())
        //            {
        //                DefModExt_HeadwearOffset modExt = ap.def.GetModExtension<DefModExt_HeadwearOffset>();
        //                if (TryGetGraphicApparelSpecial(ap, __instance.graphics.pawn.story.bodyType, modExt, out item))
        //                {
        //                    offsetApparelList.Add(item);
        //                }
        //            }
        //        }

        //        // Render if any Apparel in the list and NOT an animal.
        //        if (offsetApparelList.Count >= 1)
        //        {
        //            Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
        //            for (int i = 0; i < offsetApparelList.Count; i++)
        //            {
        //                DefModExt_HeadwearOffset modExt = offsetApparelList[i].sourceApparel.def.GetModExtension<DefModExt_HeadwearOffset>();
        //                Vector3 baseOffset = quaternion * modExt.offset;
        //                Mesh mesh = __instance.graphics.HairMeshSet.MeshAt(headFacing);
        //                Vector3 loc2 = rootLoc + baseOffset;
        //                loc2.y += 0.03125f;

        //                if (modExt.bodyDependant)
        //                {

        //                }
        //                else
        //                {
        //                    if (!offsetApparelList[i].sourceApparel.def.apparel.hatRenderedFrontOfFace)
        //                    {
        //                        Material material2 = offsetApparelList[i].graphic.MatAt(bodyFacing, null);
        //                        material2 = __instance.graphics.flasher.GetDamagedMat(material2);
        //                        GenDraw.DrawMeshNowOrLater(mesh, loc2, quaternion, material2, portrait);
        //                    }
        //                    else
        //                    {
        //                        Material material3 = offsetApparelList[i].graphic.MatAt(bodyFacing, null);
        //                        material3 = __instance.graphics.flasher.GetDamagedMat(material3);
        //                        Vector3 loc3 = rootLoc + baseOffset;
        //                        loc3.y += ((!(bodyFacing == Rot4.North)) ? 0.03515625f : 0.00390625f);
        //                        GenDraw.DrawMeshNowOrLater(mesh, loc3, quaternion, material3, portrait);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        private static bool TryGetGraphicApparelSpecial(Apparel apparel, BodyTypeDef bodyType, DefModExt_HeadwearOffset modExt, out ApparelGraphicRecord rec)
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

        /**public static void GenerateStartingApparelForPostfix()
        {
            Traverse.Create(typeof(PawnApparelGenerator)).Field("allApparelPairs").GetValue<List<ThingStuffPair>>().AddRange(HarmonyPatches.apparelList);
        }
        
        public static void GenerateStartingApparelForPrefix(Pawn pawn)
        {
            Traverse traverse = Traverse.Create(typeof(PawnApparelGenerator)).Field("allApparelPairs");
            HarmonyPatches.apparelList = new HashSet<ThingStuffPair>();
            foreach (ThingStuffPair thingStuffPair in GenList.ListFullCopy<ThingStuffPair>(traverse.GetValue<List<ThingStuffPair>>()))
            {
                if (!RaceRestrictionSettings.CanWear(thingStuffPair.thing, pawn.def))
                {
                    HarmonyPatches.apparelList.Add(thingStuffPair);
                }
                if (!ApparelRestrict.RestrictionCheck.CanWear(thingStuffPair.thing, pawn.story.bodyType))
                {

                }
            }
            traverse.GetValue<List<ThingStuffPair>>().RemoveAll((ThingStuffPair tsp) => HarmonyPatches.apparelList.Contains(tsp));
        }**/

        public static void ApparelScoreGainPostFix(Pawn pawn, Apparel ap, ref float __result)
        {
            if (__result < 0f)
            {
                return;
            }
            if (!pawn.AnimalOrWildMan())
            {
                if (!ApparelExt.RestrictionCheck.CanWear(ap.def, pawn))
                {
                    __result -= 50f;
                }
            }
        }

        public static void AddHumanlikeOrdersPostfix(ref List<FloatMenuOption> opts, Pawn pawn, Vector3 clickPos)
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
                    if (!list.NullOrEmpty<FloatMenuOption>() && !WeaponRestrict.RestrictionCheck.CanEquip(equipment.def, pawn))
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
                if (!list2.NullOrEmpty<FloatMenuOption>() && !ApparelExt.RestrictionCheck.CanWear(apparel.def, pawn))
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
        #endregion ApparelPatches
    }
}
