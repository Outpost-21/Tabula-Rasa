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
    public class CompProperties_ClusterGrower : CompProperties
    {
        public List<ClusterPlantClass> clusterPlants = new List<ClusterPlantClass>();

        public int growthTicks = 3000;

        public List<ThingDef> cannotGrowOver = new List<ThingDef>();

        public ThingDef undergrowth;

        public float undergrowthRadius = 5.5f;

        public int undergrowthTicks = 1000;

        public bool undergrowthClears = false;

        public SimpleCurve undergrowthCurve = new SimpleCurve()
        {
            new CurvePoint (0f, 1f),
            new CurvePoint (2.5f, 1f),
            new CurvePoint (6f, 0f)
        };

        public CompProperties_ClusterGrower()
        {
            compClass = typeof(Comp_ClusterGrower);
        }
    }

    public class ClusterPlantClass
    {
        public ThingDef def;
        public int count;
        public FloatRange radius = new FloatRange(0f, 4f);
        public bool matureOnly = false;
        public bool onUndergrowthOnly = false;
        public float minDistance = 0f;
        public float chance = 1f;
    }
}
