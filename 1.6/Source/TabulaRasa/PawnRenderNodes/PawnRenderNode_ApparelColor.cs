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
    public class PawnRenderNode_ApparelColor : PawnRenderNode
    {
        public PawnRenderNode_ApparelColor(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {

        }

        public override Color ColorFor(Pawn pawn)
        {
            return apparel.DrawColor;
        }
    }
}
