using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;
using RimWorld.Planet;

namespace O21Toolbox.BiomeExt
{
    public class BiomeWorker_Universal : BiomeWorker
    {
        public override float GetScore(Tile tile, int tileID)
        {
            return 0f;
        }
    }
}
