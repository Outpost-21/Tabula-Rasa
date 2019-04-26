using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;

namespace O21Toolbox.Spaceship
{
    public class Building_OrbitalRelay : Building
    {
        public bool landingPadIsAvailable = false;

        // Power
        public CompPowerTrader powerComp = null;

        // Sound
        public Sustainer rotationSoundSustainer = null;

        // Relay Data
        public Comp_OrbitalRelay orbitalRelayComp = null;

        // Dish periodical rotation
        public const float dishRotationPerTick = 0.06f;
        public const int rotationIntervalMin = 1200;
        public const int rotationIntervalMax = 2400;
        public int ticksToNextRotation = rotationIntervalMin;
        public const int rotationDurationMin = 500;
        public const int rotationDurationMax = 1500;
        public int ticksToRotationEnd = 0;
        public bool clockwiseRotation = true;

        public float dishRotation = 0f;
        public Matrix4x4 dishMatrix = default(Matrix4x4);

        public bool canUseConsoleNow
        {
            get
            {
                return (!this.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare) && this.powerComp.PowerOn);
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.powerComp = base.GetComp<CompPowerTrader>();
            this.orbitalRelayComp = base.GetComp<Comp_OrbitalRelay>();

            if(respawningAfterLoad == false)
            {
                this.dishRotation = this.Rotation.AsAngle;
                UpdateLandingPadAvailability();
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            StopRotationSound();
            base.Destroy(mode);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.landingPadIsAvailable, "landingPadIsAvailable");

            Scribe_Values.Look<int>(ref this.ticksToNextRotation, "ticksToNextRotation");
            Scribe_Values.Look<float>(ref this.dishRotation, "dishRotation");
            Scribe_Values.Look<bool>(ref this.clockwiseRotation, "clockwiseRotation");
            Scribe_Values.Look<int>(ref this.ticksToRotationEnd, "ticksToRotationEnd");
        }

        public override void Tick()
        {
            base.Tick();

            if (!powerComp.PowerOn)
            {
                StopRotationSound();
            }
            else
            {
                UpdateDishRotation();
            }
        }

        public void UpdateLandingPadAvailability()
        {
            this.landingPadIsAvailable = Util_LandingPad.GetAllFreeAndPoweredLandingPads(this.Map) != null;
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());

            if (!this.powerComp.PowerOn)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append("Orbital Link: OFFLINE");
                return stringBuilder.ToString();
            }
            else
            {
                stringBuilder.AppendLine();
                stringBuilder.Append("Orbital Link: ONLINE");
                return stringBuilder.ToString();
            }
        }

        public void StartRotationSound()
        {
            StopRotationSound();
            this.rotationSoundSustainer = orbitalRelayComp.Props.dishSustainer.TrySpawnSustainer(new TargetInfo(this.Position, this.Map));
        }

        public void StopRotationSound()
        {
            if(this.rotationSoundSustainer != null)
            {
                this.rotationSoundSustainer.End();
                this.rotationSoundSustainer = null;
            }
        }

        public void UpdateDishRotation()
        {
            if (this.ticksToNextRotation > 0)
            {
                this.ticksToNextRotation--;
                if (this.ticksToNextRotation == 0)
                {
                    this.ticksToRotationEnd = Rand.RangeInclusive(rotationDurationMin, rotationDurationMax);
                    if (Rand.Value < 0.5f)
                    {
                        this.clockwiseRotation = true;
                    }
                    else
                    {
                        this.clockwiseRotation = false;
                    }
                    StartRotationSound();
                }
            }
            else
            {
                if (this.clockwiseRotation)
                {
                    this.dishRotation += dishRotationPerTick;
                }
                else
                {
                    this.dishRotation -= dishRotationPerTick;
                }
                this.ticksToRotationEnd--;
                if (this.ticksToRotationEnd == 0)
                {
                    this.ticksToNextRotation = Rand.RangeInclusive(rotationIntervalMin, rotationIntervalMax);
                    StopRotationSound();
                }
            }
        }

        public override void Draw()
        {
            base.Draw();

            dishMatrix.SetTRS(this.DrawPos + Altitudes.AltIncVect + new Vector3(0f, 3f, 0f), this.dishRotation.ToQuat(), orbitalRelayComp.Props.dishSize);
        }
    }
}
