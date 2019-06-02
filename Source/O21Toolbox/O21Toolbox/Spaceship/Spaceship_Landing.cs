using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace O21Toolbox.Spaceship
{
    [StaticConstructorOnStartup]
    public class Spaceship_Landing : Spaceship_Flying
    {
        public int horizontalTrajectoryDurationInTicks = 480;
        public int verticalTrajectoryDurationInTicks = 240;
        public int ticksToLanding;
        public IntVec3 landingPadPosition = IntVec3.Invalid;
        public Rot4 landingPadRotation = Rot4.North;
        public int landingDuration = 0;

        // Setup.
        public void InitializeParameters(Building_LandingPad landingPad, int landingDuration, SpaceshipDef spaceshipDef, SpaceshipKind spaceshipKind)
        {
            landingPad.Notify_ShipLanding();
            this.landingPadPosition = landingPad.Position;
            this.landingPadRotation = landingPad.Rotation;
            this.spaceshipExactRotation = this.landingPadRotation.AsAngle;
            this.landingDuration = landingDuration;
            this.spaceshipKind = spaceshipKind;
            this.thisSpaceshipDef = spaceshipDef;
            ConfigureSpaceshipTextures(this.thisSpaceshipDef);
            this.Tick(); // To update exact position for drawing purpose.
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksToLanding, "ticksToLanding");
            Scribe_Values.Look<IntVec3>(ref this.landingPadPosition, "landingPadPosition");
            Scribe_Values.Look<Rot4>(ref this.landingPadRotation, "landingPadRotation");
            Scribe_Values.Look<int>(ref this.landingDuration, "landingDuration");
        }

        public override void Tick()
        {
            base.Tick();

            if (this.ticksToLanding == horizontalTrajectoryDurationInTicks + verticalTrajectoryDurationInTicks && this.thisSpaceshipDef.preLandingSound != null)
            {
                // Atmosphere entry sound.
                this.thisSpaceshipDef.preLandingSound.PlayOneShot(new TargetInfo(this.Position, this.Map));
            }
            this.ticksToLanding--;
            if (this.ticksToLanding == verticalTrajectoryDurationInTicks && this.thisSpaceshipDef.landingSound != null)
            {
                // Landing on sound.
                this.thisSpaceshipDef.landingSound.PlayOneShot(new TargetInfo(this.Position, this.Map));
            }
            /** if (this.ticksToLanding <= verticalTrajectoryDurationInTicks)
            {
                // Throw dust during descent.
                MoteMaker.ThrowDustPuff(GenAdj.CellsAdjacentCardinal(this.landingPadPosition, this.landingPadRotation, Util_ThingDefOf.LandingPad.Size).RandomElement(), this.Map, 3f * (1f - (float)this.ticksToLanding / (float)verticalTrajectoryDurationInTicks));
            } **/
            if (this.ticksToLanding == 0)
            {
                // Spawn cargo spaceship.
                Spaceship_Building spaceship = ThingMaker.MakeThing(ThingDef.Named(this.thisSpaceshipDef.defName)) as Spaceship_Building;
                spaceship.InitializeParameters(this.Faction, this.HitPoints, this.landingDuration, this.thisSpaceshipDef, this.spaceshipKind);
                spaceship = GenSpawn.Spawn(spaceship, this.landingPadPosition, this.Map, this.landingPadRotation) as Spaceship_Building;

                this.Destroy();
            }
        }

        public override void ComputeShipExactPosition()
        {
            Vector3 exactPosition = this.landingPadPosition.ToVector3ShiftedWithAltitude(AltitudeLayer.Skyfaller);
            exactPosition += new Vector3(0f, 5.1f, 0f); // The 5f offset on Y axis is mandatory to be over the fog of war. The 0.1f is to ensure spaceship texture is above its shadow.
            // Horizontal position.
            if (this.ticksToLanding > verticalTrajectoryDurationInTicks)
            {
                // Horizontal trajectory.
                float coefficient = (float)(this.ticksToLanding - verticalTrajectoryDurationInTicks);
                float num = coefficient * coefficient * 0.001f * 0.8f;
                exactPosition -= new Vector3(0f, 0f, num).RotatedBy(this.spaceshipExactRotation);
            }
            this.spaceshipExactPosition = exactPosition;
        }

        public override void ComputeShipShadowExactPosition()
        {
            this.spaceshipShadowExactPosition = this.spaceshipExactPosition;
            float shadowDistanceCoefficient = 2f;
            if (this.ticksToLanding < verticalTrajectoryDurationInTicks)
            {
                // Ascending.
                shadowDistanceCoefficient *= ((float)this.ticksToLanding / verticalTrajectoryDurationInTicks);
            }
            GenCelestial.LightInfo lightInfo = GenCelestial.GetLightSourceInfo(this.Map, GenCelestial.LightType.Shadow);
            this.spaceshipShadowExactPosition += new Vector3(lightInfo.vector.x, -0.1f, lightInfo.vector.y) * shadowDistanceCoefficient;
        }

        public override void ComputeShipExactRotation()
        {
            // Always equal to the landing pad rotation.
        }

        public override void ComputeShipScale()
        {
            // Default value for horizontal trajectory and rotation.
            float coefficient = 1.2f;
            float shadowCoefficient = 0.9f;

            if (this.ticksToLanding <= verticalTrajectoryDurationInTicks)
            {
                // Descent.
                coefficient = 1f + 0.2f * ((float)this.ticksToLanding / verticalTrajectoryDurationInTicks);
                shadowCoefficient = 1f - 0.1f * ((float)this.ticksToLanding / verticalTrajectoryDurationInTicks);
            }
            this.spaceshipScale = this.baseSpaceshipScale * coefficient;
            this.spaceshipShadowScale = this.baseSpaceshipScale * shadowCoefficient;
        }

        // ===================== Draw =====================
        public override void SetShipVisibleAboveFog()
        {
            if (IsInBoundsAndVisible())
            {
                this.Position = this.spaceshipExactPosition.ToIntVec3();
            }
            else
            {
                this.Position = this.landingPadPosition;
            }
        }
    }
}
