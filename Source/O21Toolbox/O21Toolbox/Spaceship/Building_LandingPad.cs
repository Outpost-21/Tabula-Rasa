using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Spaceship
{
    public class Building_LandingPad : Building
    {
        /// <summary>
        /// Is this pad the primary landing pad?
        /// </summary>
        public bool isPrimary = true;

        /// <summary>
        /// Is this pad already reserved?
        /// </summary>
        public bool isReserved = false;

        /// <summary>
        /// Next tick to check for a blockage.
        /// </summary>
        public int nextBlockCheckTick = 0;

        /// <summary>
        /// Reasons the pad is blocked.
        /// </summary>
        public string blockingReasons = "";

        /// <summary>
        /// If the pad is not reserved and not blocked, then it is free.
        /// </summary>
        public bool isFree => this.isReserved == false && this.blockingReasons.Length == 0;

        /// <summary>
        /// If the pad is free and also powered.
        /// </summary>
        public bool isFreeAndPowered => this.isFree && ((this.powerComp != null && this.powerComp.PowerOn) || this.powerComp == null);

        /// <summary>
        /// Power Component.
        /// </summary>
        public CompPowerTrader powerComp = null;

        public Comp_LandingPad landingPadComp = null;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.powerComp = base.GetComp<CompPowerTrader>();
            this.powerComp.powerStartedAction = Notify_PowerStarted;
            this.powerComp.powerStoppedAction = Notify_PowerStopped;

            // Check for pre-existing Primary landing pad.
            if(this.Map.listerBuildings.AllBuildingsColonistOfClass<Building_LandingPad>() != null)
            {
                foreach(Building building in this.Map.listerBuildings.AllBuildingsColonistOfClass<Building_LandingPad>())
                {
                    Building_LandingPad landingPad = building as Building_LandingPad;
                    if(landingPad != this && landingPad.isPrimary)
                    {
                        this.isPrimary = false;
                        break;
                    }
                }
            }

            //Only spawn addons once.
            if (!respawningAfterLoad)
            {
                SpawnAddons();
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            // Destroy all addons.
            DestroyAddons();

            base.Destroy(mode);
            Util_OrbitalRelay.TryUpdateLandingPadAvailability(this.Map);
        }

        public void SpawnAddons()
        {
            // Spawn Beacons

            // Spawn Lights

            // Spawn Projectors
        }

        public void DestroyAddons()
        {
            // Despawn Beacons
            foreach(IntVec3 cell in this.OccupiedRect().Cells)
            {
                Thing thing = cell.GetFirstThing<Building_LandingPadBeacon>(this.Map);
                if(thing != null)
                {
                    thing.Destroy();
                }
            }

            // Despawn Lights
            foreach (IntVec3 cell in this.OccupiedRect().Cells)
            {
                Thing thing = cell.GetFirstThing<Building_LandingPadLight>(this.Map);
                if (thing != null)
                {
                    thing.Destroy();
                }
            }

            // Despawn Projectors
            foreach (IntVec3 cell in this.OccupiedRect().Cells)
            {
                Thing thing = cell.GetFirstThing<Building_LandingPadProjector>(this.Map);
                if (thing != null)
                {
                    thing.Destroy();
                }
            }
        }

        /// <summary>
        /// Set selected landing pad as primary.
        /// </summary>
        public void SetAsPrimary()
        {
            foreach (Building landingPad in this.Map.listerBuildings.AllBuildingsColonistOfClass<Building_LandingPad>())
            {
                (landingPad as Building_LandingPad).isPrimary = false;
            }
            this.isPrimary = true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.nextBlockCheckTick, "nextBlockCheckTick");
            Scribe_Values.Look<bool>(ref this.isPrimary, "isPrimary");
            Scribe_Values.Look<bool>(ref this.isReserved, "isReserved");
            Scribe_Values.Look<string>(ref this.blockingReasons, "blockingReasons");
        }

        public override void Tick()
        {
            base.Tick();

            // Update Blocking Reasons
            if(Current.Game.tickManager.TicksGame >= this.nextBlockCheckTick)
            {
                this.nextBlockCheckTick = Current.Game.tickManager.TicksGame + GenTicks.TicksPerRealSecond;
                CheckForBlockingThing();
            }
        }

        public void CheckForBlockingThing()
        {
            bool isUnroofed = true;

            this.blockingReasons = "";
            StringBuilder reasonsList = new StringBuilder();
            foreach(IntVec3 cell in this.OccupiedRect().Cells)
            {
                if (isUnroofed && cell.Roofed(this.Map))
                {
                    isUnroofed = false;
                    reasonsList.AppendWithComma("roof");
                }

                Building building = cell.GetEdifice(this.Map);
                if (building != null && building.def.thingClass != typeof(Building_SpaceshipTurret))
                {
                    reasonsList.AppendWithComma(building.Label);
                }

                Plant plant = cell.GetPlant(this.Map);
                if (plant !=null && plant.def.plant.IsTree)
                {
                    reasonsList.AppendWithComma(plant.Label);
                }
            }
            if(reasonsList.Length > 0)
            {
                this.blockingReasons = reasonsList.ToString();
            }
        }

        public void Notify_PowerStarted()
        {
            Util_OrbitalRelay.TryUpdateLandingPadAvailability(this.Map);
            // Light the beacons
            /** foreach(Building building in this.Map.listerBuildings.AllBuildingsColonistOfClass<Building_LandingPad>())
            {
                Building_LandingPadBeacon beacon = building as Building_LandingPadBeacon;
                if (beacon.landingPad == this)
                {
                    beacon.Notify_PowerStarted();
                }
            }
            // Light the lights
            foreach (Building building in this.Map.listerBuildings.AllBuildingsColonistOfClass<Building_LandingPad>())
            {
                Building_LandingPadLight beacon = building as Building_LandingPadLight;
                if (beacon.landingPad == this)
                {
                    beacon.Notify_PowerStarted();
                }
            }
            // Light the projectors
            foreach (Building building in this.Map.listerBuildings.AllBuildingsColonistOfClass<Building_LandingPad>())
            {
                Building_LandingPadProjector beacon = building as Building_LandingPadProjector;
                if (beacon.landingPad == this)
                {
                    beacon.Notify_PowerStarted();
                }
            } **/
        }

        public void Notify_PowerStopped()
        {
            Util_OrbitalRelay.TryUpdateLandingPadAvailability(this.Map);
            // Beacons
            /** foreach (Building building in this.Map.listerBuildings.AllBuildingsColonistOfClass<Building_LandingPad>())
            {
                Building_LandingPadBeacon beacon = building as Building_LandingPadBeacon;
                if (beacon.landingPad == this)
                {
                    beacon.Notify_PowerStopped();
                }
            }
            // Lights
            foreach (Building building in this.Map.listerBuildings.AllBuildingsColonistOfClass<Building_LandingPad>())
            {
                Building_LandingPadLight beacon = building as Building_LandingPadLight;
                if (beacon.landingPad == this)
                {
                    beacon.Notify_PowerStopped();
                }
            }
            // Projectors
            foreach (Building building in this.Map.listerBuildings.AllBuildingsColonistOfClass<Building_LandingPad>())
            {
                Building_LandingPadProjector beacon = building as Building_LandingPadProjector;
                if (beacon.landingPad == this)
                {
                    beacon.Notify_PowerStopped();
                }
            } **/
        }

        public void Notify_ShipLanding()
        {
            this.isReserved = true;
            Util_OrbitalRelay.TryUpdateLandingPadAvailability(this.Map);
        }

        public void Notify_ShipTakingOff()
        {
            this.isReserved = false;
            Util_OrbitalRelay.TryUpdateLandingPadAvailability(this.Map);
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetInspectString());

            if (this.isPrimary)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append("Primary landing pad");
            }
            if (this.blockingReasons.Length > 0)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append("Blocked by " + this.blockingReasons);
            }
            return stringBuilder.ToString();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            IList<Gizmo> buttonList = new List<Gizmo>();
            int groupKeyBase = 700000105;

            Command_Action setTargetButton = new Command_Action();
            if (this.isPrimary)
            {
                setTargetButton.icon = ContentFinder<Texture2D>.Get("UI/Toolbox/Commands_Primary");
                setTargetButton.defaultLabel = "Primary";
                setTargetButton.defaultDesc = "Vehicles will land there in priority if landing pad is clear. Otherwise, they will choose another.";
            }
            else
            {
                setTargetButton.icon = ContentFinder<Texture2D>.Get("UI/Toolbox/Commands_Ancillary");
                setTargetButton.defaultLabel = "Ancillary";
                setTargetButton.defaultDesc = "Vehicles may only land there if primary landing pad is busy. Click to set it as primary.";
            }
            setTargetButton.activateSound = SoundDef.Named("Click");
            setTargetButton.action = new Action(SetAsPrimary);
            setTargetButton.groupKey = groupKeyBase + 1;
            buttonList.Add(setTargetButton);

            IEnumerable<Gizmo> resultButtonList;
            IEnumerable<Gizmo> basebuttonList = base.GetGizmos();
            if (basebuttonList != null)
            {
                resultButtonList = basebuttonList.Concat(buttonList);
            }
            else
            {
                resultButtonList = buttonList;
            }
            return resultButtonList;
        }
    }
}
