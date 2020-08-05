using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Utility
{
    public class RemovableHediff : Hediff
    {
        public override bool ShouldRemove
        {
            get
            {
                return true;
            }
        }

        public RemovableHediff()
        {
        }
    }
}
