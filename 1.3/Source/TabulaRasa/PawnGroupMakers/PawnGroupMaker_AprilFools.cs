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
    public class PawnGroupMaker_AprilFools : PawnGroupMaker
    {
        public bool CanGenerate(PawnGroupMakerParms parms)
        {
            if (TabulaRasaMod.settings.specialOccasions && DateTime.Today.Month == 4 && DateTime.Today.Day == 1)
            {
                return true;
            }
            return false;
        }
    }
}
