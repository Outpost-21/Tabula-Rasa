using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace HRF
{
    public class Verb_Extinguish : BaseVerb
    {
        public override int ShotsPerBurst => verbProps.burstShotCount;

        public override bool TryCastShot()
        {
            (GenSpawn.Spawn(ThingDef.Named("Extinguishing"), caster.Position, caster.Map) as Extinguishing)?.Start(
                Props.extinguishRadius);
            verbProps.defaultCooldownTime = Props.extinguishRadius * 6f;
            return true;
        }
    }

    public class Extinguishing : Thing
    {
        private List<IntVec3> cells = new List<IntVec3>();
        private float maxRadius;
        private int startTick;

        public void Start(float r)
        {
            maxRadius = r;
            startTick = Find.TickManager.TicksGame;
            foreach (var cell in GenRadial.RadialPatternInRadius(r)) cells.Add(Position + cell);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref startTick, "startTick");
            Scribe_Values.Look(ref maxRadius, "maxRadius");
            Scribe_Collections.Look(ref cells, "cells");
        }

        public override void Tick()
        {
            cells.RemoveAll(cell =>
            {
                if (startTick + (cell - Position).LengthHorizontal * 6f > Find.TickManager.TicksGame)
                    return false;
                AffectCell(cell);
                return true;
            });
            if (!cells.Any()) Destroy();
        }

        public void AffectCell(IntVec3 cell)
        {
            foreach (var fire in cell.GetThingList(Map).OfType<Fire>())
            {
                fire.fireSize -= 0.1f;

                if (fire.fireSize <= 0.1f) fire.Destroy();
            }

            FilthMaker.TryMakeFilth(cell, Map, ThingDefOf.Filth_FireFoam, 3);
            var moteDef = ThingDef.Named("Mote_BlastExtinguisher");

            if (cell.GetFirstThing(Map, moteDef) is Mote mote)
                mote.spawnTick = Find.TickManager.TicksGame;
            else
                MoteMaker.ThrowExplosionCell(cell, Map, moteDef,
                    Color.Lerp(Color.white, new Color(1, 1, 1, 0.05f),
                        Mathf.Clamp01((Position - cell).LengthHorizontal / maxRadius)));
        }
    }
}