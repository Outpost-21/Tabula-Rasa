using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.BiomeExt
{
    public class Biome_GenStep_BetterCaves : DefModExtension
	{
		public float startTunnelsPer10k = 2f;

		// Token: 0x04000174 RID: 372
		public int maxStartTunnelsPerRockGroup = 6;

		// Token: 0x04000175 RID: 373
		public int minRocksToGenerateAnyTunnel = 600;

		// Token: 0x04000176 RID: 374
		public float tunnelStartWidthFactorMin = 0.6f;

		// Token: 0x04000177 RID: 375
		public float tunnelStartWidthFactorMax = 0.9f;

		// Token: 0x04000178 RID: 376
		public float directionChangeSpeed = 8f;

		// Token: 0x04000179 RID: 377
		public int allowBranchingAfterSteps = 30;

		// Token: 0x0400017A RID: 378
		public int minDistToOutsideForBranching = 15;

		// Token: 0x0400017B RID: 379
		public float branchChance = 0.015f;

		// Token: 0x0400017C RID: 380
		public float branchChanceTypeRoom = 0.15f;

		// Token: 0x0400017D RID: 381
		public float branchChanceTypeTunnel = 0.25f;

		// Token: 0x0400017E RID: 382
		public float minTunnelWidth = 3.4f;

		// Token: 0x0400017F RID: 383
		public float widthOffsetPerCellMin = 0.019f;

		// Token: 0x04000180 RID: 384
		public float widthOffsetPerCellMax = 0.044f;

		// Token: 0x04000181 RID: 385
		public float widthOffsetPerCellTunnelFactor = 0.2f;

		// Token: 0x04000182 RID: 386
		public float branchWidthFactorMin = 0.6f;

		// Token: 0x04000183 RID: 387
		public float branchWidthFactorMax = 0.9f;

		// Token: 0x04000184 RID: 388
		public float branchRoomFixedWidthMin = 20f;

		// Token: 0x04000185 RID: 389
		public float branchRoomFixedWidthMax = 30f;

		// Token: 0x04000186 RID: 390
		public float branchTunnelFixedWidthMin = 4.5f;

		// Token: 0x04000187 RID: 391
		public float branchTunnelFixedWidthMax = 7f;

		// Token: 0x04000188 RID: 392
		public int branchRoomMaxLength = 25;

		// Token: 0x04000189 RID: 393
		public float terrainPatchMakerFrequencyCaveWater = 0.08f;

		// Token: 0x0400018A RID: 394
		public List<TerrainThresholdWEO> terrainPatchMakerCaveWater = null;

		// Token: 0x0400018B RID: 395
		public float terrainPatchMakerFrequencyCaveGravel = 0.16f;

		// Token: 0x0400018C RID: 396
		public List<TerrainThresholdWEO> terrainPatchMakerCaveGravel = null;
	}
}
