using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

using O21Toolbox.ApparelExt;

namespace O21Toolbox
{
    [HarmonyPatch(typeof(ApparelGraphicRecordGetter), "TryGetGraphicApparel")]
    public static class Patch_ApparelGraphicRecordGetter_TryGetGraphicApparel
    {
        public static string NextRaceDefName = null;

        [HarmonyPostfix]
        public static void Postfix(Apparel apparel, BodyTypeDef bodyType, ref ApparelGraphicRecord rec, bool __result)
        {
            if (NextRaceDefName == null)
            {
                return;
            }

            if (!__result)
            {
                return;
            }

            var extension = apparel.def.GetModExtension<DefModExt_RaceSpecificApparel>();

            var data = extension?.TryGetRaceData(NextRaceDefName);
            if (data == null)
            {
                return;
            }

            string styleName = apparel.StyleDef?.defName;
            styleName = styleName?.Substring(0, styleName.IndexOf('_'));
            bool doStyles = styleName != null && !string.IsNullOrEmpty(apparel.StyleDef.wornGraphicPath) && data.autoStyle && data.customPath != null && data.AllowStyle(styleName);

            string path;
            if (apparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead || apparel.def.apparel.LastLayer == ApparelLayerDefOf.EyeCover || PawnRenderer.RenderAsPack(apparel) || apparel.WornGraphicPath == BaseContent.PlaceholderImagePath || apparel.WornGraphicPath == BaseContent.PlaceholderGearImagePath)
            {
                path = data.customPath ?? apparel.WornGraphicPath + $"_{NextRaceDefName}";
                if (doStyles)
                {
                    path += $"_{styleName}";
                }
            }
            else
            {
                if (data.customPath == null)
                {
                    path = apparel.WornGraphicPath + $"_{NextRaceDefName}" + $"_{bodyType.defName}";
                }
                else
                {
                    path = data.customPath + $"_{bodyType.defName}";
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
                graphic = GraphicDatabase.Get<Graphic_Multi>(path, shader, data.customSize ?? apparel.def.graphicData.drawSize, data.customColor ?? apparel.DrawColor);
            }
            catch
            {
                graphic = null;
            }

            if (graphic == null || graphic.MatSingle == null)
            {
                Log.Warning($":: O21 Toolbox :: Looked for '{path}' but failed to find that texture for race '{NextRaceDefName}'. Apparel is {apparel.def.defName} ({apparel.def.fileName}).");
                return;
            }

            rec = new ApparelGraphicRecord(graphic, apparel);
        }
    }
}
