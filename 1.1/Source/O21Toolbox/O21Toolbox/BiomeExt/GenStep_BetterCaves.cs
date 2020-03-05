using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BiomeExt
{
    public class GenStep_BetterCaves : GenStep
	{
		public override int SeedPart
		{
			get
			{
				return 235321433;
			}
		}
	}
	public override void Generate(Map map, GenStepParams parms)
	{
		bool flag = !Find.World.HasCaves(map.Tile);
		if (!flag)
		{
			ModExt_Biome_GenStep_BetterCaves modExtension = map.Biome.GetModExtension<ModExt_Biome_GenStep_BetterCaves>();
			bool flag2 = modExtension == null || !modExtension.GenerateOnTile(map.Tile);
			if (!flag2)
			{
				this.extCaves = modExtension;
				this.directionNoise = new Perlin(0.002050000010058284, 2.0, 0.5, 4, Rand.Int, QualityMode.Medium);
				this.tunnelWidthNoise = new Perlin(0.019999999552965164, 2.0, 0.5, 4, Rand.Int, QualityMode.Medium);
				this.tunnelWidthNoise = new ScaleBias(0.4, 1.0, this.tunnelWidthNoise);
				MapGenFloatGrid elevation = MapGenerator.Elevation;
				BoolGrid visited = new BoolGrid(map);
				List<IntVec3> rockCells = new List<IntVec3>();
				Predicate<IntVec3> <> 9__0;
				Action<IntVec3> <> 9__1;
				foreach (IntVec3 intVec in map.AllCells)
				{
					bool flag3 = !visited[intVec] && this.IsRock(intVec, elevation, map);
					if (flag3)
					{
						rockCells.Clear();
						FloodFiller floodFiller = map.floodFiller;
						IntVec3 root = intVec;
						Predicate<IntVec3> passCheck;
						if ((passCheck = <> 9__0) == null)
						{
							passCheck = (<> 9__0 = ((IntVec3 x) => this.IsRock(x, elevation, map)));
						}
						Action<IntVec3> processor;
						if ((processor = <> 9__1) == null)
						{
							processor = (<> 9__1 = delegate (IntVec3 x)
							{
								visited[x] = true;
								rockCells.Add(x);
							});
						}
						floodFiller.FloodFill(root, passCheck, processor, int.MaxValue, false, null);
						this.Trim(rockCells, map);
						this.RemoveSmallDisconnectedSubGroups(rockCells, map);
						bool flag4 = rockCells.Count >= modExtension.minRocksToGenerateAnyTunnel;
						if (flag4)
						{
							this.StartWithTunnel(rockCells, map);
						}
					}
				}
				this.SmoothGenerated(map);
			}
		}
	}
}
