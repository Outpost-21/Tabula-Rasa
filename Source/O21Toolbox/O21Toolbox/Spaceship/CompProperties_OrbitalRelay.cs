using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Spaceship
{
    public class CompProperties_OrbitalRelay : CompProperties
    {
        /// <summary>
        /// Path for dish texture path.
        /// </summary>
        public String dishTexturePath;

        /// <summary>
        /// Rendered dish size.
        /// </summary>
        public Vector3 dishSize;

        /// <summary>
        /// SoundDef for when the dish moves.
        /// </summary>
        public SoundDef dishSustainer;
    }
}
