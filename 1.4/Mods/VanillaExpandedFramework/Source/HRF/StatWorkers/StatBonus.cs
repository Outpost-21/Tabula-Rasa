using RimWorld;
using System.Xml;
using Verse;

namespace HRF
{
    public class StatBonus : IExposable
    {
        public StatDef stat;
        public float value;
        public float statOffset;
        public float statFactor;
        public StatBonus()
        {

        }

        public StatBonus(StatDef stat)
        {
            this.stat = stat;
        }
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "stat", xmlRoot.Name);
            value = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
        }
        public void ExposeData()
        {
            Scribe_Defs.Look(ref stat, "stat");
            Scribe_Values.Look(ref value, "value");
            Scribe_Values.Look(ref statOffset, "statOffset");
            Scribe_Values.Look(ref statFactor, "statFactor");
        }
    }
}
