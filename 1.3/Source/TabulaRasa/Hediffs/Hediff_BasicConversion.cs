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
    public class Hediff_BasicConversion : HediffWithComps
    {
        public override void Tick()
        {
            base.Tick();

            if (Severity >= 1.0)
            {
                DoBasicConvert();
            }
        }

        private void DoBasicConvert()
        {
            DefModExt_BasicConversion modExt = def.GetModExtension<DefModExt_BasicConversion>();
            PawnGenerationRequest request = new PawnGenerationRequest(
                modExt.defaultPawnKind,
                faction: Faction.OfPlayer,
                forceGenerateNewPawn: true,
                canGeneratePawnRelations: false,
                colonistRelationChanceFactor: 0f,
                fixedBiologicalAge: pawn.ageTracker.AgeBiologicalYearsFloat,
                fixedChronologicalAge: pawn.ageTracker.AgeChronologicalYearsFloat,
                allowFood: false,
                allowAddictions: false);

            Pawn convertedPawn = PawnGenerator.GeneratePawn(request);
            GenPlace.TryPlaceThing(convertedPawn, pawn.Position, pawn.Map, ThingPlaceMode.Direct);
            if (modExt.forceDropEquipment)
            {
                if (pawn.inventory != null)
                {
                    pawn.inventory.DropAllNearPawn(pawn.Position);
                }
                if (pawn.apparel != null)
                {
                    pawn.apparel.DropAll(pawn.Position);
                }
                if (pawn.equipment != null)
                {
                    pawn.equipment.DropAllEquipment(pawn.Position);
                }
            }
            if (modExt.killPawn)
            {
                pawn.Kill(null, this);
            }
            pawn.Destroy();
        }
    }
}
