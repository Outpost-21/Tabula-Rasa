
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace TabulaRasa
{
    public class Building_RandomGraphic : Building
    {

        public override void PostMake()
        {
            base.PostMake();
            Graphic_Random rg = (base.Graphic as Graphic_Random) ?? ((base.Graphic as Graphic_Linked).subGraphic as Graphic_Random);
            this.overrideGraphicIndex = new int?(Rand.RangeInclusive(0, rg.SubGraphicsCount));
        }
    }
}
