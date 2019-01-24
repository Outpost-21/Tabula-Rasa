using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace O21Toolbox.Spaceship
{
    public class AirStrikeDef : Def
    {
        public const int maxWeapons = 3;

        public int runsNumber = 1;
        public int costInSilver = 500;
        public float ammoResupplyDays = 1;

        public float cellsTravelledPerTick = 0.25f;
        
        // Overflight means the ship is exactly over the target.
        public int ticksBeforeOverflightInitialValue = 10 * GenTicks.TicksPerRealSecond; // Ship will appear this time before overflight.
        public int ticksBeforeOverflightPlaySound = 4 * GenTicks.TicksPerRealSecond;     // Flight sound is played at this time before overflight.
        public int ticksBeforeOverflightReducedSpeed = 4 * GenTicks.TicksPerRealSecond;  // Ship is flying at slow speed during this time before overflight.
        public int ticksAfterOverflightReducedSpeed = 0;                                 // Ship is flying at slow speed during this time after overflight.
        public int ticksAfterOverflightFinalValue = 10 * GenTicks.TicksPerRealSecond;    // Ship will disappear this time after overflight.

        public List<WeaponDef> weapons = new List<WeaponDef>();
    }
}
