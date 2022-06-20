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
    public class DefModExt_FoodNeedAdjuster : DefModExtension
    {
        public string newLabel = "Energy";

        public string newDescription = "This is an intentionally renamed food need. This exists to prevent errors in various parts of the game without having to make complicated and intrusive patches.";

        public bool disableFoodNeed = false;

        public bool dieOnEmpty = false;

        public bool wirelesslyCharged = true;

        public HediffDef malnutritionReplacer;
    }
}
