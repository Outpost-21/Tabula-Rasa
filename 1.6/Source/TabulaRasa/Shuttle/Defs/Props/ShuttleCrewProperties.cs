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
    public class ShuttleCrewProperties
    {
        public int pilotCapacity = 0;

        public int requiredPilots = 0;

        public int basePassengerCapacity = 0;

        public List<ShuttleGunnerProperties> gunnerSlots = new List<ShuttleGunnerProperties>();

        public bool canEjectPassengers = false;

        public bool passengersCanShoot = false;
    }
}
