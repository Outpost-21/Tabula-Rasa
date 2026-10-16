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
    public class DefModExt_RegrowingPart : DefModExtension
    {
        public string labelText = "Regrowing: ";

        public float growthPerDay = 1f;

        public bool coverageMultiplier = true;
    }
}
