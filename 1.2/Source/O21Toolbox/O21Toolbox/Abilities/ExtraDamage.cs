using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Abilities
{
    public class ExtraDamage : IExposable
    {
        public float chance;
        public int damage;
        public DamageDef damageDef;

        public void ExposeData()
        {
            Scribe_Values.Look(ref damage, "damage", -1);
            Scribe_Defs.Look(ref damageDef, "damageDef");
            Scribe_Values.Look(ref chance, "chance", -1f);
        }
    }
}
