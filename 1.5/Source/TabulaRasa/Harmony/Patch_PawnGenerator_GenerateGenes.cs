using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace TabulaRasa
{
    [HarmonyPatch(typeof(PawnGenerator), "GenerateGenes")]
    public static class Patch_PawnGen_GenerateGenes
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, XenotypeDef xenotype, PawnGenerationRequest request)
        {
            DefModExt_PawnKindExtended modExt = pawn.kindDef.GetModExtension<DefModExt_PawnKindExtended>();

            if (modExt != null && !pawn.Dead)
            {
                if (!modExt.geneGroups.NullOrEmpty() && Rand.Chance(modExt.geneGroupChance))
                {
                    AddGenesFromGroup(GetGeneGroup(pawn.kindDef), pawn);
                }
            }
        }

        public static void AddGenesFromGroup(GeneGroup entry, Pawn pawn)
        {
            for (int i = 0; i < entry.genes.Count; i++)
            {
                GeneDef curGene = entry.genes[i];
                if(entry.heritable && !pawn.genes.HasEndogene(curGene))
                {
                    pawn.genes.AddGene(curGene, false);
                }
                else if (!pawn.genes.HasGene(curGene))
                {
                    pawn.genes.AddGene(curGene, true);
                }
            }
        }

        public static GeneGroup GetGeneGroup(PawnKindDef pawnkind)
        {
            if (pawnkind.HasModExtension<DefModExt_PawnKindExtended>())
            {
                DefModExt_PawnKindExtended modExt = pawnkind.GetModExtension<DefModExt_PawnKindExtended>();
                GeneGroup result;
                if (!modExt.geneGroups.NullOrEmpty())
                {
                    Func<GeneGroup, float> selector = (GeneGroup x) => x.commonality;
                    result = modExt.geneGroups.RandomElementByWeight(selector);

                    return result;
                }
            }

            return null;
        }
    }
}
