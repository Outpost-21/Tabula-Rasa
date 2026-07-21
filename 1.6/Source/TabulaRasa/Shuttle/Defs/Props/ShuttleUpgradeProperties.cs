using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace TabulaRasa.Shuttle
{
    public class ShuttleUpgradeProperties
    {
        // Properties of upgrade slots.
        public List<ShuttleUpgrade> upgradeSlots = new List<ShuttleUpgrade>();
        // Properties of built in turret slots.
        public List<ShuttleUpgrade> turretSlots = new List<ShuttleUpgrade>();
    }
}
