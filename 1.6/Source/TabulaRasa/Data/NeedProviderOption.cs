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
    public class NeedProviderOption
    {
        public NeedDef need;
        public bool usesPower = false;
        public bool usesNutrition = false;
        public float efficiency = 1.0f;
    }
}
