﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

using HarmonyLib;

using O21Toolbox.Abilities;

namespace O21Toolbox.HarmonyPatches.Patches
{
    public class Harmony_Abilities
    {
        [HarmonyPatch(typeof(Targeter), nameof(Targeter.TargeterUpdate))]
        public static void TargeterUpdate_PostFix(Targeter __instance)
        {
            if (__instance.targetingSource is Verb_UseAbility tVerb &&
                tVerb.verbProps is VerbProperties_Ability tVerbProps)
            {
                if (tVerbProps?.range > 0)
                    GenDraw.DrawRadiusRing(tVerb.CasterPawn.PositionHeld, tVerbProps.range);
                if (tVerbProps?.TargetAoEProperties?.range > 0 && Find.CurrentMap is Map map &&
                    UI.MouseCell().InBounds(map))
                    GenDraw.DrawRadiusRing(UI.MouseCell(), tVerbProps.TargetAoEProperties.range);
            }
        }

        [HarmonyPatch(typeof(Targeter), nameof(Targeter.ProcessInputEvents))]
        public static bool ProcessInputEvents_PreFix(Targeter __instance)
        {
            if (__instance.targetingSource is Verb_UseAbility v)
            {
                if (v.UseAbilityProps.AbilityTargetCategory == AbilityTargetCategory.TargetSelf)
                {
                    var caster = (Pawn)__instance.targetingSource.CasterPawn;
                    v.Ability.TryCastAbility(AbilityContext.Player,
                        caster); // caster, source.First<LocalTargetInfo>(), caster.GetComp<Comp_AbilityUser>(), (Verb_UseAbility)__instance.targetingSource, ((Verb_UseAbility)(__instance.targetingSource)).ability.powerdef as AbilityDef)?.Invoke();
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                    __instance.StopTargeting();
                    Event.current.Use();
                    return false;
                }
                AccessTools.Method(typeof(Targeter), "ConfirmStillValid").Invoke(__instance, null);
                if (Event.current.type == EventType.MouseDown)
                    if (Event.current.button == 0 && __instance.IsTargeting)
                    {
                        var obj = (LocalTargetInfo)AccessTools.Method(typeof(Targeter), "CurrentTargetUnderMouse")
                            .Invoke(__instance, new object[] { false });
                        if (obj.IsValid)
                            v.Ability.TryCastAbility(AbilityContext.Player, obj);
                        SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                        __instance.StopTargeting();
                        Event.current.Use();
                        return false;
                        //if (__instance.targetingSource is Verb_UseAbility)
                        //{
                        //    Verb_UseAbility abilityVerb = __instance.targetingSource as Verb_UseAbility;
                        //    if (abilityVerb.Ability.Def.MainVerb.AbilityTargetCategory != AbilityTargetCategory.TargetSelf)
                        //    {
                        //        TargetingParameters targetParams = abilityVerb.Ability.Def.MainVerb.targetParams;
                        //        if (targetParams != null)
                        //        {
                        //            IEnumerable<LocalTargetInfo> source = GenUI.TargetsAtMouse(targetParams, false);

                        //            if (source != null && source.Count<LocalTargetInfo>() > 0)
                        //            {

                        //                if (source.Any<LocalTargetInfo>())
                        //                {

                        //                    Pawn caster = (Pawn)__instance.targetingSource.caster;
                        //                    abilityVerb.Ability.TryCastAbility(AbilityContext.Player, source.First<LocalTargetInfo>());// caster, source.First<LocalTargetInfo>(), caster.GetComp<Comp_AbilityUser>(), (Verb_UseAbility)__instance.targetingSource, ((Verb_UseAbility)(__instance.targetingSource)).ability.powerdef as AbilityDef)?.Invoke();
                        //                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                        //                    __instance.StopTargeting();
                        //                    Event.current.Use();
                        //                    return false;
                        //                }
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        Pawn caster = (Pawn)__instance.targetingSource.caster;
                        //        abilityVerb.Ability.TryCastAbility(AbilityContext.Player, null);// caster.GetComp<Comp_AbilityUser>(), (Verb_UseAbility)__instance.targetingSource, ((Verb_UseAbility)(__instance.targetingSource)).ability.powerdef as AbilityDef)?.Invoke();
                        //        SoundDefOf.Tick_High.PlayOneShotOnCamera();
                        //        __instance.StopTargeting();
                        //        Event.current.Use();
                        //        return false;
                        //    }
                        //}
                        //}
                    }
            }
            return true;
        }

