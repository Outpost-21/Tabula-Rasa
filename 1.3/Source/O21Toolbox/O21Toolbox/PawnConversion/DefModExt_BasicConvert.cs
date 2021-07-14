using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.PawnConversion
{
    public class DefModExt_BasicConvert : DefModExtension
    {
        public PawnKindDef defaultPawnKind;

        public bool forceDropEquipment;
        public bool killPawn;
    }
}
