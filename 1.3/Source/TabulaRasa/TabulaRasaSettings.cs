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
        public bool verboseLogging = false;

        // UpdateListings
        public bool modUpdates = true;
        public List<string> markedAsSeen = new List<String>();

        public override void ExposeData()
        {
            base.ExposeData();

            // UpdateListings
            Scribe_Values.Look(ref this.modUpdates, "modUpdates", true);
            Scribe_Collections.Look(ref this.markedAsSeen, "markedAsSeen");
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
