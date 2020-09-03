using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Drones;

namespace O21Toolbox
{
    public class CompProperties_PawnSpawner : CompProperties
    {
        public CompProperties_PawnSpawner()
        {
            this.compClass = typeof(Comp_PawnSpawner);
        }
        
        public PawnKindDef pawnKind;

        public List<PawnKindDef> pawnKinds = new List<PawnKindDef>();

        public bool newborn = false;

        public int timer = -1;

        public List<SkillLevelSetting> skillSettings = new List<SkillLevelSetting>();
    }
}
