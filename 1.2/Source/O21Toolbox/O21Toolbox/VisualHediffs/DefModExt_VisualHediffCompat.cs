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
    public class DefModExt_VisualHediffCompat : DefModExtension
    {
        public string pathSuffix = string.Empty;

        public Vector2 drawSize = new Vector2(1.0f, 1.0f);
    }
}
