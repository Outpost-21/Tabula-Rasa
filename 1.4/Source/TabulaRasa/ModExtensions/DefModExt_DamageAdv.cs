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
    public class DefModExt_DamageAdv : DefModExtension
    {
        public FleshTypeDef fleshTypeToAffect;
        public HediffDef hediff;
        public float hediffSev = 0f;
        public float increaseSevPerShot = 0f;
        public bool stunPawn;
        public bool wholeBody;
        public BodyPartDef bodyPart;
    }
}
