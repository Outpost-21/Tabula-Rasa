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
    public class Building_RefundOnDeconstruct : Building
    {
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if(mode == DestroyMode.Deconstruct)
            {
                mode = DestroyMode.Refund;
            }
            base.Destroy(mode);
        }
    }
}
