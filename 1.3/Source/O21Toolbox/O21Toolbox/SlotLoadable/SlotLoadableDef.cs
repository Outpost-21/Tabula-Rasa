using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.SlotLoadable
{
    public class SlotLoadableDef : ThingDef
    {
        public ColorInt colorToChangeTo;
        public ColorInt secondColorToChangeTo;

        public bool doesChangeGraphic = false;

        public bool doesChangeColor = false;

        public bool doesChangeSecondColor = false;

        public bool doesChangeStats = false;

        public List<ThingDef> slottableThingDefs;
    }
}
