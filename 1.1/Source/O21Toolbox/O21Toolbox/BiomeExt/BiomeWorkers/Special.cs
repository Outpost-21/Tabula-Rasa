using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace O21Toolbox.BiomeExt.BiomeWorkers
{
    public abstract class Special : BiomeWorker
    {
        protected float genChance = 0f;

        public virtual WLTileGraphicData GetWLTileGraphicData(WorldGrid grid, int tileID)
        {
            return null;
		}

		protected void DigTilesForBiomeChange(int startTileID, int digLengthMin, int digLengthMax, int maxDirChange, bool digBothDirections = true)
		{
			WorldGrid worldGrid = Find.WorldGrid;
			bool flag = false;
			int num = startTileID;
			int dir = Rand.RangeInclusive(0, 5);
			for (int i = 0; i < digLengthMax; i++)
			{
				int dir2 = GenWorldGen.NextRandomDigDir(dir, maxDirChange);
				num = worldGrid.GetTileNeighborByDirection6WayInt(num, dir2);
				Tile tile = worldGrid[num];
				if ((i >= digLengthMin || !this.MinPreRequirements(tile)) && !this.PreRequirements(tile))
				{
					if (flag)
					{
						break;
					}
					num = startTileID;
					dir = GenWorldGen.InvertDigDir(dir);
					i = -1;
					flag = true;
				}
				else
				{
					if (tile.biome.WorkerSpecial() == null)
					{
						bool end = i == digLengthMax - 1;
						this.ChangeTileAfterSuccessfulDig(tile, end);
					}
					if (digBothDirections && i == digLengthMax - 1 && !flag)
					{
						num = startTileID;
						dir = GenWorldGen.InvertDigDir(dir);
						i = -1;
						flag = true;
					}
				}
			}
		}

		protected virtual void ChangeTileAfterSuccessfulDig(Tile tile, bool end)
		{
		}

		public bool TryGenerateByChance()
		{
			bool result;
			if (Rand.Value < this.genChance)
			{
				if (this.genChance == this.InitialGenChance)
				{
					this.genChance -= this.GenChanceOffsetAfterFirstHit;
				}
				this.genChance = this.genChance * this.GenChancePerHitFactor - this.GenChancePerHitOffset;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void ResetChance()
		{
			this.genChance = this.InitialGenChance;
		}

		public virtual void PostGeneration(int tileID)
		{
		}

		public virtual bool PreRequirements(Tile tile)
		{
			return false;
		}

		public virtual bool MinPreRequirements(Tile tile)
		{
			return !tile.WaterCovered;
		}

		public override float GetScore(Tile tile, int tileID)
		{
			return -100f;
		}

		protected virtual float GenChancePerHitOffset
		{
			get
			{
				return 0f;
			}
		}

		protected virtual float GenChancePerHitFactor
		{
			get
			{
				return 0f;
			}
		}

		protected virtual float GenChanceOffsetAfterFirstHit
		{
			get
			{
				return 0f;
			}
		}

		protected virtual float InitialGenChance
		{
			get
			{
				return 0f;
			}
		}
	}
}
