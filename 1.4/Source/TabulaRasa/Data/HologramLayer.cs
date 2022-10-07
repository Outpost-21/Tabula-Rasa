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
    public class HologramLayer
    {
        public string texPath;

        public bool canChangeColor = true;

        public Color? defaultColor;

        public Material Hologram => MaterialPool.MatFrom(texPath, ShaderDatabase.TransparentPostLight);
    }
}