        [HarmonyPatch(typeof(Targeter), "ConfirmStillValid")]
        public static bool ConfirmStillValid(Targeter __instance)
        {
            if (__instance.targetingSource is Verb_UseAbility)
            {
                var caster = Traverse.Create(__instance).Field("caster").GetValue<Pawn>();

                if (caster != null && (caster.Map != Find.CurrentMap || caster.Destroyed ||
                                       !Find.Selector.IsSelected(caster) ||
                                       caster.Faction != Faction.OfPlayerSilentFail))
                    __instance.StopTargeting();
                if (__instance.targetingSource != null)
                {
                    var selector = Find.Selector;
                    if (__instance.targetingSource.CasterPawn.Map != Find.CurrentMap ||
                        __instance.targetingSource.CasterPawn.Destroyed ||
                        !selector.IsSelected(__instance.targetingSource.CasterPawn))
                    {
                        __instance.StopTargeting();
                    }
                    else
                    {
                        if (!__instance.targetingSourceAdditionalPawns.NullOrEmpty())
                            for (var i = 0; i < __instance.targetingSourceAdditionalPawns.Count; i++)
                                if (__instance.targetingSourceAdditionalPawns[i].Destroyed ||
                                    !selector.IsSelected(__instance.targetingSourceAdditionalPawns[i]))
                                {
                                    __instance.StopTargeting();
                                    break;
                                }
                    }
                }
                return false;
            }
            return true;
        }

        // Initializes the AbilityUsers on Pawns
        [HarmonyPatch(typeof(ThingWithComps), nameof(ThingWithComps.InitializeComps))]
        public static void InitializeComps_PostFix(ThingWithComps __instance)
        {
            if (__instance is Pawn p) InternalAddInAbilityUsers(p);
        }

