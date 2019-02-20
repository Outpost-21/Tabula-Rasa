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

        public bool FirstStartUp = true;
        public bool EnergyNeedCompatMode = true;

        public override void ExposeData()
        {
            Scribe_Values.Look<bool>(ref this.FirstStartUp, "FirstStartUp", true, true);
            Scribe_Values.Look<bool>(ref this.EnergyNeedCompatMode, "EnergyNeedCompatMode", true, true);
        }

        public void ResetToDefault()
        {

        }

        public void SetBool(ref bool b, bool set)
        {
            b = set;
        }

        public void SetValue(ref int i, int set)
        {
            i = set;
        }
    }
}
