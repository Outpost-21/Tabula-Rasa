using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Verse;

namespace O21Toolbox.PawnCrafter
{
    public class ThingOrderRequest : IExposable
    {
        public ThingDef thingDef;

        public ThingFilter thingFilter = null;

        public float amount;

        public ThingRequest Request()
        {
            if (thingDef != null)
            {
                return ThingRequest.ForDef(thingDef);
            }

            return ThingRequest.ForUndefined();
        }
        
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            if (xmlRoot.ChildNodes.Count != 1)
            {
                Log.Error("Misconfigured ThingOrderRequest: " + xmlRoot.OuterXml);
                return;
            }
            else
            {
                DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "thingDef", xmlRoot.Name);
            }

            amount = (float)ParseHelper.FromString(xmlRoot.FirstChild.Value, typeof(float));
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref thingDef, "thingDef");
            Scribe_Values.Look(ref amount, "amount");
        }
    }
}
