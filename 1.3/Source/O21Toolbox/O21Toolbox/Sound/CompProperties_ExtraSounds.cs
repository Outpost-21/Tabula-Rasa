using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Sound
{
    public class CompProperties_ExtraSounds : CompProperties
    {
        public CompProperties_ExtraSounds()
        {
            compClass = typeof(Comp_ExtraSounds);
        }

        public SoundDef soundExtra;
        public SoundDef soundExtraTwo;
        public SoundDef soundHitBuilding;
        public SoundDef soundHitPawn;
        public SoundDef soundMiss;
    }
}
