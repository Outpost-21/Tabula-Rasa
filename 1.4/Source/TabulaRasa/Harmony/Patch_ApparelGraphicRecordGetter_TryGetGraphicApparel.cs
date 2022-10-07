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
    [HarmonyPatch(typeof(ApparelGraphicRecordGetter), "TryGetGraphicApparel")]
    public static class Patch_ApparelGraphicRecordGetter_TryGetGraphicApparel
    {
        public static string nextRaceDefName = null;

        [HarmonyPostfix]
        public static void Postfix(Apparel apparel, BodyTypeDef bodyType, ref ApparelGraphicRecord rec, bool __result)
        {
            if (nextRaceDefName == null)
            {
                return;
            }

            if (!__result)
            {
                return;
            }

            var extension = apparel.def.GetModExtension<DefModExt_RaceApparel>();

            var data = extension?.TryGetRaceApparelData(nextRaceDefName);
            if (data == null)
            {
                return;
            }

            string styleName = apparel.StyleDef?.defName;
            styleName = styleName?.Substring(0, styleName.IndexOf('_'));
            bool doStyles = styleName != null && !string.IsNullOrEmpty(apparel.StyleDef.wornGraphicPath) && data.autoStyle && data.path != null && data.AllowedStyle(styleName);

            string path;
            if (apparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead || apparel.def.apparel.LastLayer == ApparelLayerDefOf.EyeCover || PawnRenderer.RenderAsPack(apparel) || apparel.WornGraphicPath == BaseContent.PlaceholderImagePath || apparel.WornGraphicPath == BaseContent.PlaceholderGearImagePath)
            {
                path = data.path ?? apparel.WornGraphicPath + $"_{nextRaceDefName}";
                if (doStyles)
                {
                    path += $"_{styleName}";
                }
            }
            else
            {
                if (data.path == null)
                {
                    path = apparel.WornGraphicPath + $"_{nextRaceDefName}" + $"_{bodyType.defName}";
                }
                else
                {
                    path = data.path + $"_{bodyType.defName}";
                }

                if (doStyles)
                {
                    path += $"_{styleName}";
                }
            }
            Shader shader = ShaderDatabase.Cutout;
            if (apparel.def.apparel.useWornGraphicMask)
            {
                shader = ShaderDatabase.CutoutComplex;
            }

            Graphic graphic;
            try
            {
                graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, data.size ?? apparel.def.graphicData.drawSize, data.color ?? apparel.DrawColor);
            }
            catch
            {
                graphic = null;
            }

            if (graphic == null || graphic.MatSingle == null)
            {
                LogUtil.LogWarning($"Could not find custom apparel textures at '{path}' for race '{nextRaceDefName}'. Apparel is {apparel.def.defName} ({apparel.def.fileName}).");
                return;
            }

            rec = new ApparelGraphicRecord(graphic, apparel);
        }
    }
}
