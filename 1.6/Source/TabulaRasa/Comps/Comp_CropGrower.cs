using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa.Comps
{
    public class Comp_CropGrower : ThingComp
    {
        CompProperties_CropGrower Props => (CompProperties_CropGrower)props;

        public int currTick;

        public int CurrTick { get { return currTick; } set { currTick = value; } }

        public int PlantGrowTicks => Mathf.RoundToInt(CurrPlant?.plant?.growDays ?? 0);

        public float PercentageGrown => CurrTick / PlantGrowTicks;

        public ThingDef currPlant;

        public ThingDef CurrPlant => currPlant;

        public override void CompTick()
        {
            base.CompTick();
            if (CurrPlant != null && Find.TickManager.TicksAbs >= CurrTick)
            {
                FinalizeGrowth();
            }
        }

        public void RestartGrowth()
        {
            if (CurrPlant == null) { return; }
            CurrTick = 0;
        }

        public void FinalizeGrowth()
        {
            if (CurrPlant == null) { return; }
            if (CurrTick < PlantGrowTicks) { CurrTick++; }
            if (CurrTick > PlantGrowTicks) { CurrTick = PlantGrowTicks; }
            if (PercentageGrown >= 1f && Props.exportWhenComplete)
            {
                ProduceOutputForPlant(CurrPlant);
                RestartGrowth();
            }
        }

        public void ProduceOutputForPlant(ThingDef plant)
        {
            Thing t = ThingMaker.MakeThing(plant.plant.harvestedThingDef);
            t.stackCount = Mathf.RoundToInt(plant.plant.harvestYield);
            GenPlace.TryPlaceDirect(t, parent.InteractionCell, t.Rotation, parent.Map, out t);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref currPlant, "currPlant");
            Scribe_Values.Look(ref currTick, "finalTick");
        }

        public override void PostDraw()
        {
            base.PostDraw();
            // Render Plant
            // Render any overlays provided
        }
    }
}
