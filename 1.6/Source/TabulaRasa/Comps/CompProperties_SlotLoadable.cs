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
    public class CompProperties_SlotLoadable : CompProperties
    {
        public CompProperties_SlotLoadable()
        {
            compClass = typeof(Comp_SlotLoadable);
        }

        public bool gizmosOnEquip = true;

        public List<SlotLoadableDef> slots = new List<SlotLoadableDef>();
    }
}
