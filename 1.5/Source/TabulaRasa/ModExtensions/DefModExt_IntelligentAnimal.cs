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
    public class DefModExt_IntelligentAnimal : DefModExtension
    {
        public List<WorkTypeDef> enabledWorkTypeDefs = new List<WorkTypeDef>();

        public List<SkillLevelSetting> skillSettings = new List<SkillLevelSetting>();

        public bool automaticTraining = false;
    }
}
