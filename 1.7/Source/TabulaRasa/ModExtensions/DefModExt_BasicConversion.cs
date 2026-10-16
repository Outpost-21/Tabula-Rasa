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
    public class DefModExt_BasicConversion : DefModExtension
    {
        public XenotypeDef xenotype;
        public ThingDef structure;
        public bool structureOnMapChangesFaction = false;

        // TODO: Remove.
        public bool forceDropEquipment;
        public bool killPawn;
        public PawnKindDef defaultPawnKind;
    }
}
