using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace HRF
{
    public class Verb_Avatar : BaseVerb
    {
        public override bool TryCastShot()
        {
            var pawns = GenRadial.RadialDistinctThingsAround(caster.Position, caster.Map, Props.effectRadius, true)
                .OfType<Pawn>().Where(p => p.Faction == caster.Faction).Take(Props.maxTargets).ToList();
            Props.effectCount.SortBy(mcd => mcd.minCount);
            var minCountDef = Props.effectCount.Last(mcd => mcd.minCount <= pawns.Count);
            Thing thing;
            if (!minCountDef.spawnDef.NullOrEmpty())
                thing = ThingMaker.MakeThing(ThingDef.Named(minCountDef.spawnDef));
            else if (!minCountDef.pawnKindDef.NullOrEmpty())
                thing = PawnGenerator.GeneratePawn(PawnKindDef.Named(minCountDef.pawnKindDef), Faction.OfPlayer);
            else return false;
            GenSpawn.Spawn(thing, CurrentTarget.Cell, caster.Map);
            thing.SetFaction(caster.Faction);
            if (thing is Pawn pwn) pwn.drafter = pwn.drafter ?? new Pawn_DraftController(pwn);
            var tracker =
                (RemovalTracker) GenSpawn.Spawn(ThingDef.Named("RemovalTracker"), caster.Position, caster.Map);
            tracker.ToTrack = thing;
            tracker.ToRemove = pawns.Select(pawn => pawn.health.AddHediff(Props.effectHediff)).ToList();
            tracker.ToApply = Props.feedbackHediff;
            tracker.RemoveTick = Find.TickManager.TicksGame + Props.avatarDuration;
            return true;
        }

        public override void DrawHighlight(LocalTargetInfo target)
        {
            base.DrawHighlight(target);
            GenDraw.DrawRadiusRing(caster.Position, Props.effectRadius);
        }
    }

    public class RemovalTracker : Thing
    {
        private bool reachedTime;
        public int RemoveTick;
        public HediffDef ToApply;
        public List<Hediff> ToRemove;
        public Thing ToTrack;

        public override void Tick()
        {
            if (ToTrack is Pawn p && p.Dead)
            {
                p.Corpse.Destroy();
                ToTrack.Destroy();
            }

            if (Find.TickManager.TicksGame >= RemoveTick)
            {
                ToTrack.Destroy();
                reachedTime = true;
            }

            if (!ToTrack.Spawned || ToTrack.Destroyed) OnRemove();
        }

        private void OnRemove()
        {
            var pawns = new List<Pawn>();
            if (ToRemove != null)
                foreach (var hediff in ToRemove)
                {
                    pawns.Add(hediff.pawn);
                    hediff.pawn.health.RemoveHediff(hediff);
                }

            if (ToApply != null)
                foreach (var pawn in pawns)
                    pawn.health.AddHediff(ToApply);

            Destroy();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref RemoveTick, "removeTick");
            Scribe_Values.Look(ref reachedTime, "reachedTime");
            Scribe_References.Look(ref ToTrack, "toTrack");
            Scribe_Collections.Look(ref ToRemove, "toRemove", LookMode.Reference);
        }
    }
}