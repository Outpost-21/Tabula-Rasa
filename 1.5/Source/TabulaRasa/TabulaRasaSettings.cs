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
        public bool showOldUpdates = false;
        public List<string> markedAsSeen = new List<String>();

        public bool specialOccasions = true;
        public bool showXenotypeEditorMenu = true;

        // Empire Hostility
        public bool preventEmpireHostility = true;
        public Dictionary<string, bool> empireHostilityFixedFactions = new Dictionary<string, bool>();

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

            // Empire Hostility
            Scribe_Values.Look(ref preventEmpireHostility, "preventEmpireHostility", true);
            Scribe_Collections.Look(ref empireHostilityFixedFactions, "empireHostilityFixedFactions");
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
