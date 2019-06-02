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
    public class Spaceship_TakingOff : Spaceship_Flying
    {
        public int horizontalTrajectoryDurationInTicks = 480;
        public int verticalTrajectoryDurationInTicks = 240;

        public int ticksSinceTakeOff = 0;
        public IntVec3 landingPadPosition = IntVec3.Invalid;
        public Rot4 landingPadRotation = Rot4.North;

        // Sound
        public SoundDef takingOffSound = null;

        // Setup
        public void InitializeParameters(IntVec3 position, Rot4 rotation, SpaceshipDef thisSpaceshipDef, SpaceshipKind spaceshipKind)
        {
            this.landingPadPosition = position;
            this.landingPadRotation = rotation;
            this.spaceshipExactRotation = this.landingPadRotation.AsAngle;
            this.thisSpaceshipDef = thisSpaceshipDef;
            this.horizontalTrajectoryDurationInTicks = this.thisSpaceshipDef.flightSpeedH;
            this.verticalTrajectoryDurationInTicks = this.thisSpaceshipDef.flightSpeedV;
            this.takingOffSound = this.thisSpaceshipDef.takeoffSound;
            ConfigureSpaceshipTextures(this.thisSpaceshipDef);
            base.Tick();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksSinceTakeOff, "ticksSinceTakeOff");
            Scribe_Values.Look<IntVec3>(ref this.landingPadPosition, "landingPadPosition");
            Scribe_Values.Look<Rot4>(ref this.landingPadRotation, "landingPadRotation");
        }

        // Main Functions

        public override void Tick()
        {
            base.Tick();

            this.ticksSinceTakeOff++;
            /** if (this.ticksSinceTakeOff <= verticalTrajectoryDurationInTicks)
            {
                MoteMaker.ThrowDustPuff(GenAdj.CellsAdjacentCardinal(this.landingPadPosition, this.landingPadRotation, Util_ThingDefOf.LandingPad.Size).RandomElement(), this.Map, 3f * (1f - (float)this.ticksSinceTakeOff / (float)verticalTrajectoryDurationInTicks));
            } **/
            if (this.ticksSinceTakeOff == 1 && this.takingOffSound != null)
            {
                // Taking off sound.
                this.takingOffSound.PlayOneShot(new TargetInfo(this.Position, this.Map));
            }
            if (this.ticksSinceTakeOff >= verticalTrajectoryDurationInTicks + horizontalTrajectoryDurationInTicks)
            {
                this.Destroy();
            }
        }

        public override void ComputeShipExactPosition()
        {
            Vector3 exactPosition = this.landingPadPosition.ToVector3ShiftedWithAltitude(AltitudeLayer.Skyfaller);
            exactPosition += new Vector3(0f, 5.1f, 0f); // The 5f offset on Y axis is mandatory to be over the fog of war. The 0.1f is to ensure spaceship texture is above its shadow.
            // Horizontal position.
            if (this.ticksSinceTakeOff >= verticalTrajectoryDurationInTicks)
            {
                // Horizontal trajectory.
                float coefficient = (float)(this.ticksSinceTakeOff - verticalTrajectoryDurationInTicks);
                float num = coefficient * coefficient * 0.001f * 0.8f;
                exactPosition += new Vector3(0f, 0f, num).RotatedBy(this.spaceshipExactRotation);
            }
            this.spaceshipExactPosition = exactPosition;
        }

        public override void ComputeShipShadowExactPosition()
        {
            this.spaceshipShadowExactPosition = this.spaceshipExactPosition;
            float shadowDistanceCoefficient = 2f;
            if (this.ticksSinceTakeOff < verticalTrajectoryDurationInTicks)
            {
                // Ascending.
                shadowDistanceCoefficient *= ((float)this.ticksSinceTakeOff / verticalTrajectoryDurationInTicks);
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
            float shadowCoefficient = 0.8f;

            if (this.ticksSinceTakeOff < verticalTrajectoryDurationInTicks)
            {
                // Ascending.
                coefficient = 1f + 0.2f * ((float)this.ticksSinceTakeOff / verticalTrajectoryDurationInTicks);
                shadowCoefficient = 1f - 0.2f * ((float)this.ticksSinceTakeOff / verticalTrajectoryDurationInTicks);
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
