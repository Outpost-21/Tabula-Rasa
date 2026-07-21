using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa.Shuttle
{
    public class ShuttleUpgrade
    {
        // Name displayed for the slot in-game.
        public string slotLabel;

        // Allows a custom image to be used for the slot background.
        public string slotIconPath;

        // Groups this slot is allowed to have installed.
        public List<UpgradeGroupDef> upgradeGroups;

        // If defined, the slot will instead of being empty, have this pre-defined upgrade in it.
        // Default upgrades should not be defined in upgradeGroups.
        public UpgradeDef defaultUpgrade;

        // Position in the upgrade window for the slot to be displayed in.
        public Vector2 slotPosition = Vector2.zero;

        // Offsets only used for turrets.
        public Vector3 offset = Vector3.zero;

        public Vector3? offsetNorth;

        public Vector3? offsetSouth;

        public Vector3? offsetEast;

        public Vector3? offsetWest;
    }
}
