using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Toolbox.ApparelExt
{
    public class DefModExt_RaceSpecificApparel : DefModExtension
    {
        private List<RaceSpecificData> races = new List<RaceSpecificData>();
        private Dictionary<string, RaceSpecificData> dataMap;

        public RaceSpecificData TryGetRaceData(string raceDefName)
        {
            if (dataMap == null)
            {
                dataMap = new Dictionary<string, RaceSpecificData>();
                foreach (var item in races)
                {
                    if (string.IsNullOrWhiteSpace(item.race))
                    {
                        Log.Error(":: O21 Toolbox :: Missing <race> tag in race specific data!");
                        continue;
                    }
                    if (dataMap.ContainsKey(item.race))
                    {
                        Log.Error($":: O21 Toolbox :: There is already apparel data for race '{item.race}'. Do not add it twice!");
                        continue;
                    }

                    // Try to find race...
                    var found = DefDatabase<ThingDef>.GetNamed(item.race, false);
                    if (found == null)
                        Log.Warning($":: O21 Toolbox :: Failed to find race called '{item.race}'. Perhaps the corresponding race mod is not installed?");

                    dataMap.Add(item.race, item);
                }
            }

            if (raceDefName == null)
                return null;

            return dataMap.TryGetValue(raceDefName);
        }

        public class RaceSpecificData
        {
            public string race;
            public string customPath;
            public Vector2? customSize;
            public Color? customColor;
            public bool autoStyle = true;

            private List<string> disallowStyle = new List<string>();

            private static HashSet<string> _dis;

            public bool AllowStyle(string styleName)
            {
                if (_dis == null)
                {
                    _dis = new HashSet<string>();
                    _dis.AddRange(disallowStyle);
                }

                return !_dis.Contains(styleName);
            }
        }
    }
}
