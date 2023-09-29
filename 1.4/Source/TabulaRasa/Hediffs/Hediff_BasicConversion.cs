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
                DefModExt_BasicConversion modExt = def.GetModExtension<DefModExt_BasicConversion>();
                if(modExt.xenotype != null)
                {
                    DoConversion(modExt);
                }
                else if (modExt.defaultPawnKind != null)
                {
                    DoBasicConvert(modExt);
                }
            }
        }

        public void DoConversion(DefModExt_BasicConversion modExt)
        {
            List<Gene> list3 = pawn.genes.Endogenes;
            for (int num = list3.Count - 1; num >= 0; num--)
            {
                Gene gene2 = list3[num];
                if (gene2.def.endogeneCategory != EndogeneCategory.Melanin && gene2.def.endogeneCategory != EndogeneCategory.HairColor)
                {
                    pawn.genes.RemoveGene(gene2);
                }
            }
            pawn.genes.SetXenotype(modExt.xenotype);
            pawn.health.RemoveHediff(this);


            if(modExt.structure != null)
            {
                List<Building> structures = pawn?.Map?.listerBuildings?.AllBuildingsColonistOfDef(modExt.structure)?.ToList();
                if (!structures.NullOrEmpty())
                {
                    Building structure = structures.First();
                    if (structure != null && modExt.structureOnMapChangesFaction)
                    {
                        pawn.SetFaction(structure.Faction);
                    }
                }
            }
        }

        public void DoBasicConvert(DefModExt_BasicConversion modExt)
        {
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
