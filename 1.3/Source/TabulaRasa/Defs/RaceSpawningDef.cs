using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static RimWorld.QuestGen.QuestNode_GetRandomPawnKindForFaction;

namespace TabulaRasa
{
    public class RaceSpawningDef : Def
    {
        /// <summary>
        /// Provided races for these particular settings. If a race is added to a pawnKind twice with 
        /// different weights using this method, then only the first is kept. This maintains any manual
        /// additions using xml patches.
        /// </summary>
        public List<ThingDef> races = new List<ThingDef>();

        /// <summary>
        /// List of factions to automatically add the races to. The code cycles all pawnKinds which
        /// have any of these factions set as their defaultFactionType to find which pawnKinds it should
        /// apply the extension to.
        /// </summary>
        public List<FactionDef> factions = new List<FactionDef>();

        /// <summary>
        /// Weight of the race against others when randomly chosen. Defaults to 100, same as humans.
        /// Recommended weight is 20 so humans remain most common but not massively so, based on most
        /// sci-fi and fantasy lore having that be a thing.
        /// </summary>
        public int weight = 100;
    }
}
