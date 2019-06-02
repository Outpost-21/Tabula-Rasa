using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;

namespace O21Toolbox.Spaceship
{
    public class Util_Spaceship
    {
        public static Pawn GetRandomReachableDownedPawn(Pawn carrier)
        {
            foreach (Pawn downedPawn in carrier.Map.mapPawns.FreeHumanlikesSpawnedOfFaction(carrier.Faction))
            {
                if (downedPawn.Downed)
                {
                    if (carrier.CanReserveAndReach(downedPawn, PathEndMode.OnCell, Danger.Some))
                    {
                        return downedPawn;
                    }
                }
            }
            return null;
        }

        public static Pawn GetNearestReachableDownedPawn(Pawn carrier)
        {
            Pawn nearestDownedPawn = null;
            float minDistance = 99999f;
            foreach (Pawn downedPawn in carrier.Map.mapPawns.FreeHumanlikesSpawnedOfFaction(carrier.Faction))
            {
                if (downedPawn.Downed)
                {
                    if (carrier.CanReserveAndReach(downedPawn, PathEndMode.OnCell, Danger.Some))
                    {
                        float distance = IntVec3Utility.DistanceTo(carrier.Position, downedPawn.Position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestDownedPawn = downedPawn;
                        }
                    }
                }
            }
            return nearestDownedPawn;
        }

        public static bool TryFindRandomExitSpot(Map map, IntVec3 startSpot, out IntVec3 exitSpot)
        {
            Predicate<IntVec3> validator = delegate (IntVec3 cell)
            {
                return ((cell.Fogged(map) == false)
                    && (map.roofGrid.Roofed(cell) == false)
                    && cell.Standable(map)
                    && map.reachability.CanReach(startSpot, cell, PathEndMode.Touch, TraverseMode.PassDoors, Danger.Some));
            };
            return CellFinder.TryFindRandomEdgeCellWith(validator, map, CellFinder.EdgeRoadChance_Always, out exitSpot);
        }

        public static void RandomlyDamagePawn(Pawn pawn, int injuriesNumber, int damageAmount)
        {

            if (pawn.story.traits.HasTrait(TraitDef.Named("Wimp")))
            {
                // Do not hurt wimp pawns as they could be spawned as dead and break the lord behavior.
                return;
            }
            HediffSet hediffSet = pawn.health.hediffSet;
            int injuriesIndex = 0;
            while ((pawn.Dead == false) && (injuriesIndex < injuriesNumber) && HittablePartsViolence(hediffSet).Any<BodyPartRecord>())
            {
                injuriesIndex++;
                BodyPartRecord bodyPartRecord = HittablePartsViolence(hediffSet).RandomElementByWeight((BodyPartRecord x) => x.coverageAbs);
                DamageDef def;
                if (bodyPartRecord.depth == BodyPartDepth.Outside)
                {
                    def = HealthUtility.RandomViolenceDamageType();
                }
                else
                {
                    def = DamageDefOf.Blunt;
                }
                BodyPartRecord forceHitPart = bodyPartRecord;
                DamageInfo dinfo = new DamageInfo(def, damageAmount, 0f, -1f, null, forceHitPart, null, DamageInfo.SourceCategory.ThingOrUnknown);
                pawn.TakeDamage(dinfo);
            }
        }

        // Copied from Verse.HealthUtility.
        private static IEnumerable<BodyPartRecord> HittablePartsViolence(HediffSet bodyModel)
        {
            return from x in bodyModel.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined)
                   where x.depth == BodyPartDepth.Outside || (x.depth == BodyPartDepth.Inside && x.def.IsSolid(x, bodyModel.hediffs))
                   select x;
        }

        public static Spaceship_Landing SpawnLandingSpaceship(Building_LandingPad landingPad, SpaceshipDef spaceshipDef, SpaceshipKind spaceshipKind)
        {
            Building_OrbitalRelay orbitalRelay = Util_OrbitalRelay.GetOrbitalRelay(landingPad.Map);
            int landingDuration = 0;
            {
                    landingDuration = spaceshipDef.landingPeriod;
                    if (orbitalRelay != null)
                    {
                        orbitalRelay.Notify_SpaceshipLanding();
                    }
                    Messages.Message("Incoming airborne vehicle.", new TargetInfo(landingPad.Position, landingPad.Map), MessageTypeDefOf.NeutralEvent);
            }

            Spaceship_Landing flyingSpaceship = ThingMaker.MakeThing(ThingDef.Named(spaceshipDef.defName + "_Landing")) as Spaceship_Landing;
            GenSpawn.Spawn(flyingSpaceship, landingPad.Position, landingPad.Map, landingPad.Rotation);
            flyingSpaceship.InitializeParameters(landingPad, landingDuration, spaceshipDef, spaceshipKind);
            return flyingSpaceship;
        }

        public static void SpawnStrikeShip(Map map, IntVec3 targetPosition, SpaceshipDef spaceshipDef, SpaceshipKind spaceshipKind, AirStrikeDef airStrikeDef)
        {
            //Spaceship_Airstrike strikeShip = ThingMaker.MakeThing(ThingDef.Named(spaceshipDef.defName + "_Airstrike")) as Spaceship_Airstrike;
            //GenSpawn.Spawn(strikeShip, targetPosition, map);
            //strikeShip.InitializeParamaters(targetPosition, airStrikeDef);
        } 
    }
}
