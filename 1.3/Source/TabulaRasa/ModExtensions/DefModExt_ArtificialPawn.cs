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
    /// <summary>
    /// Pawns with this exension are automatically exempt from:
    /// - Being Inspired
    /// - Vomiting
    /// </summary>
    public class DefModExt_ArtificialPawn : DefModExtension
    {
        // All default to typical organic pawn settings
        public bool noSkillLoss = false; // Done
        public float skillLossMulti = 1f; // Done
        public bool noSkillGain = false; // Done
        public float skillGainMulti = 1f; // Done
        public bool canSocialize = true; // Done
        public bool deathMatters = true; // Done
        public bool corpseEdible = true; // Done
        public bool corpseRots = true; // Done
        public bool affectedByEMP = false; // Done
        // List of items able to be used in place of medicine
        public bool canBeRepaired = true;
        public List<ThingDef> repairParts = new List<ThingDef>();
    }
}
