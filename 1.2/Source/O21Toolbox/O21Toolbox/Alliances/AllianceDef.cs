using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Alliances
{
    /// <summary>
    /// Defines Alliances.
    /// </summary>
    public class AllianceDef : Def
    {
        /// <summary>
        /// Members of the defined alliance.
        /// </summary>
        public List<string> memberFactions = new List<string>();
        
        /// <summary>
        /// Defined relations to start against factions.
        /// </summary>
        public List<RelationFaction> factionRelations = new List<RelationFaction>();

        /// <summary>
        /// Defined relations to start against the player based on baseMember pawnkind.
        /// </summary>
        public List<RelationPlayer> playerRelations = new List<RelationPlayer>();

        /// <summary>
        /// Defined relations to start against other alliances.
        /// </summary>
        public List<RelationAlliance> allianceRelations = new List<RelationAlliance>();
    }
}