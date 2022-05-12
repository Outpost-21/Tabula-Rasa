using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace O21Toolbox
{
    public class CompProperties_RandomSounds : CompProperties
    {
        public CompProperties_RandomSounds()
        {
            compClass = typeof(Comp_RandomSounds);
        }

        /// <summary>
        /// A random element from this list is pulled to play a single sound when the comp ticks.
        /// </summary>
        public List<SoundDef> soundDefs = new List<SoundDef>();

        /// <summary>
        /// Int range used to define when the next sound will occur.
        /// </summary>
        public IntRange tickRange = new IntRange(1000, 10000);
    }
}
