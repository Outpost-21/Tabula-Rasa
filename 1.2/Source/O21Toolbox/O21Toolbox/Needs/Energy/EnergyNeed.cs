using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace O21Toolbox.Needs
{
    /// <summary>
    /// Energy Need for Pawns
    /// </summary>
    public class Need_Energy : Need
    {
        /// <summary>
        /// The percentage level at which we should attempt recharging.
        /// </summary>
        public static float rechargePercentage = 0.505f;

        public float EnergyDrainRate
        {
            get
            {
                float drainModifier = 1f;
                if ((!pawn.IsCaravanMember() && pawn.TryGetComp<Comp_EnergyTracker>() is Comp_EnergyTracker energyTracker && energyTracker.EnergyProperties.canHibernate && pawn.CurJobDef == energyTracker.EnergyProperties.hibernationJob) || pawn.IsPrisoner || pawn.Faction != Faction.OfPlayer)
                {
                    drainModifier = -0.1f;
                }

                float result = ((1f / 1200f) * pawn.TryGetComp<Comp_EnergyTracker>().EnergyProperties.baseEnergyDecayRate) * pawn.GetStatValue(Utility.StatDefOf.O21_EnergyConsuptionRate);

                return result * drainModifier;
            }
        }

        public Need_Energy(Pawn pawn)
        {
            this.pawn = pawn;
        }

        public override float MaxLevel => 1.0f;

        /// <summary>
        /// Start with Energy maxed.
        /// </summary>
        public override void SetInitialLevel()
        {
            CurLevel = 1.0f;
        }

        public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = int.MaxValue, float customMargin = -1, bool drawArrows = true, bool doTooltip = true)
        {
            if(threshPercents == null)
            {
                threshPercents = new List<float>();
            }
            threshPercents.Clear();
            threshPercents.Add(0.5f);
            threshPercents.Add(0.2f);

            base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
        }

        public override void NeedInterval()
        {

            if (pawn.TryGetComp<Comp_EnergyTracker>() != null)
            {
                foreach (Need need in pawn.needs.AllNeeds)
                {
                    if (pawn.def.HasModExtension<ArtificialPawnProperties>())
                    {
                        ArtificialPawnProperties modExt = pawn.def.GetModExtension<ArtificialPawnProperties>();

                        if (need.def == NeedDefOf.Rest && !modExt.needRest)
                        {
                            need.CurLevelPercentage = 1f;
                        }
                        else if (need.def == NeedDefOf.Food && !modExt.needFood)
                        {
                            need.CurLevelPercentage = 1f;
                        }
                        else if (need.def == NeedDefOf.Joy && !modExt.needJoy)
                        {
                            need.CurLevelPercentage = 1f;
                        }
                        else if (need.def == NeedsDefOf.Beauty && !modExt.needBeauty)
                        {
                            need.CurLevelPercentage = 1f;
                        }
                        else if (need.def == NeedsDefOf.Comfort && !modExt.needComfort)
                        {
                            need.CurLevelPercentage = 1f;
                        }
                        else if (need.def == NeedsDefOf.RoomSize && !modExt.needRoomSize)
                        {
                            need.CurLevelPercentage = 1f;
                        }
                        else if (need.def == NeedsDefOf.Outdoors && !modExt.needOutdoors)
                        {
                            need.CurLevelPercentage = 1f;
                        }
                    }
                    else if (need.def != NeedsDefOf.O21Energy)
                    {
                        need.CurLevelPercentage = 1f;
                    }
                }
            }

            if (pawn.needs != null && !IsFrozen)
            {
                //Normal drain
                CurLevel -= EnergyDrainRate;

                //Energy gain from having a food need
                if (pawn.needs.food != null && pawn.needs.food.CurLevelPercentage > 0.0f)
                {
                    CurLevel += 1f / 75f;
                }

                //Energy gain from apparel
                if (pawn.apparel != null)
                {
                    foreach (Apparel apparel in pawn.apparel.WornApparel)
                    {
                        Comp_EnergySource energySourceComp = apparel.TryGetComp<Comp_EnergySource>();
                        if (energySourceComp != null && !energySourceComp.EnergyProps.isConsumable)
                        {
                            energySourceComp.RechargeEnergyNeed(pawn);
                        }
                    }
                }

                //Energy gain in caravan.
                if (pawn.IsCaravanMember() && CurLevelPercentage < rechargePercentage)
                {
                    Caravan caravan = pawn.GetCaravan();
                    Thing validEnergySource =
                        caravan.Goods.FirstOrDefault(
                            thing =>
                            thing.TryGetComp<Comp_EnergySource>() is Comp_EnergySource energySource &&
                            energySource.EnergyProps.isConsumable
                            );
                    if (validEnergySource != null)
                    {
                        //Use enough to get satisfied.
                        Comp_EnergySource energySourceComp = validEnergySource.TryGetComp<Comp_EnergySource>();

                        int thingCount = (int)Math.Ceiling((MaxLevel - CurLevel) / energySourceComp.EnergyProps.energyWhenConsumed);
                        thingCount = Math.Min(thingCount, validEnergySource.stackCount);

                        Thing splitOff = validEnergySource.SplitOff(thingCount);
                        Comp_EnergySource energySourceCompSplit = splitOff.TryGetComp<Comp_EnergySource>();
                        energySourceCompSplit.RechargeEnergyNeed(pawn);
                        splitOff.Destroy(DestroyMode.Vanish);
                    }
                }

                //Slow down.
                if (CurLevel < 0.2f)
                {
                    if (!pawn.health.hediffSet.HasHediff(pawn.def.GetModExtension<EnergyHediffs>().powerShortage))
                        pawn.health.AddHediff(pawn.def.GetModExtension<EnergyHediffs>().powerShortage);
                }
                else
                {
                    if (pawn.health.hediffSet.HasHediff(pawn.def.GetModExtension<EnergyHediffs>().powerShortage))
                        pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(pawn.def.GetModExtension<EnergyHediffs>().powerShortage)); ;
                }

                //Point beyond no return.
                if (CurLevel <= 0f)
                {
                    //Die
                    Hediff exactCulprit = HediffMaker.MakeHediff(pawn.def.GetModExtension<EnergyHediffs>().powerFailure, pawn);
                    pawn.health.AddHediff(exactCulprit);
                    pawn.Kill(null, exactCulprit);
                }
            }
        }
    }
}
