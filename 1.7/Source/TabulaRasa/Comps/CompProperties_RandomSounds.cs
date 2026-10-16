using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public class CompProperties_RandomSounds : CompProperties
    {
        public CompProperties_RandomSounds()
        {
            compClass = typeof(Comp_RandomSounds);
        }

        public List<SoundDef> soundDefs = new List<SoundDef>();

        public IntRange tickRange = new IntRange(1000, 10000);
    }
}
