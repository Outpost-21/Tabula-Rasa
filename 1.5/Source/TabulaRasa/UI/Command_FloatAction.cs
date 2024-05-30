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
    public class Command_FloatAction : Command_Action
    {
        public Func<IEnumerable<FloatMenuOption>> floatMenuFunc;

        public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions => floatMenuFunc?.Invoke();
    }
}
