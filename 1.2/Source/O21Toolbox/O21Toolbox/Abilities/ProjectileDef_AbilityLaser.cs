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
    public class ProjectileDef_AbilityLaser : ProjectileDef_Ability
    {
        public bool CanStartFire = false;
        public int postFiringDuration = 0;
        public float postFiringFinalIntensity = 0f;
        public float postFiringInitialIntensity = 0f;
        public int preFiringDuration = 0;
        public float preFiringFinalIntensity = 0f;
        public float preFiringInitialIntensity = 0f;
        public float StartFireChance;
        public string warmupGraphicPathSingle = null;
    }
}
