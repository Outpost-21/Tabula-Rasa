using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

using O21Toolbox.Utility;

namespace O21Toolbox.FactionExt
{
    public class PawnGroupMaker_FactionPoints : PawnGroupMaker
    {
        public int minPoints = 0;

        public int maxPoints = int.MaxValue;

        public bool CanGenerate(PawnGroupMakerParms parms)
        {
            int num = FactionUtils.FactionPoints();
            return num > this.minPoints && num < this.maxPoints;
        }
    }
}
