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
    public class ApparelAlts
    {
        public List<string> headTypeDef;
        public string path;
        public bool affectStyles = true;
        public List<string> styleWhitelist = new List<string>();
        public static HashSet<string> whitelist;
        public List<string> styleBlacklist = new List<string>();
        public static HashSet<string> blacklist;

        public bool IsAllowedStyle(string styleName)
        {
            if(blacklist == null)
            {
                blacklist = new HashSet<string>();
                blacklist.AddRange(styleBlacklist);
            }

            return blacklist.Contains(styleName);
        }
    }
}
