using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.VisualHediffs
{
    public class DefModExt_VisualHediff : DefModExtension
    {
        public string texPath = string.Empty;
        public ShaderTypeDef shader;
        public Graphic graphic = null;
        public bool flip = false;
        public CenterOnPart centerOnPart = CenterOnPart.Head;
    }

    public enum CenterOnPart
    {
        Body,
        Head
    }
}
