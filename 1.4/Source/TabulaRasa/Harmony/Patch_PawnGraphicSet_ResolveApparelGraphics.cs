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
    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveAllGraphics")]
    public static class Patch_PawnGraphicSet_ResolveAllGraphics
    {
        [HarmonyPostfix]
        public static void Postfix(PawnGraphicSet __instance)
        {
            Pawn pawn = __instance.pawn;
            if (!ModsConfig.BiotechActive || pawn == null || pawn.RaceProps?.Humanlike != true || pawn?.genes == null)
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
                        __instance.furCoveredGraphic = (pawn.genes.GenesListForReading.Where((Gene x) => x.Active).Any((Gene g) => g.def.GetModExtension<DefModExt_FurDef>()?.useSkinColorForFur ?? false) ? GraphicDatabase.Get<Graphic_Multi>(pawn.story.furDef.GetFurBodyGraphicPath(pawn), ShaderDatabase.CutoutComplex, Vector2.one, pawn.story.SkinColor) : GraphicDatabase.Get<Graphic_Multi>(pawn.story.furDef.GetFurBodyGraphicPath(pawn), ShaderDatabase.CutoutSkinOverlay, Vector2.one, pawn.story.HairColor));
                    }
                    else if (modExtension.useSkinColorForFur)
                    {
                        __instance.furCoveredGraphic = GraphicDatabase.Get<Graphic_Multi>(pawn.story.furDef.GetFurBodyGraphicPath(pawn), ShaderUtility.GetSkinShader(pawn.story.SkinColorOverriden), Vector2.one, pawn.story.SkinColor);
                    }
                }
            }
        }
    }
}
