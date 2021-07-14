using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SettingActiveAttribute : Attribute
    {
        public string setting;

        public SettingActiveAttribute(string setting)
        {
            this.setting = setting;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class SettingInactiveAttribute : Attribute
    {
        public string setting;

        public SettingInactiveAttribute(string setting)
        {
            this.setting = setting;
        }
    }
}
