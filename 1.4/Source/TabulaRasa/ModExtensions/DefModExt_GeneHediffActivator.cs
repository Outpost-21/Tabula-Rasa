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
    public class DefModExt_GeneHediffActivator : DefModExtension
    {
        public string labelKey;
        public string descKey;
        public string iconTex;

        public HediffDef hediff;

        // Restrict based on skill levels
        public List<SkillLevelSetting> reqSkillLevels = new List<SkillLevelSetting>();

        // Restrict based on body type
        public List<BodyTypeDef> reqBodyTypes = new List<BodyTypeDef>();

        // Restrict based on hediff defs
        public List<HediffDef> reqHediffs = new List<HediffDef>();

        // Restrict based on traits
        public List<TraitDef> reqTraits = new List<TraitDef>();

        public int cooldown = 60000;
    }
}
