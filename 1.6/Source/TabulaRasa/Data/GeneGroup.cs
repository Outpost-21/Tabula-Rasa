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
    public class GeneGroup
    {
        public float commonality = 100f;

        public bool heritable = false;

        public List<GeneDef> genes = new List<GeneDef>();
    }
}
