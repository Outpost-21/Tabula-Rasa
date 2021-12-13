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
    /// <summary>
    /// Spawns a pawn and sets specific paramaters that usually aren't easily adjustable to compensate for the fact the pawn isn't naturally spawning like most.
    /// If this is applied to a plant then the spawning action will take place when the plant is fully mature, this is automatic and not changeable.
    /// </summary>
    public class CompProperties_PawnSpawner : CompProperties
    {
        public CompProperties_PawnSpawner()
        {
            this.compClass = typeof(Comp_PawnSpawner);
        }

        /// <summary>
        /// Should the thing wait before replacing itself with the pawn?
        /// </summary>
        public int timer = -1;

        /// <summary>
        /// If true the spawner will not delete itself, and will instead spawn until it reaches the repeatCount.
        /// </summary>
        public bool repeatSpawn = false;

        /// <summary>
        /// Limits the number of times repeatSpawn functions, to a number randomly selected between the min and max for optional variety.
        /// </summary>
        public IntRange repeatCount = new IntRange(0, 1);

        /// <summary>
        /// Deletes the spawner once it no longer has a purpose.
        /// </summary>
        public bool deleteWhenDone = true;

        /// <summary>
        /// Single set pawnkind to spawn. Overrides pawnKinds if both exist.
        /// </summary>
        public PawnKindDef pawnKind;

        /// <summary>
        /// List from which a random pawnKind is chosen.
        /// </summary>
        public List<PawnKindDef> pawnKinds = new List<PawnKindDef>();

        /// <summary>
        /// Sets skill all to 0 before setting the ones from settings.
        /// </summary>
        public bool purgeSkillsBeforeSetting = true;

        /// <summary>
        /// Sets skills ot the level desired.
        /// </summary>
        public List<SkillLevelSetting> skillSettings = new List<SkillLevelSetting>();

        /// <summary>
        /// Whether or not the pawn should be treated as a newborn.
        /// </summary>
        public bool newborn = false;

        /// <summary>
        /// Whether or not the pawn should spawn with any traits.
        /// </summary>
        public bool purgeTraits = false;

        /// <summary>
        /// Whether or not the pawn should spawn with any relations.
        /// </summary>
        public bool canGeneratePawnRelations = false;

        /// <summary>
        /// Removes any apparel that somehow made it onto the pawn. Example: Ideo preferred apparel.
        /// </summary>
        public bool purgeApparel = false;
    }
}
