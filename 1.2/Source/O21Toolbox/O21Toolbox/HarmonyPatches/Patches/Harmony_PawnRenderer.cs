using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;

using O21Toolbox.ApparelExt;
using O21Toolbox.VisualHediffs;

namespace O21Toolbox.HarmonyPatches
{
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal")]
    [HarmonyPatch(new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool), typeof(bool) })]
    public static class PawnRenderer_RenderPawnInternal
    {
        [HarmonyPostfix]
        public static void Postfix(PawnRenderer __instance, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
        {
            if (!__instance?.graphics?.pawn?.RaceProps?.Animal ?? false)
            {
                List<ApparelGraphicRecord> offsetApparelList = new List<ApparelGraphicRecord>();
                // Get all apparel with the defModExt.
                foreach (Apparel ap in __instance?.graphics?.pawn?.apparel?.WornApparel)
                {
                    ApparelGraphicRecord item;
                    if (ap.def.HasModExtension<DefModExt_ApparelOffset>())
                    {
                        DefModExt_ApparelOffset modExt = ap.def.GetModExtension<DefModExt_ApparelOffset>();
                        if (Harmony_Apparel.TryGetGraphicApparelSpecial(ap, __instance.graphics.pawn.story.bodyType, modExt, out item))
                        {
                            offsetApparelList.Add(item);
                        }
                    }
                }

                // Render if any Apparel in the list
                if (offsetApparelList?.Count >= 1)
                {
                    Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
                    Mesh mesh = MeshPool.humanlikeBodySet.MeshAt(bodyFacing);
                    Vector3 vector2 = rootLoc;
                    vector2.y += ((bodyFacing == Rot4.South) ? 0.006122449f : 0.02755102f);

                    for (int i = 0; i < offsetApparelList.Count; i++)
                    {
                        ApparelGraphicRecord apparelGraphicRecord = offsetApparelList[i];
                        Pawn pawn = __instance.graphics.pawn;
                        DefModExt_ApparelOffset modExt = apparelGraphicRecord.sourceApparel.def.GetModExtension<DefModExt_ApparelOffset>();

                        Material material = apparelGraphicRecord.graphic.MatAt(bodyFacing, null);
                        material = OverrideMaterialIfNeeded(__instance, material, pawn, portrait);
                        if (apparelGraphicRecord.sourceApparel.def.apparel.wornGraphicData != null)
                        {
                            Vector2 vector3 = apparelGraphicRecord.sourceApparel.def.apparel.wornGraphicData.BeltOffsetAt(bodyFacing, pawn.story.bodyType);
                            Vector2 vector4 = apparelGraphicRecord.sourceApparel.def.apparel.wornGraphicData.BeltScaleAt(pawn.story.bodyType);
                            Matrix4x4 matrix = Matrix4x4.Translate(vector2) * Matrix4x4.Rotate(quaternion) * Matrix4x4.Translate(new Vector3(vector3.x, modExt.zOffset, vector3.y)) * Matrix4x4.Scale(new Vector3(vector4.x, 1f, vector4.y));
                            GenDraw.DrawMeshNowOrLater_NewTemp(mesh, matrix, material, portrait);
                        }
                    }
                }
            }

            RenderVisualHediffs(bodyFacing, renderBody, headFacing, __instance.pawn, bodyDrawType);
        }

        public static Material OverrideMaterialIfNeeded(PawnRenderer __instance, Material original, Pawn pawn, bool portrait = false)
        {
            Material baseMat = (!portrait && pawn.IsInvisible()) ? InvisibilityMatPool.GetInvisibleMat(original) : original;
            return __instance.graphics.flasher.GetDamagedMat(baseMat);
		}

        public static void RenderVisualHediffs(Rot4 bodyFacing, bool renderBody, Rot4 headFacing, Pawn pawn, RotDrawMode drawMode)
        {
            IEnumerable<Hediff> visualHediffs = from hediff in pawn.health.hediffSet.hediffs
                                                where hediff.def.HasModExtension<DefModExt_VisualHediff>()
                                                select hediff;


            foreach(Hediff hediff in visualHediffs)
            {
                DefModExt_VisualHediff modExt = hediff.def.GetModExtension<DefModExt_VisualHediff>();

                HediffGraphicRecord record;
                TryGetVisualHediffRecord(pawn, hediff, hediff.part.Label.Contains("left"), modExt, out record);
            }
        }

        public static bool TryGetVisualHediffRecord(Pawn pawn, Hediff hediff, bool flip, DefModExt_VisualHediff modExt, out HediffGraphicRecord result)
        {
            if (modExt.texPath.NullOrEmpty())
            {
                result = new HediffGraphicRecord(null, null);
                return false;
            }
            string path = modExt.texPath;
            if(modExt.centerOnPart == CenterOnPart.Body)
            {
                path += "_" + pawn.story.bodyType.defName;
            }
            Vector2 drawSize = new Vector2(1.0f, 1.0f);
            if(pawn.def.defName != "Human" && pawn.def.HasModExtension<DefModExt_VisualHediffCompat>())
            {
                DefModExt_VisualHediffCompat raceExt = pawn.def.GetModExtension<DefModExt_VisualHediffCompat>();

                if (!raceExt.pathSuffix.NullOrEmpty())
                {
                    path += "_" + raceExt.pathSuffix;
                }
                drawSize = raceExt.drawSize;
            }
            Graphic graphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderDatabase.Cutout, drawSize, Color.white);
            result = new HediffGraphicRecord(graphic, hediff);
            return true;
        }
	}
}
