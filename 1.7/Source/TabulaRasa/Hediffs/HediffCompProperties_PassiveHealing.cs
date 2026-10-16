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
    public class HediffCompProperties_PassiveHealing : HediffCompProperties
    {
        public HediffCompProperties_PassiveHealing()
        {
            compClass = typeof(HediffComp_PassiveHealing);
        }

        public int healTicks = 60;
        public bool healWounds = false;
        public bool tendWounds = false;
        public float tendQuality = 1f;
        public bool healWoundsSeq = true;
        public float healWoundsVal = 0.2f;
        public List<HediffDef> woundBlacklist = new List<HediffDef>();

        public int sickTicks = 60;
        public bool healSickness = false;
        public bool healSicknessSeq = true;
        public float healSicknessVal = 0.2f;
        public bool preventSicknesses = false;
        public List<HediffDef> sicknessWhitelist = new List<HediffDef>();
        public List<HediffDef> sicknessBlacklist = new List<HediffDef>();

        public int regrowTicks = 180;
        public bool regrowParts = false;
        public bool regrowPartsSeq = true;
        public HediffDef regrowingPartDef;
    }
}
