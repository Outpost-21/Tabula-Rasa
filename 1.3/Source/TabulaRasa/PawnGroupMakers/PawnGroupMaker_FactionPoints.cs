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
    public class PawnGroupMaker_FactionPoints : PawnGroupMaker
    {
        public int minPoints = 0;

        public int maxPoints = int.MaxValue;

        public bool CanGenerate(PawnGroupMakerParms parms)
        {
            int num = FactionUtil.FactionPoints();
            return num > this.minPoints && num < this.maxPoints;
        }
    }
}
