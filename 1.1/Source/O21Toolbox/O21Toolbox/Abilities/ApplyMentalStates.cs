using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Abilities
{
    public class ApplyMentalStates : IExposable
    {
        public float applyChance = 1.0f;
        public MentalStateDef mentalStateDef;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref mentalStateDef, "mentalStateDef");
            Scribe_Values.Look(ref applyChance, "applyChance", 1.0f);
        }
    }
}
