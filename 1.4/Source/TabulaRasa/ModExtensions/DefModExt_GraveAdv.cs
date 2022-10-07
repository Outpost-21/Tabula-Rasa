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
    public class DefModExt_GraveAdv : DefModExtension
    {
        public int capacity = 1;

        public bool dissolveCorpses = false;

        public int dissolveTicks = 60000;
    }
}
