using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using RimWorld.Planet;
using System.Collections;

namespace O21Toolbox.BiomeExt
{
    public class WorldLayer_UniversalBiomeWorker : WorldLayer
    {
        private static readonly IntVec2 TexturesInAtlas = new IntVec2(2, 2);

        public override IEnumerable Regenerate()
		{
			foreach (object obj in base.Regenerate())
			{
				yield return obj;
			}
			Rand.PushState();
			Rand.Seed = Find.World.info.Seed;
			WorldGrid worldGrid = Find.WorldGrid;
			IEnumerable tweakedDefs = from x in DefDatabase<BiomeDef>.AllDefsListForReading
									  where x.HasModExtension<DefModExt_BiomeWorker>()
									  select x;
			foreach (BiomeDef biomeDef in tweakedDefs)
			{
				int num;
				for (int tileID = 0; tileID < Find.WorldGrid.TilesCount; tileID = num + 1)
				{
					Tile t = Find.WorldGrid[tileID];
					DefModExt_BiomeWorker defExt = biomeDef.GetModExtension<DefModExt_BiomeWorker>();
					if (t.biome == biomeDef)
					{
						if (!(defExt.materialPath == "World/MapGraphics/Default"))
						{
							Material material = MaterialPool.MatFrom(defExt.materialPath, ShaderDatabase.WorldOverlayTransparentLit, defExt.materialLayer);
							LayerSubMesh subMesh = base.GetSubMesh(material);
							Vector3 vector = worldGrid.GetTileCenter(tileID);
							WorldRendererUtility.PrintQuadTangentialToPlanet(vector, vector, worldGrid.averageTileSize, 0.01f, subMesh, false, true, false);
							WorldRendererUtility.PrintTextureAtlasUVs(Rand.Range(0, TexturesInAtlas.x), Rand.Range(0, TexturesInAtlas.z), TexturesInAtlas.x, TexturesInAtlas.z, subMesh);
							t = null;
							defExt = null;
							material = null;
							subMesh = null;
							vector = default(Vector3);
						}
					}
					num = tileID;
				}
			}
			Rand.PopState();
			base.FinalizeMesh(MeshParts.All);
			yield break;
		}
    }
}
