using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnConverter
{
    public class DefModExt_Hediff_Converter : DefModExtension
    {
        /// <summary>
        /// Recipe to use for conversion. 
        /// </summary>
        public PawnConvertingDef conversionRecipe = null;
    }
}
