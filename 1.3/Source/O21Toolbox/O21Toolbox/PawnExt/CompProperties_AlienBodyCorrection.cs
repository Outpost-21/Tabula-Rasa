using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnExt
{
    public class CompProperties_AlienBodyCorrection : CompProperties
    {
        public CompProperties_AlienBodyCorrection()
        {
            this.compClass = typeof(Comp_AlienBodyCorrection);
        }

        public List<BodyTypeDef> maleBodyTypes = new List<BodyTypeDef>();

        public List<BodyTypeDef> femaleBodyTypes = new List<BodyTypeDef>();
    }
}
