using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Needs
{
    /// <summary>
    /// Basically tags a ThingDef as a mechanical pawn.
    /// </summary>
    public class ArtificialPawnProperties : DefModExtension
    {
        /// <summary>
        /// If true the pawn will not lose any skill due to decay.
        /// </summary>
        public bool noSkillLoss = true;

        /// <summary>
        /// Can this Droid be social?
        /// </summary>
        public bool canSocialize = false;

        /// <summary>
        /// Does the colony care if they die?
        /// </summary>
        public bool colonyCaresIfDead = false;

        /// <summary>
        /// Def for applicable repair parts (medicine)
        /// </summary>
        public List<ThingDef> repairParts = null;

        /// <summary>
        /// Prevents corpse rotting.
        /// </summary>
        public bool tweakCorpseRot = true;
    }

    [StaticConstructorOnStartup]
    public static class PostInitializationTweaker
    {
        static PostInitializationTweaker()
        {
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                ArtificialPawnProperties tweaker = thingDef.GetModExtension<ArtificialPawnProperties>();
                if (tweaker != null)
                {
                    ThingDef corpseDef = thingDef?.race?.corpseDef;
                    if (corpseDef != null)
                    {
                        if (tweaker.tweakCorpseRot)
                        {
                            corpseDef.comps.RemoveAll(compProperties => compProperties is CompProperties_Rottable);
                            corpseDef.comps.RemoveAll(compProperties => compProperties is CompProperties_SpawnerFilth);
                        }
                    }
                }
            }
        }
    }
}
