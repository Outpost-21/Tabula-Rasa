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
    public class DefModExt_PreventPlantSpawns : DefModExtension
    {
        public List<ThingDef> allowedPlants = new List<ThingDef>();
    }
}
