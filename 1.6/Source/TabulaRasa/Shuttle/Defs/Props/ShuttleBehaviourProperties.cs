using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace TabulaRasa.Shuttle
{
    public class ShuttleBehaviourProperties
    {
        public bool canSearchForEnemies = true;

        public bool searchForEnemiesDefault = false;

        public int searchTickInterval = 600;

        public string huntToggleLabel = "TabulaRasa.HuntToggleLabel";

        public string huntToggleOffLabel = "TabulaRasa.HuntToggleOffLabel";

        public string huntToggleDesc = "TabulaRasa.HuntToggleDesc";

        public string huntToggleIcon = "TabulaRasa/UI/Shuttles/Hunt";

        public string huntToggleOffIcon = "TabulaRasa/UI/Shuttles/HuntOff";
    }
}
