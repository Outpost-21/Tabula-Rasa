using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using RimWorld.Planet;

namespace O21Toolbox.Needs
{
    public class Need_Solar : Need
    {
        protected float needFallPerTick => (-2.6666667E-05f) * solarNeedExt.consumeRate;

        private int lastNonStarvingTick = -99999;
        public override float MaxLevel => 1.0f;

        private bool InSunlight = false;

        public Need_Solar(Pawn pawn)
        {
            this.pawn = pawn;
        }

        protected DefModExt_SolarNeed solarNeedExt
        {
            get
            {
                return this.pawn.def.GetModExtension<DefModExt_SolarNeed>() ?? null;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.lastNonStarvingTick, "lastNonStarvingTick", -99999, false);
        }

        public override void SetInitialLevel()
        {
            if (this.pawn.RaceProps.Humanlike)
            {
                base.CurLevelPercentage = 0.8f;
            }
            else
            {
                base.CurLevelPercentage = Rand.Range(0.5f, 0.9f);
            }
            if (Current.ProgramState == ProgramState.Playing)
            {
                this.lastNonStarvingTick = Find.TickManager.TicksGame;
            }
        }

        public override void NeedInterval()
        {
            if (!base.IsFrozen)
            {
                if (this.pawn.IsCaravanMember())
                {
                    //Don't change need while in caravan. No point in overcomplicating matters.
                }
                else
                {
                    this.CurLevel += (float)this.NeedChangePerTick();
                }
            }

            if (!this.Starving)
            {
                this.lastNonStarvingTick = Find.TickManager.TicksGame;
            }

            if (!base.IsFrozen)
            {
                if (this.Starving)
                {
                    HealthUtility.AdjustSeverity(this.pawn, HediffDefOf.Malnutrition, this.MalnutritionSeverityPerInterval);
                }
                else
                {
                    HealthUtility.AdjustSeverity(this.pawn, HediffDefOf.Malnutrition, -this.MalnutritionSeverityPerInterval);
                }
            }

        }
        protected bool Resting
        {
            get
            {
                return solarNeedExt.restAtNight && (GenLocalDate.DayPercent(this.pawn) < 0.25f || GenLocalDate.DayPercent(this.pawn) > 0.8f);
            }
        }

        public float NeedChangePerTick()
        {
            float result = 0f;
            if (!Resting)
            {
                result += needFallPerTick;
            }
            if(this.pawn.Spawned && this.solarNeedExt != null)
            {
                if (this.pawn.Position.UsesOutdoorTemperature(this.pawn.Map))
                {
                    result += this.GetSunlightPercentage * (this.solarNeedExt.absorbRate * 4);
                }
                else if(!this.solarNeedExt.naturalOnly)
                {
                    result += this.solarNeedExt.artificialRate * NeedRateFactorInLight;
                }
            }

            if(result >= 0)
            {
                InSunlight = true;
            }
            else
            {
                InSunlight = false;
            }
            //Log.Message("Solar Need Change for " + pawn.Name + ": " + result);
            return result;
        }
        public float NeedRateFactorInLight
        {
            get
            {
                float num = this.pawn.Map.glowGrid.GameGlowAt(this.pawn.Position, false);
                if (this.solarNeedExt.minArtificialGlow == this.solarNeedExt.optimalArtificialGlow && num == this.solarNeedExt.optimalArtificialGlow)
                {
                    return 1f;
                }
                return GenMath.InverseLerp(this.solarNeedExt.minArtificialGlow, this.solarNeedExt.optimalArtificialGlow, num);
            }
        }

        protected float GetSunlightPercentage
        {
            get
            {
                return Mathf.Lerp(0f, MaxLevel, this.pawn.Map.skyManager.CurSkyGlow);
            }
        }

        public bool IsUndergrounder()
        {
            return this.pawn.story.traits.HasTrait(TraitDefOf.Undergrounder);
        }

        public override void DrawOnGUI(Rect rect, int maxThresholdMarkers = 2147483647, float customMargin = -1f, bool drawArrows = true, bool doTooltip = true)
        {
            if (this.threshPercents == null)
            {
                this.threshPercents = new List<float>();
            }
            this.threshPercents.Clear();
            this.threshPercents.Add(this.PercentageThreshHungry);
            this.threshPercents.Add(this.PercentageThreshUrgentlyHungry);
            base.DrawOnGUI(rect, maxThresholdMarkers, customMargin, drawArrows, doTooltip);
        }

        public float PercentageThreshHungry
        {
            get
            {
                return 0.4f;
            }
        }

        public float PercentageThreshUrgentlyHungry
        {
            get
            {
                return 0.2f;
            }
        }

        public bool Starving
        {
            get
            {
                return this.CurLevel <= 0;
            }
        }
        public override int GUIChangeArrow
        {
            get
            {
                return (!this.InSunlight) ? -1 : 1;
            }
        }
        private float MalnutritionSeverityPerInterval
        {
            get
            {
                return 0.0011333333f * Mathf.Lerp(0.8f, 1.2f, Rand.ValueSeeded(this.pawn.thingIDNumber ^ 2551674));
            }
        }
    }
}
