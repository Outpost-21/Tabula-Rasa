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
    public class FacilityRecipeListing
    {
        public ThingDef facility;

        public List<RecipeDef> recipes;

        public QualityCategory minQuality = QualityCategory.Awful;
    }
}
