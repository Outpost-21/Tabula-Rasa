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
    public class ProjectileDef_Ability : ThingDef
    {
        public int HealCapacity = 3;
        public float HealFailChance = 0.3f;
        public bool IsBeamProjectile = false;
    }
}
