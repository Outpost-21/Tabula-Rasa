using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace TabulaRasa.Shuttle
{
    public class ShuttleDrawProperties
    {
        public string texPath;

        public ShaderTypeDef shaderType = ShaderTypeDefOf.Cutout;

        public Color color = Color.white;

        public bool useStuffColor = false;

        public Vector2 scale = new Vector2(1, 1);

        public float altitude;

        public int layer;

        public bool includeInShadow = true;

        public ShuttleState modesVisible = ShuttleState.All;
    }
}