        // when the Pawn_EquipmentTracker is notified of a new item, see if that has Comp_AbilityItem.
        [HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.Notify_EquipmentAdded))]
        public static void Notify_EquipmentAdded_PostFix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            foreach (var cai in eq.GetComps<Comp_AbilityItem>()
                ) //((Pawn)__instance.ParentHolder).GetComps<Comp_AbilityItem>() )
                  //Log.Message("Notify_EquipmentAdded_PostFix 1 : "+eq.ToString());
                  //Log.Message("  Found Comp_AbilityItem, for Comp_AbilityUser of "+cai.Props.AbilityUserClass.ToString());

                foreach (var cau in ((Pawn)__instance.ParentHolder).GetComps<Comp_AbilityUser>())
                    //Log.Message("  Found Comp_AbilityUser, "+cau.ToString() +" : "+ cau.GetType()+":"+cai.Props.AbilityUserClass ); //Props.AbilityUserTarget.ToString());
                    if (cau.GetType() == cai.Props.AbilityUserClass)
                    {
                        //Log.Message("  and they match types " );
                        cai.AbilityUserTarget = cau;
                        foreach (var abdef in cai.Props.Abilities) cau.AddWeaponAbility(abdef);
                    }
        }

        // when the Pawn_EquipmentTracker is notified of one less item, see if that has Comp_AbilityItem.
        [HarmonyPatch(typeof(Pawn_EquipmentTracker), nameof(Pawn_EquipmentTracker.Notify_EquipmentRemoved))]
        public static void Notify_EquipmentRemoved_PostFix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            foreach (var cai in eq.GetComps<Comp_AbilityItem>()
                ) //((Pawn)__instance.ParentHolder).GetComps<Comp_AbilityItem>() )
                  //Log.Message("Notify_EquipmentAdded_PostFix 1 : "+eq.ToString());
                  //Log.Message("  Found Comp_AbilityItem, for Comp_AbilityUser of "+cai.Props.AbilityUserClass.ToString());

                foreach (var cau in ((Pawn)__instance.ParentHolder).GetComps<Comp_AbilityUser>())
                    //Log.Message("  Found Comp_AbilityUser, "+cau.ToString() +" : "+ cau.GetType()+":"+cai.Props.AbilityUserClass ); //Props.AbilityUserTarget.ToString());
                    if (cau.GetType() == cai.Props.AbilityUserClass)
                        foreach (var abdef in cai.Props.Abilities) cau.RemoveWeaponAbility(abdef);
        }

        // when the Pawn_ApparelTracker is notified of a new item, see if that has Comp_AbilityItem.
        [HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Notify_ApparelAdded))]
        public static void Notify_ApparelAdded_PostFix(Pawn_ApparelTracker __instance, Apparel apparel)
        {
            foreach (var cai in apparel.GetComps<Comp_AbilityItem>()
            ) //((Pawn)__instance.ParentHolder).GetComps<Comp_AbilityItem>() )
                foreach (var cau in ((Pawn)__instance.ParentHolder).GetComps<Comp_AbilityUser>())
                    if (cau.GetType() == cai.Props.AbilityUserClass)
                    {
                        cai.AbilityUserTarget = cau;
                        foreach (var abdef in cai.Props.Abilities) cau.AddApparelAbility(abdef);
                    }
        }

        // when the Pawn_ApparelTracker is notified of one less item, see if that has Comp_AbilityItem.
        [HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.Notify_ApparelRemoved))]
        public static void Notify_ApparelRemoved_PostFix(Pawn_ApparelTracker __instance, Apparel apparel)
        {
            foreach (var cai in apparel.GetComps<Comp_AbilityItem>()
            ) //((Pawn)__instance.ParentHolder).GetComps<Comp_AbilityItem>() )
                foreach (var cau in ((Pawn)__instance.ParentHolder).GetComps<Comp_AbilityUser>())
                    if (cau.GetType() == cai.Props.AbilityUserClass)
                        foreach (var abdef in cai.Props.Abilities) cau.RemoveApparelAbility(abdef);
        }


        [HarmonyPatch(typeof(ShortHashGiver), "GiveShortHash")]
        public static bool GiveShortHash_PrePatch(Def def, Type defType)
        {
            //Log.Message("Shorthash called");
            if (def.shortHash != 0)
                if (defType.IsAssignableFrom(typeof(Abilities.AbilityDef)) || defType == typeof(Abilities.AbilityDef) ||
                    def is Abilities.AbilityDef)
                    return false;
            return true;
        }

        [HarmonyPatch(typeof(PawnGroupKindWorker), nameof(PawnGroupKindWorker.GeneratePawns))]
        public static void GeneratePawns_PostFix(PawnGroupMakerParms parms, PawnGroupMaker groupMaker,
            bool errorOnZeroResults, ref List<Pawn> __result)
        {
            //Anyone special?
            if (__result?.Count > 0 &&
                __result.FindAll(x => x.TryGetComp<Comp_AbilityUser>() is Comp_AbilityUser cu && cu.CombatPoints() > 0) is
                    List<Pawn> specialPawns && specialPawns?.Count > 0)
            {
                //Log.Message("Special Pawns Detected");
                //Log.Message("------------------");

                //Points
                var previousPoints = parms.points;
                //Log.Message("Points: " +  previousPoints);

                //Log.Message("Average Characters");
                //Log.Message("------------------");

                //Anyone average?
                int avgPawns = 0;
                var avgCombatPoints = new Dictionary<Pawn, float>();
                if (__result.FindAll(x => x.TryGetComp<Comp_AbilityUser>() == null) is List<Pawn> averagePawns)
                {
                    avgPawns = averagePawns.Count;
                    averagePawns.ForEach(x =>
                    {
                        avgCombatPoints.Add(x, x.kindDef.combatPower);
                        //Log.Message(x.LabelShort + " : " + x.kindDef.combatPower);
                    });
                }

                //Log.Message("------------------");                                
                //Log.Message("Special Characters");
                //Log.Message("------------------");

                //What's your powers?
                var specCombatPoints = new Dictionary<Pawn, float>();
                specialPawns.ForEach(x =>
                {
                    var combatValue = x.kindDef.combatPower;
                    foreach (var thingComp in x.AllComps.FindAll(y => y is Comp_AbilityUser))
                    {
                        //var compAbilityUser = (Comp_AbilityUser) thingComp;
                        var val = Traverse.Create(thingComp).Method("CombatPoints").GetValue<float>();
                        combatValue += val; //compAbilityUser.CombatPoints();
                    }
                    specCombatPoints.Add(x, combatValue);
                    //Log.Message(x.LabelShort + " : " + combatValue);
                });

                //Special case -- single raider/character should not be special to avoid problems (e.g. Werewolf raid destroys everyone).
                if (avgPawns == 0 && specCombatPoints.Sum(x => x.Value) > 0 && specialPawns.Count == 1)
                {
                    //Log.Message("Special case called: Single character");
                    specialPawns.First().TryGetComp<Comp_AbilityUser>().DisableAbilityUser();
                    return;
                }

                //Special case -- no special characters.
                if (specialPawns?.Count <= 0)
                    return;

                //Should we rebalance?
                int tryLimit = avgPawns + specialPawns.Count + 1;
                int initTryLimit = tryLimit;
                var tempAvgCombatPoints = new Dictionary<Pawn, float>(avgCombatPoints);
                var tempSpecCombatPoints = new Dictionary<Pawn, float>(specCombatPoints);
                var removedCharacters = new List<Pawn>();
                while (previousPoints < tempAvgCombatPoints.Sum(x => x.Value) + tempSpecCombatPoints.Sum(x => x.Value))
                {
                    //Log.Message("------------------");                                
                    //Log.Message("Rebalance Attempt # " + (initTryLimit - tryLimit + 1));
                    //Log.Message("------------------");
                    //Log.Message("Scenario Points: " + previousPoints + ". Total Points: " + tempAvgCombatPoints.Sum(x => x.Value) + tempSpecCombatPoints.Sum(x => x.Value));

                    //In-case some stupid stuff occurs
                    --tryLimit;
                    if (tryLimit < 0)
                        break;

                    //If special characters outnumber the avg characters, try removing some of the special characters instead.
                    if (tempSpecCombatPoints.Count >= tempAvgCombatPoints.Count)
                    {
                        var toRemove = tempSpecCombatPoints?.Keys?.RandomElement();
                        if (toRemove != null)
                        {
                            //Log.Message("Removed: " + toRemove.LabelShort + " : " + tempSpecCombatPoints[toRemove]);
                            removedCharacters.Add(toRemove);
                            tempSpecCombatPoints.Remove(toRemove);
                        }
                    }
                    //If average characters outnumber special characters, then check if the combat value of avg is greater.
                    else if (tempSpecCombatPoints.Count < tempAvgCombatPoints.Count)
                    {
                        //Remove a random average character if the average characters have more combat points for a score
                        if (tempAvgCombatPoints.Sum(x => x.Value) > tempSpecCombatPoints.Sum(x => x.Value))
                        {
                            var toRemove = tempAvgCombatPoints?.Keys?.RandomElement();
                            if (toRemove != null)
                            {
                                //Log.Message("Removed: " + toRemove.LabelShort + " : " + tempSpecCombatPoints[toRemove]);
                                removedCharacters.Add(toRemove);
                                tempAvgCombatPoints.Remove(toRemove);
                            }
                        }
                        else
                        {
                            var toRemove = tempSpecCombatPoints?.Keys?.RandomElement();
                            //Log.Message("Removed: " + toRemove.LabelShort + " : " + tempSpecCombatPoints[toRemove]);
                            if (toRemove != null)
                            {
                                removedCharacters.Add(toRemove);
                                tempSpecCombatPoints.Remove(toRemove);
                            }
                        }
                    }
                }
                avgCombatPoints = tempAvgCombatPoints;
                specCombatPoints = tempSpecCombatPoints;

                //                Log.Message("------------");                                
                //                Log.Message("Final Report");
                //                Log.Message("------------");
                //                Log.Message("Scenario Points: " + previousPoints + ". Total Points: " + tempAvgCombatPoints.Sum(x => x.Value) + tempSpecCombatPoints.Sum(x => x.Value));
                //                Log.Message("------------");
                //                Log.Message("Characters");
                //                Log.Message("------------------");
                __result.ForEach(x =>
                {
                    var combatValue = x.kindDef.combatPower + x?.TryGetComp<Comp_AbilityUser>()?.CombatPoints() ?? 0f;
                    //Log.Message(x.LabelShort + " : " + combatValue);
                });
                foreach (var x in removedCharacters)
                {
                    if (x.TryGetComp<Comp_AbilityUser>() is Comp_AbilityUser cu && cu.CombatPoints() > 0)
                        cu.DisableAbilityUser();
                    else x.DestroyOrPassToWorld();
                }
                removedCharacters.Clear();
                avgCombatPoints.Clear();
                specCombatPoints.Clear();
            }
        }


        //RimWorld v1.0.1964
        [HarmonyPatch(typeof(Verb), nameof(Verb.UIIcon))]
        public static bool get_UIIcon(Verb __instance, ref Texture2D __result)
        {
            if (__instance is Verb_UseAbility abilityVerb && abilityVerb.Ability.PowerButton is Texture2D tex)
            {
                __result = tex;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Verb_LaunchProjectile), nameof(Verb_LaunchProjectile.Projectile))]
        public static bool get_Projectile_Prefix(Verb_LaunchProjectile __instance, ref ThingDef __result)
        {
            if (__instance is Verb_UseAbility vua)
            {
                __result = __instance.verbProps.defaultProjectile;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Verb), nameof(Verb.DirectOwner))]
        public static bool get_DirectOwner_Prefix(Verb __instance, ref IVerbOwner __result)
        {
            if (__instance is Verb_UseAbility vua)
            {
                __result = __instance.CasterPawn;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Verb), nameof(Verb.TryStartCastOn))]
        public static bool TryStartCastOn_Prefix(Verb __instance, LocalTargetInfo castTarg, LocalTargetInfo destTarg, bool surpriseAttack, bool canHitNonTargetPawns, ref bool __result)
        {
            if (!(__instance is Verb_UseAbility vua))
                return true;
            else
            {
                var result = vua.PreCastShot(castTarg, destTarg, surpriseAttack, canHitNonTargetPawns);
                __result = result;
                return false;
            }
        }

        //// Catches loading of Pawns
        //public static void ExposeData_PostFix(Pawn __instance)
        //{ HarmonyPatches.internalAddInAbilityUsers(__instance); }

        //// Catches generation of Pawns
        //public static void GeneratePawn_PostFix(PawnGenerationRequest request, Pawn __result)
        //{ HarmonyPatches.internalAddInAbilityUsers(__result); }

        // Add in any AbilityUser Components, if the Pawn is accepting
        public static void InternalAddInAbilityUsers(Pawn pawn)
        {
            //            Log.Message("Trying to add AbilityUsers to Pawn");
            if (pawn != null && pawn.RaceProps != null && pawn.RaceProps.Humanlike)
                AbilityUserUtility.TransformPawn(pawn);
        }

    }
}
