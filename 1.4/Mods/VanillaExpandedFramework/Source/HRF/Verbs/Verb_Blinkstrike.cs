using Verse;

namespace HRF
{
    public class Verb_Blinkstrike : BaseVerb
    {
        public TeleMarker Marker;

        public override bool Targetable => Props.blinkDuration < 0 || Marker == null || Marker.Destroyed;

        public override bool TryCastShot()
        {
            var cell = CurrentTarget.HasThing ? CurrentTarget.Thing.RandomAdjacentCellCardinal() : CurrentTarget.Cell;
            if (Marker != null && !Marker.Destroyed)
            {
                Marker.DoTeleport();
                return true;
            }

            if (Props.blinkDuration > 0)
            {
                Marker = (TeleMarker) GenSpawn.Spawn(ThingDef.Named("TeleportMarker"), caster.Position, caster.Map);
                Marker.Target = caster;
                Marker.EndTick = Find.TickManager.TicksGame + Props.blinkDuration.SecondsToTicks();
                Marker.Verb = this;
            }

            caster.Position = cell;
            if (CasterIsPawn) CasterPawn.Notify_Teleported();
            return true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref Marker, "marker");
        }
    }

    public class TeleMarker : Thing
    {
        public int EndTick;
        public Thing Target;
        public Verb_Blinkstrike Verb;

        public override void Tick()
        {
            if (EndTick > Find.TickManager.TicksGame) return;
            DoTeleport();
        }

        public void DoTeleport()
        {
            Target.Position = Position;
            Verb.Marker = null;
            if (Target is Pawn p) p.Notify_Teleported();
            Destroy();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref EndTick, "endTick");
            Scribe_References.Look(ref Target, "target");
            Scribe_References.Look(ref Verb, "verb");
        }
    }
}