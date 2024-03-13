using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TabulaRasa.Hediffs
{
    public class HediffCompProperties_GeneticConversion : HediffCompProperties
    {
        public HediffCompProperties_GeneticConversion()
        {
            compClass = typeof(HediffComp_GeneticConversion);
        }

        /// <summary>
        /// Range in which a random time period is selected for the conversion to take place after the hediff is added.
        /// </summary>
        public IntRange tickRange = new IntRange(0, 0);

        /// <summary>
        /// Xenotype to use as a basis for conversion.
        /// </summary>
        public XenotypeDef xenotype;

        /// <summary>
        /// If true, exisitng genes will be cleared and only the added ones will remain.
        /// </summary>
        public bool overwriteGenes = false;

        /// <summary>
        /// If true, pawn will be converted to the target faction. If faction is null they'll be converted to the player.
        /// </summary>
        public bool convertPawn = false;

        /// <summary>
        /// Faction to convert the pawn to.
        /// </summary>
        public FactionDef faction = null;
    }
}
