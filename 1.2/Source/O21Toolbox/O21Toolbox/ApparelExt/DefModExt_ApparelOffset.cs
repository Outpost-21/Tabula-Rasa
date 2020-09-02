using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ApparelExt
{
    public class DefModExt_ApparelOffset : DefModExtension
    {
        /// <summary>
        /// Path of the textures, same as if it was in the normal apparelProperties.
        /// </summary>
        [NoTranslate]
        public string wornGraphicPath = string.Empty;

        /// <summary>
        /// Decides if the code looks for textures related to body type or not.
        /// </summary>
        public bool bodyDependant = false;

        /// <summary>
        /// Offsets the layer this is rendered at, 0.025 will result in it being between the pawn and their weapon for example.
        /// </summary>
        public float zOffset = 0.0f;
    }
}
