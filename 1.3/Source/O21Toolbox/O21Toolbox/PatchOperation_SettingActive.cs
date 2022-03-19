using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Verse;

namespace O21Toolbox
{
    public class PatchOperation_SettingActive : PatchOperation
    {
        public List<string> settings;

        public PatchOperation active;

        public PatchOperation inactive;

        public override bool ApplyWorker(XmlDocument xml)
        {
            bool flag = false;
            for (int i = 0; i < settings.Count(); i++)
            {
                if (O21ToolboxMod.settings.GetEnabledSettings.Contains(settings[i]))
                {
                    flag = true;
                    break;
                }
            }

            if (flag)
            {
                if (active != null)
                {
                    return active.Apply(xml);
                }
            }
            else if (inactive != null)
            {
                return inactive.Apply(xml);
            }
            return true;
        }
    }
}
