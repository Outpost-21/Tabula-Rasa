using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Needs
{
    [StaticConstructorOnStartup]
    static public class PawnExt
    {
        static readonly public FleshTypeDef artificialFlesh;

        static PawnExt()
        {
            artificialFlesh = DefDatabase<FleshTypeDef>.GetNamed("Artificial");
        }

        static public bool IsArtificial(this Pawn pawn)
        {
            return pawn.RaceProps.FleshType == artificialFlesh;
        }

        static public bool IsNotArtificial(this Pawn pawn)
        {
            return pawn.RaceProps.FleshType != artificialFlesh;
        }
    }
}
