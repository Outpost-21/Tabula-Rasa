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
    public class DefModExt_RecipeExtender : DefModExtension
    {
        public HediffDef requiredHediff;
        public string requiredHediffAnyPawnMsg = "TR_RequiredHediffAnyPawnMsg";
        public string requiredHediffMissingMsg = "TR_RequiredHediffMissingMsg";
    }
}
