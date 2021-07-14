using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.SlotLoadable;
using O21Toolbox.Utility;

namespace O21Toolbox.HarmonyPatches.Patches
{
    public class Harmony_SlotLoadable
    {
    }

    [HarmonyPatch(typeof(StatExtension), nameof(StatExtension.GetStatValue))]
    public static class Patch_StatExtension_GetGizmos
    {
        [HarmonyPostfix]

        public static void GetStatValue_PostFix(ref float __result, Thing thing, StatDef stat)
        {
            var retValue = 0.0f;
            try
            {
                retValue = SlotLoadableUtility.CheckThingSlotsForStatAugment(thing, stat);
            }
            catch (Exception e)
            {
                Log.Warning("Failed to add stats for " + thing.Label + "\n" + e.ToString());
            }
            __result += retValue;
        }
    }

    [HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos))]
    public static class Patch_Pawn_GetGizmos
    {
        [HarmonyPostfix]

        public static void GetGizmos_PostFix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            var pawn_EquipmentTracker = __instance.equipment;
            if (pawn_EquipmentTracker != null)
            {
                if (__instance.Faction == Faction.OfPlayer)
                {
                    var compSlotLoadable = pawn_EquipmentTracker.Primary?.GetComp<Comp_SlotLoadable>();
                    if (compSlotLoadable != null)
                    {
                        var gizmos = GizmoGetter(compSlotLoadable);
                        if (gizmos.Any())
                        {
                            __result = __result.Concat(gizmos);
                        }
                    }
                }
            }
        }

        public static IEnumerable<Gizmo> GizmoGetter(Comp_SlotLoadable CompSlotLoadable)
        {
            if (CompSlotLoadable.GizmosOnEquip)
            {
                foreach (var current in CompSlotLoadable.EquippedGizmos())
                {
                    yield return current;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Verb_MeleeAttackDamage), "DamageInfosToApply")]
    public static class Patch_Verb_MeleeAttackDamage_DamageInfosToApply
    {
        [HarmonyPostfix]
        public static void DamageInfosToApply_PostFix(Verb_MeleeAttack __instance, ref IEnumerable<DamageInfo> __result,
            LocalTargetInfo target)
        {
            var slots = __instance.EquipmentSource?.GetComp<Comp_SlotLoadable>()?.Slots;
            if (slots != null)
            {
                List<DamageInfo> newList = null;
                var statSlots = slots.FindAll(z =>
                    !z.IsEmpty() && ((SlotLoadableDef)z.def).doesChangeStats);
                foreach (var slot in statSlots)
                {
                    var slotBonus = slot.SlotOccupant.TryGetComp<Comp_SlottedBonus>();
                    if (slotBonus != null)
                    {
                        if (slotBonus.Props.damageDef != null)
                        {
                            var num = __instance.verbProps.AdjustedMeleeDamageAmount(__instance,
                                __instance.CasterPawn);
                            var def = __instance.verbProps.meleeDamageDef;
                            BodyPartGroupDef weaponBodyPartGroup = null;
                            HediffDef weaponHediff = null;
                            if (__instance.CasterIsPawn)
                                if (num >= 1f)
                                {
                                    weaponBodyPartGroup = __instance.verbProps.linkedBodyPartsGroup;
                                    if (__instance.HediffCompSource != null)
                                        weaponHediff = __instance.HediffCompSource.Def;
                                }
                                else
                                {
                                    num = 1f;
                                    def = DamageDefOf.Blunt;
                                }

                            ThingDef def2;
                            if (__instance.EquipmentSource != null)
                                def2 = __instance.EquipmentSource.def;
                            else
                                def2 = __instance.CasterPawn.def;

                            var angle = (target.Thing.Position - __instance.CasterPawn.Position)
                                .ToVector3();

                            var caster = __instance.caster;

                            var newdamage = GenMath.RoundRandom(num);
                            //Log.Message("applying damage " + newdamage + " out of "+num);
                            var damageInfo = new DamageInfo(slotBonus.Props.damageDef, newdamage, slotBonus.Props.armorPenetration, -1f,
                                caster, null, def2);
                            damageInfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                            damageInfo.SetWeaponBodyPartGroup(weaponBodyPartGroup);
                            damageInfo.SetWeaponHediff(weaponHediff);
                            damageInfo.SetAngle(angle);

                            if (newList == null)
                                newList = new List<DamageInfo>();
                            newList.Add(damageInfo);

                            __result = newList.AsEnumerable();
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(ITab_Pawn_Gear), "DrawThingRow")]
    public static class Patch_ITab_Pawn_Gear_DrawThingRow
    {
        private static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);

        private static readonly Color ThingLabelColor = new Color(0.9f, 0.9f, 0.9f, 1f);

        [HarmonyPostfix]
        public static void DrawThingRow_PostFix(ref float y, float width, Thing thing)
        {
            var compSlotLoadable = thing.TryGetComp<Comp_SlotLoadable>();
            if (compSlotLoadable?.Slots != null)
            {
                foreach (var slot in compSlotLoadable.Slots)
                {
                    if (!slot.IsEmpty())
                    {
                        var rect = new Rect(0f, y, width, 28f);
                        Widgets.InfoCardButton(rect.width - 24f, y, slot.SlotOccupant);
                        rect.width -= 24f;
                        if (Mouse.IsOver(rect))
                        {
                            GUI.color = HighlightColor;
                            GUI.DrawTexture(rect, TexUI.HighlightTex);
                        }
                        if (slot.SlotOccupant.def.DrawMatSingle != null &&
                            slot.SlotOccupant.def.DrawMatSingle.mainTexture != null)
                            Widgets.ThingIcon(new Rect(4f, y, 28f, 28f), slot.SlotOccupant, 1f);
                        Text.Anchor = TextAnchor.MiddleLeft;
                        GUI.color = ThingLabelColor;
                        var rect4 = new Rect(36f, y, width - 36f, 28f);
                        var text = slot.SlotOccupant.LabelCap;
                        Widgets.Label(rect4, text);
                        y += 28f;
                    }
                }
            }
        }
    }
        
    [HarmonyPatch(typeof(StatWorker), "StatOffsetFromGear")]
    public static class Patch_StatWorker_StatOffsetFromGear
    {
        [HarmonyPostfix]
        public static void StatOffsetFromGear_PostFix(ref float __result, Thing gear, StatDef stat)
        {
            var retValue = 0.0f;
            try
            {
                retValue = SlotLoadableUtility.CheckThingSlotsForStatAugment(gear, stat);
            }
            catch (Exception e)
            {
                Log.Warning("Failed to add stats for " + gear.Label + "\n" + e.ToString());
            }
            __result += retValue;
        }
    }
}
