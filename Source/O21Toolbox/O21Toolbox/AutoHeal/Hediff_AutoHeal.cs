using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Utility;

namespace O21Toolbox.AutoHeal
{
    public class Hediff_AutoHeal : HediffWithComps
    {
        public int ticksUntilNextHeal;
        public int ticksUntilNextGrow;

        public override void PostMake()
        {
            base.PostMake();
            this.SetNextHealTick();
            this.SetNextGrowTick();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksUntilNextHeal, "ticksUntilNextHeal", 0, false);
        }

        public override void Tick()
        {
            base.Tick();
            if (Current.Game.tickManager.TicksGame >= this.ticksUntilNextHeal)
            {
                this.TrySealWounds();
                this.SetNextHealTick();
            }
            if (Current.Game.tickManager.TicksGame >= this.ticksUntilNextGrow && this.def.TryGetModExtension<DefModExtension_AutoHealProps>().regrowParts)
            {
                this.TryRegrowBodyparts();
                this.SetNextGrowTick();
            }
        }

        public void TrySealWounds()
        {
            IEnumerable<Hediff> enumerable = from hd in this.pawn.health.hediffSet.hediffs
                                                where hd.TendableNow()
                                                select hd;
            bool flag = enumerable != null;
            if (flag)
            {
                foreach (Hediff hediff in enumerable)
                {
                    HediffWithComps hediffWithComps = hediff as HediffWithComps;
                    bool flag2 = hediffWithComps != null;
                    if (flag2)
                    {
                        HediffComp_TendDuration hediffComp_TendDuration = HediffUtility.TryGetComp<HediffComp_TendDuration>(hediffWithComps);
                        hediffComp_TendDuration.tendQuality = 2f;
                        hediffComp_TendDuration.tendTicksLeft = Find.TickManager.TicksGame;
                        this.pawn.health.Notify_HediffChanged(hediff);
                    }
                }
            }
        }

        public void TryRegrowBodyparts()
        {
            if (this.def.TryGetModExtension<DefModExtension_AutoHealProps>().protoBodyPart != null)
            {
                using (IEnumerator<BodyPartRecord> enumerator = this.pawn.GetFirstMatchingBodyparts(this.pawn.RaceProps.body.corePart, HediffDefOf.MissingBodyPart, this.def.TryGetModExtension<DefModExtension_AutoHealProps>().protoBodyPart, (Hediff hediff) => hediff is Hediff_AddedPart).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        BodyPartRecord part = enumerator.Current;
                        Hediff hediff2 = this.pawn.health.hediffSet.hediffs.First((Hediff hediff) => hediff.Part == part && hediff.def == HediffDefOf.MissingBodyPart);
                        bool flag = hediff2 != null;
                        if (flag)
                        {
                            this.pawn.health.RemoveHediff(hediff2);
                            this.pawn.health.AddHediff(this.def.TryGetModExtension<DefModExtension_AutoHealProps>().protoBodyPart, part, null, null);
                            this.pawn.health.hediffSet.DirtyCache();
                        }
                    }
                }
            }
            else
            {
                using (IEnumerator<BodyPartRecord> enumerator = this.pawn.GetFirstMatchingBodyparts(this.pawn.RaceProps.body.corePart, HediffDefOf.MissingBodyPart, HediffDefOf_AutoHeal.AutoHeal_ProtoBodypart, (Hediff hediff) => hediff is Hediff_AddedPart).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        BodyPartRecord part = enumerator.Current;
                        Hediff hediff2 = this.pawn.health.hediffSet.hediffs.First((Hediff hediff) => hediff.Part == part && hediff.def == HediffDefOf.MissingBodyPart);
                        bool flag = hediff2 != null;
                        if (flag)
                        {
                            this.pawn.health.RemoveHediff(hediff2);
                            this.pawn.health.AddHediff(HediffDefOf_AutoHeal.AutoHeal_ProtoBodypart, part, null, null);
                            this.pawn.health.hediffSet.DirtyCache();
                        }
                    }
                }
            }
        }

        public void SetNextHealTick()
        {
            this.ticksUntilNextHeal = Current.Game.tickManager.TicksGame + this.def.TryGetModExtension<DefModExtension_AutoHealProps>().healTicks;
        }

        public void SetNextGrowTick()
        {
            this.ticksUntilNextGrow = Current.Game.tickManager.TicksGame + this.def.TryGetModExtension<DefModExtension_AutoHealProps>().growthTicks;
        }
    }
}
