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
    public abstract class FloatMenuPatch
    {
        public abstract IEnumerable<KeyValuePair<Condition, Func<Vector3, Pawn, Thing, List<FloatMenuOption>>>> GetFloatMenus();
    }
}
