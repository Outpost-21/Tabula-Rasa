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
        // RaceSpawning shit
        public bool onlyReplaceHumans = true;
        public Dictionary<string, bool> raceSpawningSettings = new Dictionary<string, bool>();
        public Dictionary<string, float> raceSpawningWeights = new Dictionary<string, float>();

        // UpdateListings
        public bool modUpdates = true;
        public List<string> markedAsSeen = new List<String>();

        public bool specialOccasions = true;
        public bool preventEmpireHostility = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref raceSpawningSettings, "raceSpawningSettings");
            Scribe_Collections.Look(ref raceSpawningWeights, "raceSpawningWeights");

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
