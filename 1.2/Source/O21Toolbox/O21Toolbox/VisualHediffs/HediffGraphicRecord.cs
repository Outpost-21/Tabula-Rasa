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
    public struct HediffGraphicRecord
    {
        public Graphic graphic;

        public Hediff sourceHediff;

        public HediffGraphicRecord(Graphic graphic, Hediff sourceHediff)
        {
            this.graphic = graphic;
            this.sourceHediff = sourceHediff;
        }
    }
}
