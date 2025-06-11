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
    public class HologramDef : Def
    {
        public List<string> hologramTags = new List<string>();

        public List<HologramLayer> hologramLayers = new List<HologramLayer>();
    }
}
