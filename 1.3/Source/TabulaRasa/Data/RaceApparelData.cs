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
    public class RaceApparelData
    {
        public string raceDef;
        public string path;
        public Vector2? size;
        public Color? color;
        public bool autoStyle = true;
        public List<string> styleBlacklist = new List<string>();
        public static HashSet<string> _blacklist;

        public bool AllowedStyle(string styleName)
        {
            if(_blacklist == null)
            {
                _blacklist = new HashSet<string>();
                _blacklist.AddRange(styleBlacklist);
            }

            return _blacklist.Contains(styleName);
        }
    }
}
