using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox
{
    public class O21ToolboxSettings : ModSettings
    {
        public static O21ToolboxSettings Instance;

        public O21ToolboxSettings()
        {
            O21ToolboxSettings.Instance = this;
        }

        public override void ExposeData()
        {

        }

        public void ResetToDefault()
        {

        }
    }
}
