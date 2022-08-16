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
    public class CompProperties_HologramProjection : CompProperties_Glower
    {
        public Vector2 size = new Vector2(1f, 1f);
        public Vector3 offset = new Vector3(0f, 0f, 0f);
        public List<string> hologramTags = new List<string>();
        public string holobeam;

        public float radius = 0;
        public float recreationPerDay;
        public float certaintyPerDay;

        public CompProperties_HologramProjection()
        {
            compClass = typeof(Comp_HologramProjection);
        }

        public Material Holobeam => MaterialPool.MatFrom(holobeam, ShaderDatabase.MoteGlow);
    }
}
