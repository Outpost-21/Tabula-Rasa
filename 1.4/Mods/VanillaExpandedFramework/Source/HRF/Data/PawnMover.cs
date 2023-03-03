using RimWorld;
using UnityEngine;
using Verse;

namespace HRF
{
    public class PawnMover : PawnFlyer
    {
        protected Vector3 effectivePos;
        private IntVec3 lastPos;
        private int positionLastComputedTick;

        public override Vector3 DrawPos
        {
            get
            {
                RecomputePosition();
                return effectivePos;
            }
        }

        public void End()
        {
            if (FlyingPawn == null) Destroy();
            RecomputePosition();
            Position = IntVec3.FromVector3(effectivePos);
            RespawnPawn();
            Destroy();
        }

        public override void Tick()
        {
            base.Tick();
            if (FlyingPawn != null) RecomputePosition();
            var pos = IntVec3.FromVector3(effectivePos);
            if (pos != lastPos)
            {
                AffectCell(pos);
                lastPos = pos;
            }
        }

        public virtual void AffectCell(IntVec3 cell)
        {
        }

        protected bool CheckRecompute()
        {
            if (positionLastComputedTick == ticksFlying) return false;

            positionLastComputedTick = ticksFlying;
            return true;
        }

        protected virtual void RecomputePosition()
        {
            if (CheckRecompute()) return;
            effectivePos = Vector3.Lerp(startVec, DestinationPos, ticksFlying / (float) ticksFlightTime);
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            FlyingPawn.DrawAt(drawLoc, flip);
        }
    }
}