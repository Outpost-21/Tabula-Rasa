using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Abilities
{
    public class TargetAoEProperties
    {
        public bool friendlyFire = false;
        public int maxTargets = 3;
        public int range;
        public bool showRangeOnSelect = true;
        public bool startsFromCaster = true;
        public Type targetClass;
    }
}
