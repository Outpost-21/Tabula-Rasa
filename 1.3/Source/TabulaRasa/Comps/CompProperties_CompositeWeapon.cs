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
    public class CompProperties_CompositeWeapon : CompProperties
    {
        public GraphicData graphicData;

        public CompProperties_CompositeWeapon()
        {
            compClass = typeof(Comp_CompositeWeapon);
        }
    }
}
