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
    public class Apparel_Customizable : Apparel
    {
        public bool colorsCalculated;

        public Color drawColorFirst = Color.white;
        public Color drawColorSecond = Color.white;

        public DefModExt_ApparelCustomizable modExtCached;
        public DefModExt_ApparelCustomizable ModExt 
        {
            get
            {
                if (modExtCached == null)
                { 
                    modExtCached = def.GetModExtension<DefModExt_ApparelCustomizable>(); 
                }
                return modExtCached;
            }
        }

        public void InitColors(Color first, Color second)
        {
            drawColorFirst = first;
            drawColorSecond = second;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                InitColors(ModExt.defaultColorFirst, ModExt.defaultColorSecond);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref drawColorFirst, "drawColorFirst", Color.white);
            Scribe_Values.Look(ref drawColorSecond, "drawColorSecond", Color.white);
        }
    }
}
