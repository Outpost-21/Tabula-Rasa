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
    public class TabulaRasaSettings : ModSettings
    {
        // UpdateListings
        public bool modUpdates = true;
        public List<string> markedAsSeen = new List<String>();

        // Special PawnGroupMakers
        public bool specialOccasions = true;

        public override void ExposeData()
        {
            base.ExposeData();

            // UpdateListings
            Scribe_Values.Look(ref modUpdates, "modUpdates", true);
            Scribe_Collections.Look(ref markedAsSeen, "markedAsSeen");

            // Special PawnGroupMakers
            Scribe_Values.Look(ref specialOccasions, "specialPawnGroupMakers", true);
        }

        public bool IsValidSetting(string input)
        {
            if (GetType().GetFields().Where(p => p.FieldType == typeof(bool)).Any(i => i.Name == input))
            {
                return true;
            }

            return false;
        }

        public IEnumerable<string> GetEnabledSettings
        {
            get
            {
                return GetType().GetFields().Where(p => p.FieldType == typeof(bool) && (bool)p.GetValue(this)).Select(p => p.Name);
            }
        }
    }
}
