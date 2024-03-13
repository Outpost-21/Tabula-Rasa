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
    public class PawnGroupMaker_PrideMonth : PawnGroupMaker
    {

        public bool CanGenerate(PawnGroupMakerParms parms)
        {
            if (TabulaRasaMod.settings.specialOccasions && DateTime.Now.Month == 6)
            {
                return true;
            }
            return false;
        }
    }
}
