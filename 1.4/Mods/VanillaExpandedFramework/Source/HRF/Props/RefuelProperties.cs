using RimWorld;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace HRF
{
	public sealed class ThingDefCountClassFloat : IExposable
	{
		public ThingDef thingDef;

		public float rate;

		public ThingDefCountClassFloat()
		{
		}

		public ThingDefCountClassFloat(ThingDef thingDef, float count)
		{
			this.thingDef = thingDef;
			this.rate = count;
		}

		public void ExposeData()
		{
			Scribe_Defs.Look(ref thingDef, "thingDef");
			Scribe_Values.Look(ref rate, "rate", 1f);
		}

		public void LoadDataFromXmlCustom(XmlNode xmlRoot)
		{
			if (xmlRoot.ChildNodes.Count != 1)
			{
				HRFLog.Error("Misconfigured ThingDefCountClass: " + xmlRoot.OuterXml);
				return;
			}
			DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "thingDef", xmlRoot.Name);
			rate = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
		}

		public override string ToString()
		{
			return "(" + rate + "x " + ((thingDef != null) ? thingDef.defName : "null") + ")";
		}

		public override int GetHashCode()
		{
			return thingDef.shortHash;
		}

		public IngredientCount ToIngredientCount()
		{
			IngredientCount ingredientCount = new IngredientCount();
			ingredientCount.SetBaseCount(rate);
			ingredientCount.filter.SetAllow(thingDef, allow: true);
			return ingredientCount;
		}
	}
    public class RefuelProperties
    {
		public List<ThingDefCountClassFloat> resourcesPerFuelUnit;
		public HediffResourceDef hediffResource;
    }
}
