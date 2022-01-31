using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace O21Toolbox.Projectiles
{
    public class DefModExt_EventCauser : DefModExtension
    {
        public List<Condition> conditions;
    }

    public class Condition
    {
        public GameConditionDef def;
        public float chance;
        public IntRange duration;
    }
}
