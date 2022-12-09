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
    [HarmonyPatch(typeof(HeadTypeDef), "GetGraphic")]
    public static class Patch_HeadTypeDef_GetGraphic
    {
        [HarmonyPostfix]
        public static void Postfix(HeadTypeDef __instance, Color color, ref Graphic_Multi __result, bool dessicated, bool skinColorOverriden)
        {
            DefModExt_HeadTypeStuff modExt = __instance.GetModExtension<DefModExt_HeadTypeStuff>();
            if (modExt != null)
            {
                Shader shader = ((!dessicated) ? ShaderUtility.GetSkinShader(skinColorOverriden) : ShaderDatabase.Cutout);
                if (!modExt.useSkinShader)
                {
                    shader = modExt.shaderType?.Shader ?? ShaderDatabase.Cutout;
                }
                for (int i = 0; i < __instance.graphics.Count; i++)
                {
                    if (color.IndistinguishableFrom(__instance.graphics[i].Key) && __instance.graphics[i].Value.Shader == shader)
                    {
                        __result = __instance.graphics[i].Value;
                    }
                }
                Graphic_Multi graphic_Multi = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>(__instance.graphicPath, shader, Vector2.one, color);
                __instance.graphics.Add(new KeyValuePair<Color, Graphic_Multi>(color, graphic_Multi));
                __result = graphic_Multi;
            }
        }
    }
}
