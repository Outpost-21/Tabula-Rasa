using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.ApparelExt;

namespace O21Toolbox.HarmonyPatches
{
    public class Harmony_Apparel
    {
        public static void Harmony_Patch(Harmony O21ToolboxHarmony, Type patchType)
        {

        }

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

        public static bool TryGetGraphicApparelSpecial(Apparel apparel, BodyTypeDef bodyType, DefModExt_ApparelOffset modExt, out ApparelGraphicRecord rec)
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
    }
}
