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
    public class DefModExt_SpecialButchering : DefModExtension
    {
        public List<ThingDefCount> products = new List<ThingDefCount>();

        public bool affectedBySize = false;

        public bool affectedByDamage = false;
    }
}
