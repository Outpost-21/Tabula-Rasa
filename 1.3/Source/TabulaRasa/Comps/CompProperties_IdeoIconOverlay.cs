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
    public class CompProperties_IdeoIconOverlay : CompProperties
    {
        public CompProperties_IdeoIconOverlay()
        {
            compClass = typeof(Comp_IdeoIconOverlay);
        }

        public bool showSouth = true;
        public bool showNorth = true;
        public bool showEast = true;
        public bool showWest = true;

        public Vector3 offsetSouth = new Vector3(0, 0, 0);
        public Vector3 offsetNorth = new Vector3(0, 0, 0);
        public Vector3 offsetEast = new Vector3(0, 0, 0);
        public Vector3 offsetWest = new Vector3(0, 0, 0);

        public Vector2 drawSize = new Vector2(1, 1);
    }
}
