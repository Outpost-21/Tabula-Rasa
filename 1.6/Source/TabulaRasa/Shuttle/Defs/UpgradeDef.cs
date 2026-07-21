using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TabulaRasa.Shuttle
{
    public class UpgradeDef : Def
    {
        // Icon shown in upgrade UI.
        public string menuIcon;

        // Overlay applied to shuttle, respects offsets and is relative to the position of the shuttle.
        public GraphicData overlayGraphic;

        // Defines which slots the upgrade can be used in.
        public UpgradeGroupDef upgradeGroup;

        // Material cost of installing the upgrade.
        public List<ThingDefCountClass> costList = new List<ThingDefCountClass>();

        // Time in ticks it takes to install the upgrade. Pawns crafting and intelligence are factored.
        public int workToInstall = 0;

        // Whether an upgrade is directly replaced, in which case it will only show in a dropdown for direct upgrades over
        // this one instead of the list shown when adding a new upgrade to an empty slot.
        public UpgradeDef replacesUpgrade;

        // Any upgrades sharing the same tag will not be able to be installed together, whichever one goes in first will
        // disable the others.
        public List<string> exclusionTags = new List<string>();

        // List of which other upgrades must be installed before this one can be, does not replace them, and is not needed for
        // upgrades which do replace another (replacesUpgrade field overrides this one entirely).
        public List<UpgradeDef> requiredUpgrades = new List<UpgradeDef>();

        // Research required before you can install this upgrade.
        public List<ResearchProjectDef> requiredResearch = new List<ResearchProjectDef>();

        // Memes your Ideology if you have the DLC active must have to be able to install this upgrade.
        public List<MemeDef> requiredMemes = new List<MemeDef>();

        // Hediffs required on a pawn for them to be capable of installing the upgrade (e.g. Mechinator).
        // This will check for ANY to match, not ALL.
        public List<HediffSeverityPairing> requiredHediff = new List<HediffSeverityPairing>();

        // Hediffs required on a pawn inside the shuttle to operate its functionality.
        // This will check for ANY to match, not ALL.
        public List<HediffSeverityPairing> requiredOperatorHediff = new List<HediffSeverityPairing>();

        // Genes required on a pawn for them to be capable of installing the upgrade.
        // This will check for ANY to match, not ALL.
        public List<GeneDef> requiredGene = new List<GeneDef>();

        // Genes required on a pawn inside the shuttle to operate its functionality.
        // This will check for ANY to match, not ALL.
        public List<GeneDef> requiredOperatorGene = new List<GeneDef>();

        // Def for the turret this adds if it does so, if left blank it will not add a turret.
        // The the position of the slot on the shuttle itself will define its offsets, the scale on the gun.
        public ThingDef turretGunDef;

        // Allows an upgrade to provide additional upgrade slots.
        public List<ShuttleUpgrade> shuttleUpgradeSlots = new List<ShuttleUpgrade>();

        // Allows upgrades to add turret slots, useful for allowing an upgrade choice between different
        // shuttle loadouts. E.g. One upgrade allowing two small turrets, while another allows one large turret.
        public List<ShuttleUpgrade> shuttleTurretSlots = new List<ShuttleUpgrade>();

        // Additional stats specific to shuttles. These are offsets to increase or decrease things usually, comments will state if otherwise.
        public int passengerCapacity = 0;

        public int cargoCapacity = 0;
    }
}
