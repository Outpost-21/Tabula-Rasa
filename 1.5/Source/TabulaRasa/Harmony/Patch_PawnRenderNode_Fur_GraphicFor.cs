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
    [HarmonyPatch(typeof(PawnRenderNode_Fur), "GraphicFor")]
    public static class Patch_PawnRenderNode_Fur_GraphicFor
    {
        [HarmonyPostfix]
        public static void Postfix(PawnRenderNode_Fur __instance, Graphic __result, Pawn pawn)
        {
            if (!ModLister.CheckBiotech("Fur"))
            {
                return;
            }
            if (pawn.story?.furDef == null)
            {
                return;
            }
            List<Gene> genesListForReading = pawn.genes.GenesListForReading;
            foreach (Gene gene in genesListForReading)
            {
                if (!gene.Active)
                {
                    continue;
                }
                DefModExt_FurDef modExtension = gene.def.GetModExtension<DefModExt_FurDef>();
                if (modExtension != null)
                {
                    if (modExtension.useMaskForFur)
                    {
                        if(pawn.genes.GenesListForReading.Where((Gene x) => x.Active).Any((Gene g) => g.def.GetModExtension<DefModExt_FurDef>()?.useSkinColorForFur ?? false))
                        {
                            __result = GraphicDatabase.Get<Graphic_Multi>(pawn.story?.furDef.GetFurBodyGraphicPath(pawn), ShaderDatabase.CutoutComplex, Vector2.one, pawn.story.SkinColor);
                        }
                        else
                        {
                            __result = GraphicDatabase.Get<Graphic_Multi>(pawn.story.furDef.GetFurBodyGraphicPath(pawn), ShaderDatabase.CutoutSkinOverlay, Vector2.one, pawn.story.HairColor);
                        }
                    }
                    else if (modExtension.useSkinColorForFur)
                    {
                        __result = GraphicDatabase.Get<Graphic_Multi>(pawn.story.furDef.GetFurBodyGraphicPath(pawn), __instance.ShaderFor(pawn), Vector2.one, pawn.story.SkinColor);
                    }
                }
            }
        }
    }
}
