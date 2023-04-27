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
    public class MapComp_VisualOverlays : MapComponent
    {
        public List<Hediff_VisualOverlay> hediffOverlays = new List<Hediff_VisualOverlay>();

        public MapComp_VisualOverlays(Map map) : base(map)
        {
        }

        public override void MapComponentUpdate()
        {
            base.MapComponentUpdate();
            for (int i = 0; i < hediffOverlays.Count; i++)
            {
                Hediff_VisualOverlay hediff = hediffOverlays[i];
                if(hediff == null || hediff.pawn == null || !hediff.pawn.health.hediffSet.hediffs.Contains(hediff))
                {
                    hediffOverlays.RemoveAt(i);
                }
                else if (hediff.pawn?.MapHeld != null)
                {
                    hediff.Draw();
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref hediffOverlays, "hediffOverlays", LookMode.Reference);
        }
    }
}
