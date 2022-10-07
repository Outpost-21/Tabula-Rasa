using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TabulaRasa
{
    public class ThinkNode_ConditionalIntelligentAnimal : ThinkNode_Conditional
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalIntelligentAnimal obj = (ThinkNode_ConditionalIntelligentAnimal)base.DeepCopy(resolve);
            return obj;
        }

        public override bool Satisfied(Pawn pawn)
        {
            return pawn as Pawn_IntelligentAnimal != null;
        }
    }
}
