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
    public class DefModExt_ArtificialPawn : DefModExtension
    {
        // All default to typical organic pawn settings
        public bool noSkillLoss = false;
        public bool canSocialize = true;
        public bool deathMatters = true;
        public bool corpseEdible = true;
        public bool corpseRots = true;
        public bool affectedbyEMP = false;
        // List of items able to be used in place of medicine
        public List<ThingDef> repairParts = new List<ThingDef>();
    }
}
