using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.ActivatableEffect;
using O21Toolbox.WeaponExt;

namespace O21Toolbox.HarmonyPatches.Patches
{
    public class Harmony_Activatable
    {
    }

    [HarmonyPatch(typeof(Pawn), "GetGizmos")]
    public static class Patch_Activatable_Pawn_GetGizmos
    {
        [HarmonyPostfix]
        public static void GetGizmosPostfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            var pawn_EquipmentTracker = __instance.equipment;
            if (pawn_EquipmentTracker != null)
            {
                if (__instance.Faction == Faction.OfPlayer)
                {
                    var compActivatableEffect = pawn_EquipmentTracker.Primary?.GetComp<Comp_ActivatableEffect>();
                    if (compActivatableEffect != null)
                    {
                        var gizmos = GizmoGetter(compActivatableEffect);
                        if (gizmos.Any())
                        {
                            __result = __result.Concat(gizmos);
                        }
                        else
                        {
                            if (compActivatableEffect.CurrentState == Comp_ActivatableEffect.State.Activated)
                            {
                                compActivatableEffect.Deactivate();
                            }
                        }
                    }
                }
            }
        }

        public static IEnumerable<Gizmo> GizmoGetter(Comp_ActivatableEffect compActivatableEffect)
        {
            if (compActivatableEffect.GizmosOnEquip)
            {
                foreach (var current in compActivatableEffect.EquippedGizmos())
                {
                    yield return current;
                }
            }
            yield break;
        }
    }

    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipmentAiming")]
    public static class Patch_Activatable_PawnRenderer_DrawEquipmentAiming
    {
        [HarmonyPostfix]
        public static void DrawEquipmentAimingPostFix(Pawn ___pawn, Thing eq, Vector3 drawLoc,
            float aimAngle)
        {
            var compActivatableEffect = ___pawn.equipment?.Primary?.GetComp<Comp_ActivatableEffect>();
            if (compActivatableEffect?.Graphic == null) return;
            if (compActivatableEffect.CurrentState != Comp_ActivatableEffect.State.Activated) return;

            var num = aimAngle - 90f;
            var flip = false;

            if (aimAngle > 20f && aimAngle < 160f)
            {
                num += eq.def.equippedAngleOffset;
            }
            else if (aimAngle > 200f && aimAngle < 340f)
            {
                flip = true;
                num -= 180f;
                num -= eq.def.equippedAngleOffset;
            }
            else
            {
                num += eq.def.equippedAngleOffset;
            }

            Vector3 offset = Vector3.zero;

            if (eq is ThingWithComps eqComps)
            {
                var weaponComp = eqComps.GetComp<Comp_OversizedWeapon>();
                if (weaponComp != null)
                {
                    if (___pawn.Rotation == Rot4.East)
                        offset = weaponComp.Props.eastOffset;
                    else if (___pawn.Rotation == Rot4.West)
                        offset = weaponComp.Props.westOffset;
                    else if (___pawn.Rotation == Rot4.North)
                        offset = weaponComp.Props.northOffset;
                    else if (___pawn.Rotation == Rot4.South)
                        offset = weaponComp.Props.southOffset;
                    offset += weaponComp.Props.offset;
                }

                if (compActivatableEffect.CompDeflectorIsAnimatingNow)
                {
                    float numMod = compActivatableEffect.CompDeflectorAnimationDeflectionTicks;
                    if (numMod > 0)
                    {
                        if (!flip) num += (numMod + 1) / 2;
                        else num -= (numMod + 1) / 2;
                    }
                }
            }
            num %= 360f;

            var matSingle = compActivatableEffect.Graphic.MatSingle;

            var s = new Vector3(eq.def.graphicData.drawSize.x, 1f, eq.def.graphicData.drawSize.y);
            var matrix = default(Matrix4x4);
            matrix.SetTRS(drawLoc + offset, Quaternion.AngleAxis(num, Vector3.up), s);
            if (!flip) Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, 0);
            else Graphics.DrawMesh(MeshPool.plane10Flip, matrix, matSingle, 0);
        }
    }

    [HarmonyPatch(typeof(Verb), "TryStartCastOn")]
    [HarmonyPatch(new Type[]
    {
            typeof(LocalTargetInfo),
            typeof(bool),
            typeof(bool),
            typeof(bool)
    })]
    public static class Patch_Activatable_Verb_TryStartCastOn
    {
        [HarmonyPrefix]
        public static bool Prefix(ref bool __result, Verb __instance)
        {
            if (__instance.caster is Pawn pawn && pawn.equipment?.Primary is ThingWithComps thingWithComps &&
                thingWithComps.GetComp<Comp_ActivatableEffect>() is Comp_ActivatableEffect compActivatableEffect)
            {
                //Equipment source throws errors when checked while casting abilities with a weapon equipped.
                // to avoid this error preventing our code from executing, we do a try/catch.
                try
                {
                    if (__instance.EquipmentSource != thingWithComps)
                        return true;
                }
                catch
                {
                }

                if (compActivatableEffect.CurrentState == Comp_ActivatableEffect.State.Activated)
                    return true;
                else if (compActivatableEffect.TryActivate())
                    return true;
                if (Find.TickManager.TicksGame % 250 == 0)
                    Messages.Message("DeactivatedWarning".Translate(pawn.Label),
                        MessageTypeDefOf.RejectInput);
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Pawn_DraftController), "set_Drafted")]
    public static class Patch_Activatable_Pawn_DraftController_set_Drafted
    {
        [HarmonyPostfix]
        public static void set_DraftedPostFix(Pawn_DraftController __instance, bool value)
        {
            if (__instance.pawn?.equipment?.Primary?.GetComp<Comp_ActivatableEffect>() is Comp_ActivatableEffect compActivatableEffect)
                if (value == false)
                {
                    if (compActivatableEffect.CurrentState == Comp_ActivatableEffect.State.Activated)
                        compActivatableEffect.TryDeactivate();
                }
                else
                {
                    if (compActivatableEffect.CurrentState == Comp_ActivatableEffect.State.Deactivated)
                        compActivatableEffect.TryActivate();
                }
        }
    }

    [HarmonyPatch(typeof(Pawn), "ExitMap")]
    public static class Patch_Activatable_Pawn_ExitMap
    {
        [HarmonyPrefix]
        public static void ExitMap_PreFix(Pawn __instance)
        {
            if (__instance.equipment?.Primary?.GetComp<Comp_ActivatableEffect>() is Comp_ActivatableEffect compActivatableEffect &&
                compActivatableEffect.CurrentState == Comp_ActivatableEffect.State.Activated)
                compActivatableEffect.TryDeactivate();
        }
    }

    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "TryDropEquipment")]
    public static class Patch_Activatable_Pawn_EquipmentTracker_TryDropEquipment
    {
        [HarmonyPrefix]
        public static void TryDropEquipment_PreFix(Pawn_EquipmentTracker __instance)
        {
            if (__instance.Primary?.GetComp<Comp_ActivatableEffect>() is Comp_ActivatableEffect compActivatableEffect &&
                compActivatableEffect.CurrentState == Comp_ActivatableEffect.State.Activated)
                compActivatableEffect.TryDeactivate();
        }
    }
}
