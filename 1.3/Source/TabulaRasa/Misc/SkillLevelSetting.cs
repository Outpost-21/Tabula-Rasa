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
    public class SkillLevelSetting
    {
        public SkillDef skill;

        public int level;

        public bool setPassion = false;

        public Passion passionLevel = Passion.None;
    }
}
