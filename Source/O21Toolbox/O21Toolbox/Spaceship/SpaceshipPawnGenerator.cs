using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Spaceship
{
    public static class SpaceshipPawnGenerator
    {
        public static Pawn GeneratePawn(PawnKindDef pawnKindDef, Map map)
        {
            Pawn pawn = null;

            PawnGenerationRequest request = new PawnGenerationRequest(
                kind: pawnKindDef,
                context: PawnGenerationContext.NonPlayer,
                forceGenerateNewPawn: true,
                mustBeCapableOfViolence: true);
            pawn = PawnGenerator.GeneratePawn(request);
            return pawn;
        }
    }
}
