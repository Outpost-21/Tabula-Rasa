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
    public class Comp_HologramProjection : CompGlower
    {
        public new CompProperties_HologramProjection props => (CompProperties_HologramProjection)Props;

        public HologramDef holoDef = null;

        public List<HologramDef> viableHolos = new List<HologramDef>();

        public Dictionary<int, Color> hologramColors = new Dictionary<int, Color>();

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref holoDef, "holoDef");
            Scribe_Collections.Look(ref hologramColors, "hologramColors");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (viableHolos.NullOrEmpty())
            {
                viableHolos = DefDatabase<HologramDef>.AllDefs.Where(hd => hd.hologramTags.Any(hdt => props.hologramTags.Contains(hdt))).ToList();
            }
            if (holoDef == null)
            {
                if (!viableHolos.NullOrEmpty())
                {
                    holoDef = viableHolos.First();
                    ResetHoloColors();
                }
                else
                {
                    LogUtil.LogError($"{parent.def.defName} has no viable hologram defs! Make sure the tags match!");
                }
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (!props.holobeam.NullOrEmpty())
            {
                DrawHoloBeam();
            }

            for (int i = 0; i < holoDef.hologramLayers.Count; i++)
            {
                DrawHoloLayer(holoDef.hologramLayers[i], i);
            }
        }

        public void DrawHoloBeam()
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor(ShaderPropertyIDs.Color, hologramColors[0]);
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(parent.DrawPos, Quaternion.AngleAxis(0, Vector3.up), new Vector3(parent.Graphic.drawSize.x, 1, parent.Graphic.drawSize.y));
            Graphics.DrawMesh(MeshPool.plane10, matrix, props.Holobeam, 0, null, 0, propertyBlock);
        }

        public void DrawHoloLayer(HologramLayer layer, int layerInt)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor(ShaderPropertyIDs.Color, hologramColors[layerInt]);
            Matrix4x4 layerMatrix = default(Matrix4x4);
            layerMatrix.SetTRS(parent.DrawPos + props.offset, Quaternion.AngleAxis(0, Vector3.up), new Vector3(props.size.x, 1, props.size.y));
            Graphics.DrawMesh(MeshPool.plane10, layerMatrix, layer.Hologram, 0, null, 0, propertyBlock);
        }

        public void ResetHoloColors()
        {
            hologramColors.Clear();
            int curLayer = 0;
            foreach(HologramLayer layer in holoDef.hologramLayers)
            {
                hologramColors.Add(curLayer, layer.defaultColor ?? Color.white);
                curLayer++;
            }
            UpdateGlower();
        }

        public void SetHoloColor(int layer, Color color)
        {
            hologramColors[layer] = color;
            if(layer == 0)
            {
                UpdateGlower();
            }
        }

        public void UpdateGlower()
        {
            if (!hologramColors.NullOrEmpty())
            {
                //parent.SetColor(hologramColors[0]);
                props.glowColor = ColorIntUtility.AsColorInt(hologramColors[0]);
                parent.Map.mapDrawer.MapMeshDirty(parent.Position, (ulong)MapMeshFlagDefOf.Things);
            }
        }
    }
}
