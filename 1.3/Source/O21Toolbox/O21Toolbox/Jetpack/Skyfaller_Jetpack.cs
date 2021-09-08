using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace O21Toolbox.Jetpack
{
    public class Skyfaller_Jetpack : Skyfaller
    {
        public readonly SoundDef jumpSound = DefDatabase<SoundDef>.GetNamed("O21_JetPack");

        public int ticksToHeadeache;

        public override void Tick()
        {
            innerContainer.ThingOwnerTick(true);
            ticksToImpact--;
            ticksToHeadeache++;
            if(ticksToImpact % 3 == 0)
            {
                int numMotes = Math.Min(2, this.def.skyfaller.motesPerCell);
                for (int i = 0; i < numMotes; i++)
                {
                    FleckMaker.ThrowSmoke(base.DrawPos, base.Map, 2f);
                }
            }
            if(ticksToImpact % 25 == 0 && jumpSound != null)
            {
                jumpSound.PlayOneShot(new TargetInfo(base.DrawPos.ToIntVec3(), base.Map, false));
            }
            if (ticksToHeadeache == 10)
            {
                JetpackHitRoof(true);
            }
            if (ticksToImpact == 15)
            {
                JetpackHitRoof(false);
            }
            if(!anticipationSoundPlayed && def.skyfaller.anticipationSound != null && ticksToImpact < def.skyfaller.anticipationSoundTicks)
            {
                anticipationSoundPlayed = true;
                def.skyfaller.anticipationSound.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
            }
            if (ticksToImpact == 3)
            {
                EjectPilot();
            }
            if(ticksToImpact == 0)
            {
                // Impact is not implemented yet. May never be.
                //JetpackImpact();
            }

            if(ticksToImpact < 0)
            {
                //Log.Error("ticksToImpact < 0. Was there an exception? Destroying Skyfaller.");
                EjectPilot();
                Destroy(DestroyMode.Vanish);
            }
        }

        public void EjectPilot()
        {
            Thing pilot = GetThingForGraphic();
            if(pilot != null)
            {
                GenPlace.TryPlaceThing(pilot, Position, Map, ThingPlaceMode.Near, delegate (Thing thing, int count)
                {
                    PawnUtility.RecoverFromUnwalkablePositionOrKill(thing.Position, thing.Map);
                    if (thing.def.Fillage == FillCategory.Full && def.skyfaller.CausesExplosion && thing.Position.InHorDistOf(Position, def.skyfaller.explosionRadius))
                    {
                        Map.terrainGrid.Notify_TerrainDestroyed(thing.Position);
                    }
                    Pawn pawn = thing as Pawn;
                    if(pawn != null && pawn.Faction == Faction.OfPlayer)
                    {
                        pawn.drafter.Drafted = true;
                    }
                    JetpackApplyCooldown(thing);
                });
            }
        }

        public void JetpackApplyCooldown(Thing pilot)
        {
            Apparel_Jetpack jetpack = GetWornJetpack(pilot);
            jetpack.jetpackCooldownTicks = jetpack.comp.Props.cooldownTicks;
        }

        public Apparel_Jetpack GetWornJetpack(Thing pilot)
        {
            Apparel_Jetpack jetpack = null;
            Pawn pawn = pilot as Pawn;
            if(pawn != null && !pawn.apparel.WornApparel.NullOrEmpty())
            {
                jetpack = (Apparel_Jetpack)pawn.apparel.WornApparel.Find(a => a is Apparel_Jetpack);
            }
            return jetpack;
        }

        public void JetpackHitRoof(bool goingUp)
        {
            if (def.skyfaller.hitRoof)
            {
                CellRect cr;
                if (goingUp)
                {
                    IntVec3 impactCell = DrawPos.ToIntVec3();
                    IntVec2 impactSize = new IntVec2(3, 3);
                    cr = GenAdj.OccupiedRect(impactCell, Rotation, impactSize);
                }
                else
                {
                    cr = this.OccupiedRect();
                }
                if (cr.Cells.Any(x => x.Roofed(Map)))
                {
                    RoofDef roof = cr.Cells.First(x => x.Roofed(Map)).GetRoof(Map);
                    if (!roof.soundPunchThrough.NullOrUndefined())
                    {
                        roof.soundPunchThrough.PlayOneShot(new TargetInfo(Position, Map, false));
                    }
                    RoofCollapserImmediate.DropRoofInCells(cr.ExpandedBy(1).ClipInsideMap(Map).Cells.Where(delegate (IntVec3 c)
                    {
                        if (!c.InBounds(Map))
                        {
                            return false;
                        }
                        if (!cr.Contains(c))
                        {
                            return false;
                        }
                        if (c.GetFirstPawn(Map) != null)
                        {
                            return false;
                        }
                        if (c.GetEdifice(Map) != null || c.GetEdifice(Map).def.holdsRoof)
                        {
                            return false;
                        }
                        return true;
                    }), Map, null);
                }
            }
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            Pawn pilot = GetThingForGraphic() as Pawn;
            if(pilot != null)
            {
                new PawnRenderer(pilot).RenderPawnAt(drawLoc);
            }
        }

        public new Thing GetThingForGraphic()
        {
            Thing pilot = null;
            if (innerContainer.Any && innerContainer.Count > 0)
            {
                for (int i = 0; i < innerContainer.Count; i++)
                {
                    Thing thingchk = innerContainer[i];
                    if (thingchk is Pawn)
                    {
                        pilot = thingchk;
                    }
                }
            }
            return pilot as Pawn;
        }
    }
}
