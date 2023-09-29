﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    //TODO: Remove next RW update
    public class CompProperties_EnergySource : CompProperties
    {
        public CompProperties_EnergySource()
        {
            compClass = typeof(Comp_EnergySource);
        }

        public float energyGiven = 0f;

        public ThingDef outputThing;
    }
}
