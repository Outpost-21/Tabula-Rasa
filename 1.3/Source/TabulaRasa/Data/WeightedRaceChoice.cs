using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using Verse;

namespace TabulaRasa
{
    public sealed class WeightedRaceChoice : IExposable
    {
        public ThingDef race;

        public float weight;

		public string Label => $"Race: {race} :: Weight: {weight}";

		public string LabelCap => Label.CapitalizeFirst(race);

		public string Summary => weight + "x " + ((race != null) ? race.label : "null");

		public WeightedRaceChoice()
		{
		}

		public WeightedRaceChoice(ThingDef thingDef, float count)
		{
			if (count < 0)
			{
				Log.Warning("Tried to set ThingDefCountClass count to " + count + ". thingDef=" + thingDef);
				count = 0;
			}
			this.race = thingDef;
			this.weight = count;
		}

		public void ExposeData()
		{
			Scribe_Defs.Look(ref race, "thingDef");
			Scribe_Values.Look(ref weight, "count", 1);
		}

		public void LoadDataFromXmlCustom(XmlNode xmlRoot)
		{
			if (xmlRoot.ChildNodes.Count != 1)
			{
				Log.Error("Misconfigured WeightedRaceChoice: " + xmlRoot.OuterXml);
				return;
			}
			DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "thingDef", xmlRoot.Name);
			weight = ParseHelper.FromString<int>(xmlRoot.FirstChild.Value);
		}

		public override string ToString()
		{
			return "(" + weight + "x " + ((race != null) ? race.defName : "null") + ")";
		}

		public override int GetHashCode()
		{
			return race.shortHash + ((int)weight) << 16;
		}

		public static implicit operator WeightedRaceChoice(ThingDefCount t)
		{
			return new WeightedRaceChoice(t.ThingDef, t.Count);
		}
	}
}
