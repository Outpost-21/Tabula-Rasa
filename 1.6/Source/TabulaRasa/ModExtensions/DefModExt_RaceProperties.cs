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
    public class DefModExt_RaceProperties : DefModExtension
    {
        public bool infectionsEnabled = true;
        public bool diseasesEnabled = true;

        // Animals Only
        public bool trainingDecays = true;

        // Pawn Only
    }
}
