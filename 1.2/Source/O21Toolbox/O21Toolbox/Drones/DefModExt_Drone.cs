using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Drones
{
    public class DefModExt_Drone : DefModExtension
    {
        public List<WorkTypePriorityPair> capableWorkTypes = new List<WorkTypePriorityPair>();

        public List<SkillLevelSetting> skillLevels = new List<SkillLevelSetting>();
    }

    public class WorkTypePriorityPair
    {
        public WorkTypeDef workType;

        public int priority = 1;
    }

    public class SkillLevelSetting
    {
        public SkillDef skill;

        public int level = 15;
    }
}
