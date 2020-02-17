using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.Harmony
{
    public class DefModExt_SpaceApparel : DefModExtension
    {
        public spaceEquipmentType equipmentType = spaceEquipmentType.full;
    }

    public enum spaceEquipmentType
    {
        helmet,
        suit,
        full
    }
}
