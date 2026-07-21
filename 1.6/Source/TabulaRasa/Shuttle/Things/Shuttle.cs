using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace TabulaRasa.Shuttle
{
    public class Shuttle : Building
    {
        #region General

        public ShuttleProperties props => (def as ShuttleDef).shuttleProperties;

        public CompRefuelable fuelComp;

        public bool fuelCompChecked = false;

        public CompRefuelable FuelComp
        {
            get
            {
                if (!fuelCompChecked)
                {
                    fuelComp = this.TryGetComp<CompRefuelable>();
                    fuelCompChecked = true;
                }
                return fuelComp;
            }
        }

        public override void Tick()
        {
            base.Tick();
        }

        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            SetupCrew();
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            base.DeSpawn(mode);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref crew, "crew");
        }

        #endregion

        #region Crew

        public ShuttleCrewTracker crew;

        public int PassengerCapacity => Mathf.Clamp(props.crew.basePassengerCapacity + UpgradePassengerCapacity, 0, 9999);

        public int UpgradePassengerCapacity
        {
            get
            {
                int total = 0;
                for (int i = 0; i < installedUpgrades.Count; i++)
                {
                    total += installedUpgrades[i].passengerCapacity;
                }
                return total;
            }
        }

        public int TotalCrewCapacity => props.crew.pilotCapacity + PassengerCapacity;

        public void SetupCrew()
        {
            crew = new ShuttleCrewTracker();
            crew.capacity = TotalCrewCapacity;
        }

        #endregion

        #region Motion

        public bool IsLanded => currState == ShuttleState.Landed;

        public ShuttleState currState;

        public float currAltitude;

        public float currDrawScale;

        public float currDrawAngle;

        public FlightMode currFlightMode;

        public FlightMode FlightMode
        {
            get
            {
                if (currFlightMode == FlightMode.None)
                {
                    if (props.motion.defaultFlightMode == FlightMode.None)
                    {
                        currFlightMode = FlightMode.Moving;
                    }
                    else
                    {
                        currFlightMode = props.motion.defaultFlightMode;
                    }
                }
                return currFlightMode;
            }
        }

        #endregion

        #region Upgrades

        public List<UpgradeDef> installedUpgrades = new List<UpgradeDef>();

        public UpgradeDef currInstall;

        public UpgradeDef CurrInstall
        {
            get
            {
                if (currInstall == null)
                {
                    if (!installsToDo.NullOrEmpty())
                    {
                        currInstall = installsToDo.First();
                    }
                }
                return currInstall;
            }
        }

        public List<UpgradeDef> installsToDo = new List<UpgradeDef>();

        public bool DueInstall => CurrInstall != null;

        public float installWorkRemaining = -1f;

        public UpgradeDef currUninstall;

        public UpgradeDef CurrUninstall
        {
            get
            {
                if (currUninstall == null)
                {
                    if (!uninstallsToDo.NullOrEmpty())
                    {
                        currUninstall = uninstallsToDo.First();
                    }
                }
                return currUninstall;
            }
        }

        public List<UpgradeDef> uninstallsToDo = new List<UpgradeDef>();

        public bool DueUninstall => CurrUninstall != null;

        public float uninstallWorkRemaining = -1f;

        public List<ThingDefCountClass> storedResources = new List<ThingDefCountClass>();

        public void DoInstallWork(float amount)
        {
            if(CurrInstall != null)
            {
                installWorkRemaining -= amount;
                if (installWorkRemaining <= 0f)
                {
                    CompleteInstall();
                }
            }
        }

        public void CompleteInstall()
        {
            UpgradeDef upgrade = CurrInstall;
            if (upgrade != null)
            {
                if (!installedUpgrades.Contains(upgrade))
                {
                    installedUpgrades.Add(upgrade);
                }
                installWorkRemaining = -1f;
                currInstall = null;
                if (installsToDo.Contains(upgrade))
                {
                    installsToDo.Remove(upgrade);
                }
                storedResources.Clear();
                SoundDefOf.Building_Complete.PlayOneShot(this);
            }
        }

        public void DoUninstallWork(float amount)
        {
            if (CurrUninstall != null)
            {
                uninstallWorkRemaining -= amount;
                if(uninstallWorkRemaining <= 0f)
                {
                    CompleteUninstall();
                }
            }
        }

        public void CompleteUninstall()
        {
            UpgradeDef upgrade = CurrUninstall;
            if (upgrade != null)
            {
                if (installedUpgrades.Contains(upgrade))
                {
                    installedUpgrades.Remove(upgrade);
                }
                RefundUpgrade();
                uninstallWorkRemaining = -1f;
                currUninstall = null;
                if (uninstallsToDo.Contains(upgrade))
                {
                    uninstallsToDo.Remove(upgrade);
                }
                SoundDefOf.Building_Deconstructed.PlayOneShot(this);
            }
        }

        public void RefundUpgrade()
        {
            if (CurrUninstall == null) { return; }
            storedResources.Clear();
            storedResources = CurrUninstall.costList;
            PurgeResources(false);
            storedResources.Clear();
        }

        public void StoreResources(ThingDef thingDef, int count)
        {
            if (thingDef != null && count > 0)
            {
                ThingDefCountClass existingResource = storedResources.FirstOrDefault(tdcc => tdcc.thingDef == thingDef);
                if (existingResource != null)
                {
                    existingResource.count += count;
                }
                else
                {
                    storedResources.Add(new ThingDefCountClass(thingDef, count));
                }
            }
        }

        public void PurgeResources(bool destroy = false)
        {
            if (!destroy)
            {
                foreach (ThingDefCountClass thing in storedResources)
                {
                    SpawnResource(thing.thingDef, thing.count);
                }
            }
            storedResources.Clear();
        }

        public void SpawnResource(ThingDef thingDef, int count)
        {
            if (Map != null && thingDef != null && count > 0)
            {
                IntVec3 position = (InteractionCell.IsValid ? InteractionCell : Position);
                while (count > 0)
                {
                    int currNum = Mathf.Min(count, thingDef.stackLimit);
                    Thing thing = ThingMaker.MakeThing(thingDef);
                    thing.stackCount = currNum;
                    GenPlace.TryPlaceThing(thing, position, Map, ThingPlaceMode.Near);
                    count -= currNum;
                }
            }
        }

        #endregion

        #region Behaviour

        public int lastSearchTick;

        public bool autoTakeOffToSearch = false;

        public bool autoLandNoEnemies = false;

        public bool? currSearchMode;

        public bool SearchMode
        {
            get
            {
                if (currSearchMode == null)
                {
                    currSearchMode = props.behaviour.searchForEnemiesDefault;
                }
                return (bool)currSearchMode;
            }
        }

        #endregion
    }
}
